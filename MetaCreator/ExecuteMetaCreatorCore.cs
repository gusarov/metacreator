using System;
using System.Diagnostics;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using MetaCreator.Utils;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MyUtils;

namespace MetaCreator
{
	internal class ExecuteMetaCreatorCore
	{
		internal ExecuteMetaCreatorCore()
		{

		}

		public ExecuteMetaCreatorCore(ITaskItem[] sources, ITaskItem[] references, string intermediateOutputPath, string projDir, IBuildErrorLogger buildErrorLogger)
		{
			Sources = sources;
			References = references;
			IntermediateOutputPath = intermediateOutputPath;
			ProjDir = projDir;
			BuildErrorLogger = buildErrorLogger;
		}

		#region INPUTS
		public ITaskItem[] Sources { get; set; }
		public ITaskItem[] References { get; set; }
		public string IntermediateOutputPath { get; set; }
		public string ProjDir { get; set; }
		public IBuildErrorLogger BuildErrorLogger { get; set; }
		#endregion

		#region OUTPUTS
		readonly List<ITaskItem> _addFiles = new List<ITaskItem>();

		public IEnumerable<ITaskItem> AddFiles
		{
			get { return _addFiles.AsReadOnly(); }
		}

		readonly List<ITaskItem> _removeFiles = new List<ITaskItem>();

		public IEnumerable<ITaskItem> RemoveFiles
		{
			get { return _removeFiles.AsReadOnly(); }
		} 

		#endregion

		#region Fields and Consts
		const string _metacreatorErrorPrefix = "MetaCode: ";
		static readonly Regex _rxBlock = new Regex(@"(?s)(?<=(?'p'.)?)/\*(?'type'[@+=!])(.+?)\*/");
		internal static readonly Regex _rxStringInterpolVerbatim = new Regex(@"@""([^""]+)""");
		internal static readonly Regex _rxStringInterpolInside = new Regex(@"{([^\d].*?)}");
		internal static readonly Regex _rxStringInterpolNoVerbatim = new Regex(@"(?<!@)""(.*?[^\\])""");

		static AppDomain _appDomain;
		Lazy<AnotherAppDomApi> _anotherAppDom;
		AnotherAppDomApi AnotherAppDom { get { return _anotherAppDom; } }

		#endregion

		static string OriginalLinePredirrectiveAtStartOfMacroBlock(ProcessFileCtx ctx)
		{
			var banner = (ctx.GenerateBanner) ? @"// <MetaCreatorBanner>
// DO NOT MODIFY THIS GENERATED CODE
// You can use /*@ GenerateBanner off */ to disable this message
" : null;
			string lineRemap = null;
			if (ctx.ErrorRemap)
			{
				lineRemap = "#line default";
			}
			return CombineLines(lineRemap, banner);
		}

		static string OriginalLinePredirrectiveAtEndOfMacroBlock(ProcessFileCtx ctx)
		{
			var banner = (ctx.GenerateBanner) ? @"// </MetaCreatorBanner>" : null;
			string lineRemap = null;
			if (ctx.ErrorRemap)
			{
				lineRemap = "#line {0} \"{1}\"".Arg(ctx.CurrentMacrosEndLineInOriginalFile + 1, ctx.GetOriginalFileNameRelativeToIntermediatePath());
			}
			return CombineLines(banner, lineRemap);
		}

		static string CombineLines(params string[] lines)
		{
			if(lines.Length==2)
			{
				var line1 = lines[0];
				var line2 = lines[1];
				if (line1 != null && line1[line1.Length - 1] != Environment.NewLine[Environment.NewLine.Length - 1])
				{
					line1 += Environment.NewLine;
				}
				return line1 + line2;
			}
			if (lines.Length<2)
			{
				throw new Exception();
			}
			var linea = lines[0];
			foreach (var line in lines.Skip(1))
			{
				linea = CombineLines(linea, line);
			}
			return linea;
		}

		internal void ProcessFile(ProcessFileCtx ctx)
		{
			ctx.FileProcessedContent = ctx.FileOriginalContent;

			ctx.FileProcessedContent = _rxBlock.Replace(ctx.FileProcessedContent, match =>
				{
					SaveCaptureState(ctx, match);

					var type = match.Groups["type"].Value;
					switch (type)
					{
						//case "+":
						// BuildErrorLogger.LogWarningEvent(null);
						//	return null;
						case "!":
							var result = Evaluate(match.Groups[1].Value, false, ctx);
							if (!Environment.NewLine.Contains(match.Groups["p"].Value))
							{
								result = Environment.NewLine + result;
							}
							return result;
						case "=":
							return Evaluate(match.Groups[1].Value, true, ctx);
						case "@":
							ProcessExtender(match.Groups["1"].Value, ctx);
							return null;
						default:
							throw new Exception("Block type '{0}'".Arg(type));
					}
				});

			if (ctx.EnabledStringInterpolation)
			{
				ctx.FileProcessedContent = _rxStringInterpolNoVerbatim.Replace(ctx.FileProcessedContent, match =>
					{
						var stringValue = match.Groups[1].Value;
						stringValue = _rxStringInterpolInside.Replace(stringValue, m =>
							{
								ctx.NumberOfMacrosProcessed++;
								var val = m.Groups[1].Value;
								//if(string.IsNullOrEmpty(val))
								//{
								//   return null;
								//}
								return "\"+" + val + "+\"";
							});
						stringValue = "\"" + stringValue + "\"";
						// trim "" + and + ""
						if (stringValue.StartsWith("\"\"+"))
						{
							stringValue = stringValue.Substring(3);
						}
						if (stringValue.EndsWith("+\"\""))
						{
							stringValue = stringValue.Substring(0, stringValue.Length - 3);
						}
						return stringValue;
					});

				ctx.FileProcessedContent = _rxStringInterpolVerbatim.Replace(ctx.FileProcessedContent, match =>
					{
						var stringValue = match.Groups[1].Value;
						stringValue = _rxStringInterpolInside.Replace(stringValue, m =>
							{
								ctx.NumberOfMacrosProcessed++;
								var val = m.Groups[1].Value;
								//if (string.IsNullOrEmpty(val))
								//{
								//   return null;
								//}
								return "\"+" + val + "+@\"";
							});
						stringValue = "@\"" + stringValue + "\"";
						return stringValue;
					});
			}

		}

		public void Initialize()
		{
			_anotherAppDom = Lazy.New(() =>
				{
					_appDomain = AppDomain.CreateDomain("MetaCreator - unloadable part");
					AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
					var anotherAppDom =
						(AnotherAppDomApi)
							_appDomain.CreateInstanceFromAndUnwrap(typeof (AnotherAppDomApi).Assembly.Location,
								typeof (AnotherAppDomApi).FullName);
					AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
					if (anotherAppDom == null)
					{
						throw new Exception("Can not create another app domain");
					}
					return anotherAppDom;
				});

#if DEBUG
			if(string.IsNullOrEmpty(ProjDir))
			{
				throw new Exception("ProjDir not defined");
			}
			if(string.IsNullOrEmpty(IntermediateOutputPath))
			{
				throw new Exception("IntermediateOutputPath not defined");
			}
			if(Sources.OrEmpty().Count()<=0)
			{
				throw new Exception("Sources not defined");
			}
#endif


			if (ProjDir != Path.GetFullPath(ProjDir))
			{
				throw new Exception("ProjDir is not full path");
			}

			if (IntermediateOutputPath != Path.GetFullPath(IntermediateOutputPath))
			{
				IntermediateOutputPath = Path.Combine(ProjDir, IntermediateOutputPath);
			}

			if (Sources == null)
			{
				Sources = new ITaskItem[0];
			}
			
			if (References == null)
			{
				References = new ITaskItem[0];
			}
		}

		public bool Execute()
		{
			//Debugger.Launch();
			Initialize();

			try
			{
				int totalMacrosProcessed = 0;
				var totalTime = Stopwatch.StartNew();
				foreach (var sourceFile in Sources)
				{
					var fileName = sourceFile.ItemSpec;
					var replacementFileName =
								Path.GetFullPath(Path.Combine(IntermediateOutputPath,
									Path.GetFileNameWithoutExtension(fileName) + ".g" + Path.GetExtension(fileName)));

					var ctx = new ProcessFileCtx
						{
							BuildErrorLogger = BuildErrorLogger,
							OriginalFileName = fileName,
							FileOriginalContent = File.ReadAllText(fileName),
							ReplacementFileName = replacementFileName,
							IntermediateOutputPath = IntermediateOutputPath,
							ProjDir = ProjDir,
							ReferencesOriginal = References.Select(x => x.ItemSpec).ToArray(),
						};

					//var iOfNs = ctx.FileOriginalContent.IndexOf('{');
					//var globalUsings = ctx.FileOriginalContent.Substring(0, iOfNs).TrimEnd('}').Trim();
					ctx.NamespaceImportsOriginal = Regex.Matches(ctx.FileOriginalContent, @"(?m)^using\s+([^;]+)").Cast<Match>().Select(x => x.Groups[1].Value).ToArray();

					// Log("Content = " + ctx.FileOriginalContent);
					// Log("NamespaceImportsOriginal = " + string.Join(", ", ctx.NamespaceImportsOriginal.ToArray()));
					// Log("Namespaces = " + string.Join(", ", ctx.Namespaces.ToArray()));

					ProcessFile(ctx);

					if (ctx.NumberOfMacrosProcessed > 0)
					{
						if (ctx.ErrorRemap)
						{
							ctx.FileProcessedContent = "#line 1 \"{0}\"\r\n".Arg(ctx.GetOriginalFileNameRelativeToIntermediatePath()) + ctx.FileProcessedContent;
						}
						totalMacrosProcessed += ctx.NumberOfMacrosProcessed;
						Log(fileName + " - " + ctx.NumberOfMacrosProcessed + " macros processed");
						var dir = Path.GetDirectoryName(replacementFileName);
						if (!Directory.Exists(dir))
						{
							Directory.CreateDirectory(dir);
						}

						var theSameContent = File.Exists(replacementFileName) &&
							File.ReadAllText(replacementFileName) == ctx.FileProcessedContent;

						if (!theSameContent)
						{
							//if (File.Exists(replacementFileName))
							//{
							//   File.SetAttributes(replacementFileName, File.GetAttributes(replacementFileName) & ~FileAttributes.ReadOnly);
							//}
							File.WriteAllText(replacementFileName, ctx.FileProcessedContent);
							//File.SetAttributes(replacementFileName, File.GetAttributes(replacementFileName) | FileAttributes.ReadOnly);
						}
						_removeFiles.Add(sourceFile);
						_addFiles.Add(new TaskItem(replacementFileName));
					}
				}

				if (totalMacrosProcessed == 0)
				{
					Log("No macros found. Nothing changed. Duration = " + totalTime.ElapsedMilliseconds+"ms");
				}
				else
				{
					Log("Duration = " + totalTime.ElapsedMilliseconds + "ms");
				}

				return true;
			}
			catch
			{
				if (_macrosFailed && BuildErrorLogger.ErrorsExists)
				{
					// already logged to msbuild
					return false;
				}
				throw;
			}
			finally
			{
				if (_appDomain != null)
				{
					AppDomain.Unload(_appDomain);
					foreach (var dir in _tempPathToRemoveAfterUnloadAppDomain)
					{
						try
						{
							Directory.Delete(dir, true);
						}
						catch
						{
						}
					}
					_tempPathToRemoveAfterUnloadAppDomain.Clear();
				}
			}
		}

		static void SaveCaptureState(ProcessFileCtx ctx, Capture match)
		{
			ctx.CurrentMacrosIndex = match.Index;
			ctx.CurrentMacrosLength = match.Length;
			ctx.NumberOfMacrosProcessed++;
		}

		static void ProcessExtender(string extender, ProcessFileCtx ctx)
		{
			extender = extender.Trim();
			var name = CutFirstWord(ref extender);
			Extenders.ExecuteExtender(name, extender, ctx);
		}

		static string CutFirstWord(ref string str)
		{
			string word;
			var i = str.IndexOf(' ');
			if (i < 0)
			{
				word = str;
				str = string.Empty;
			}
			else
			{
				word = str.Substring(0, i).Trim();
				str = str.Substring(i).Trim();
			}
			return word;
		}

		static readonly List<string> _tempPathToRemoveAfterUnloadAppDomain = new List<string>();

		string Evaluate(string code, bool isExpression, ProcessFileCtx ctx)
		{
			var result = AnotherAppDom.EvaluateGeneratorCore(code, isExpression, ctx.References, ctx.Namespaces);
			_tempPathToRemoveAfterUnloadAppDomain.Add(result.CompileTempPath);
			ProcessEvaluationResult(result, ctx);

			var processed = result.ResultBody.SafeToString();

			if (!isExpression)
			{
				processed = CombineLines(OriginalLinePredirrectiveAtStartOfMacroBlock(ctx), processed, OriginalLinePredirrectiveAtEndOfMacroBlock(ctx));
			}

			return processed;
		}

		bool _macrosFailed;

		void ProcessEvaluationResult(EvaluationResult result, ProcessFileCtx ctx)
		{

			// Log meta code compile time warnings
			if (result.Warnings != null)
			{
				foreach (var error in result.Warnings)
				{
					BuildErrorLogger.LogWarningEvent(CreateBuildWarning(result, error, ctx));
				}
			}

			// Log meta code compile time errors
			if (result.Errors != null)
			{
				foreach (var error in result.Errors)
				{
					_macrosFailed = true;
					BuildErrorLogger.LogErrorEvent(CreateBuildError(result, error, ctx));
				}
			}

			// Log meta code run time exceptions
			if (result.EvaluationException != null)
			{
				_macrosFailed = true;
				// var linenumber = result.EvaluationException.
				var message = result.EvaluationException.GetType().FullName + ": " + result.EvaluationException.Message;
				Log(message);
				var line = RemapErrorLineNumber(result.EvaluationExceptionAtLine, ctx, result); 
				BuildErrorLogger.LogErrorEvent(new BuildErrorEventArgs(null, null, ctx.OriginalFileName,line , 0, 0, 0, _metacreatorErrorPrefix + message, null, null));
			}

			// terminate
			if (_macrosFailed)
			{
				throw new Exception("$ terminating, jump to global catch and return false...");
			}
		}

		BuildWarningEventArgs CreateBuildWarning(EvaluationResult result, CompilerError error, ProcessFileCtx ctx)
		{
			BuildError_GetLineNumber(error, result, ctx); // init non user code
			return new BuildWarningEventArgs(null, null, BuildError_GetFile(ctx, result), BuildError_GetLineNumber(error, result, ctx),
				BuildError_GetColumnNumber(error), 0, 0, BuildError_GetMessage(error, result),
				null, null);
		}

		BuildErrorEventArgs CreateBuildError(EvaluationResult result, CompilerError error, ProcessFileCtx ctx)
		{
			BuildError_GetLineNumber(error, result, ctx); // init non user code
			return new BuildErrorEventArgs(null, null, BuildError_GetFile(ctx, result), BuildError_GetLineNumber(error, result, ctx),
				BuildError_GetColumnNumber(error), 0, 0, BuildError_GetMessage(error, result),
				null, null);
		}

		static int BuildError_GetColumnNumber(CompilerError error)
		{
			return error.Column;
		}

		const int _nonUserCodeSpecialLineNumber = -777;

		int BuildError_GetLineNumber(CompilerError error, EvaluationResult result, ProcessFileCtx ctx)
		{
			var line = BuildError_GetLineNumberCore(error, result, ctx);
			if (line == _nonUserCodeSpecialLineNumber)
			{
				if (ctx.ErrorRemap)
				{
					result.NonUserCode = "...";
				}
			}
			return line;
		}

		static int BuildError_GetLineNumberCore(CompilerError error, EvaluationResult result, ProcessFileCtx ctx)
		{
			if (!ctx.ErrorRemap)
			{
				return error.Line;
			}
			if (result.NonUserCode != null)
			{
				return error.Line;
			}

			return RemapErrorLineNumber(error.Line, ctx, result);
		}

		/// <summary>
		/// Allow to receive line number for compile time or run time error
		/// </summary>
		/// <param name="errorLine"></param>
		/// <param name="ctx"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private static int RemapErrorLineNumber(int errorLine, ProcessFileCtx ctx, EvaluationResult result)
		{
			var userCodeIndex = result.SourceCode.IndexOf("// <UserCode>");
			var userCodeLine = result.SourceCode.Substring(0, userCodeIndex).ToCharArray().Count(x => x == '\r') + 2; // +\r + next line

			if (userCodeIndex < 1)
			{
				return _nonUserCodeSpecialLineNumber;
			}
			if (errorLine < userCodeLine)
			{
				return _nonUserCodeSpecialLineNumber;
			}
			if (userCodeLine < 0)
			{
				return _nonUserCodeSpecialLineNumber;
			}
			return errorLine -
				userCodeLine + ctx.CurrentMacrosLineInOriginalFile;
		}

		string BuildError_GetMessage(CompilerError error, EvaluationResult result)
		{
			// Log a message here. It is not very convinient considering a role of this procedure
			{
				var source = result.SourceCode;
				// Annotate source code with line numbers
				{
					source = string.Join(Environment.NewLine, source.Split('\r').Select((x, i) => (i + 1).ToString("00") + "| " + x.Trim('\n')).ToArray());
				}
				var fullLogEntry = error.ErrorText + " at line " + error.Line + " col " + error.Column + "\r\n" + source;
				Log(fullLogEntry);
			}
			return _metacreatorErrorPrefix + error.ErrorText;
		}

		static string BuildError_GetFile(ProcessFileCtx ctx, EvaluationResult result)
		{
			if (result.NonUserCode!=null || !ctx.ErrorRemap)
			{
				result.NonUserCode = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(ctx.OriginalFileName) + ".meta" + Path.GetExtension(ctx.OriginalFileName));
				File.WriteAllText(result.NonUserCode, result.SourceCode);
				return result.NonUserCode;
			}
			return ctx.OriginalFileName;
		}

		static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return Assembly.LoadFrom(typeof(ExecuteMetaCreatorCore).Assembly.Location);
		}

		void Log(string msg)
		{
			BuildErrorLogger.LogOutputMessage(msg);
		}
	}
}
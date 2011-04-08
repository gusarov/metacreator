using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MetaCreator.Utils;
using Microsoft.Build.Framework;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Builder of metacode for execution
	/// </summary>
	class Code1Builder
	{
		ProcessFileCtx _ctx;

		public Code1Builder()
		{
			_mapExtenders = new Dictionary<string, Action<string>>
			{
				{"stringinterpolation", StringInterpolation},
				{"errorremap", ErrorRemap},
				{"reference", Reference},
				{"using", Using},
				{"generatebanner", GenerateBanner},
				{"csharpversion", CSharpVersion},
				{"encoding", OverrideEncoding},
			};
		}

		private void OverrideEncoding(string encName)
		{
			encName = encName.Trim();

			int i;
			if (int.TryParse(encName, out i))
			{
				_ctx.OverrideEncoding = Encoding.GetEncoding(i);
			}
			else
			{
				var encs = Encoding.GetEncodings();
				var enc = encs.FirstOrDefault(x => string.Equals(encName, x.Name, StringComparison.OrdinalIgnoreCase));
				if (enc == null)
				{
					WriteError("Unknown encoding. Use one of:" +
					           string.Join("", encs.Select(x => Environment.NewLine + x.Name).ToArray()));
				}
				else
				{
					_ctx.OverrideEncoding = enc.GetEncoding();
				}
			}
		}

		void CSharpVersion(string version)
		{
			_ctx.CSharpVersion = version;
		}

		const string _skeleton = @"// METACODE
// This file is generated and compiled in-memory.
// If there are any compilation errors - file can be saved to temporary path for working with IDE.

#region Imports
{0}
#endregion

public static class Generator
{{
	public static string Run()
	{{
#region methodbody

		{1}

#endregion
		return Result.ToString();
	}}

#region classbody

{2}

#endregion

#region utils

	public static StringBuilder Result = new StringBuilder();

	public static void Write(string msg, params object[] args)
	{{
		Result.AppendFormat(msg, args);
	}}

	public static void WriteLine(string msg, params object[] args)
	{{
		Result.AppendFormat(msg + Environment.NewLine, args);
	}}

	public static void Write(string msg)
	{{
		Result.Append(msg);
	}}

	public static void WriteLine(string msg)
	{{
		Result.AppendLine(msg);
	}}

	public static void Write(object obj)
	{{
		Result.Append(obj == null ? string.Empty : obj.ToString());
	}}

	public static void WriteLine(object obj)
	{{
		Result.AppendLine(obj == null ? string.Empty : obj.ToString());
	}}

	public static void WriteLine()
	{{
		Result.AppendLine();
	}}

#endregion
}}";

		static readonly string[] _defaultUsings = new[]
		{
			"global::Microsoft.Win32",
			"global::System",
			"global::System.Collections",
			"global::System.Collections.Generic",
			"global::System.Collections.Specialized",
			"global::System.Collections.ObjectModel",
			"global::System.Configuration.Assemblies",
			"global::System.ComponentModel",
			"global::System.Diagnostics",
			"global::System.Diagnostics.CodeAnalysis",
			"global::System.Globalization",
			"global::System.IO",
			"global::System.IO.Compression",
			"global::System.IO.Ports",
			"global::System.Media",
			"global::System.Net",
			"global::System.Net.Sockets",
			"global::System.Reflection",
			"global::System.Resources",
			"global::System.Runtime.CompilerServices",
			"global::System.Runtime.InteropServices",
			"global::System.Runtime.InteropServices.ComTypes",
			"global::System.Runtime.Serialization",
			"global::System.Runtime.Serialization.Formatters.Binary",
			"global::System.Security",
			"global::System.Security.Cryptography",
			"global::System.Security.Cryptography.X509Certificates",
			"global::System.Security.Permissions",
			"global::System.Security.Policy",
			"global::System.Text",
			"global::System.Text.RegularExpressions",
			"global::System.Threading",
			"global::System.CodeDom.Compiler",
			"global::System.Linq",
		};

		readonly StringBuilder _methodBody = new StringBuilder();
		readonly StringBuilder _classBody = new StringBuilder();

		// write string from original file to resulting file by metacode
		void PlainTextWriter(string substring, StringBuilder sb, int line = 0)
		{
			if (line > 0 && _errorRemap)
			{
				sb.AppendLine("WriteLine();");
				sb.Append("WriteLine(@\"");
				sb.Append("#line " + line + " \"\"" + _ctx.GetOriginalFileNameRelativeToIntermediatePath() + "\"\"");
				sb.AppendLine("\");");
			}

			sb.Append("Write(@\"");
			sb.Append(substring.Replace("\"", "\"\""));
			sb.AppendLine("\");");

			if (line > 0 && _errorRemap)
			{
				sb.AppendLine("WriteLine();");
				sb.AppendLine(@"WriteLine(""#line default"");");
			}
		}

//		static string OriginalLinePredirrectiveAtStartOfMacroBlock(ProcessFileCtx _ctx)
//		{
			//			var banner = (_ctx.GenerateBanner) ? @"// <MetaCreator>
//			//// DO NOT MODIFY THIS GENERATED CODE
//			//// You can use /*@ GenerateBanner off */ to disable this message
			//" : null;
//			string lineRemap = null;
//			if (_ctx.ErrorRemap)
//			{
//				lineRemap = "#line default";
//			}
//			return CombineLines(lineRemap, banner);
//		}

//		static string CombineLines(params string[] lines)
//		{

//			if (lines.Length == 2)
//			{
//				var line1 = lines[0];
//				var line2 = lines[1];
//				if (line1 != null && line1[line1.Length - 1] != Environment.NewLine[Environment.NewLine.Length - 1])
//				{
//					line1 += Environment.NewLine;
//				}
//				return line1 + line2;
//			}
//			if (lines.Length < 2)
//			{
//				throw new Exception();
//			}
//			var linea = lines[0];
//			foreach (var line in lines.Skip(1))
//			{
//				linea = CombineLines(linea, line);
//			}
//			return linea;
//		}

//		static string OriginalLinePredirrectiveAtEndOfMacroBlock(ProcessFileCtx _ctx)
//		{
//			var banner = (_ctx.GenerateBanner) ? @"// </MetaCreator>" : null;
//			string lineRemap = null;
//			if (_ctx.ErrorRemap)
//			{
//				lineRemap = "#line {0} \"{1}\"".Arg(_ctx.CurrentMacrosEndLineInOriginalFile + 1, _ctx.GetOriginalFileNameRelativeToIntermediatePath());
//			}
//			return CombineLines(banner, lineRemap);
//		}

//		string GetCode(string methodBody, string classBody, string returnType, IEnumerable<string> imports)
//		{
			//							var result = Evaluate(match.Groups[1].Value, false, _ctx);
//							if (!Environment.NewLine.Contains(match.Groups["p"].Value))
//							{
//								result = Environment.NewLine + result;
//							}
//							return result;
//							return Evaluate(match.Groups[1].Value, true, _ctx);
//
//
//			return
//				string.Format(@"
//
//",
//				              string.Join(Environment.NewLine, imports.Select(x => "using " + x + ";").ToArray()),
//				              _generatorClassName,
//				              methodBody,
//				              _generatorMethodName,
//				              returnType,
//				              _generatorResultPropertyName,
//					);
//		}

		static readonly Regex _rxBlock = new Regex(@"(?sm)(?<=(?'pre'^.*?)?)/\*(?'type'[@+=!])(?'body'.+?)\*/");
		string[] _namespacesFromOriginalFile;
		string _code;

		public string Build(string code, ProcessFileCtx ctx)
		{
			_ctx = ctx;
			_code = code;
			code = Preprocess(code);
			var metacode = BuildMetacode(ctx, code);
			return metacode;
		}

		string Preprocess(string code)
		{
			foreach (Match match in _rxBlock.Matches(code))
			{
				SaveCaptureState(match);
				var blockType = match.Groups["type"].Value[0];
				switch (blockType)
				{
					case '@':
						ExecuteExtender(match.Groups["body"].Value);
						break;
				}
			}
			if(_enabledStringInterpolation)
			{
				code = ProcessStringInterpolation(code);
			}
			return code;
		}

		string BuildMetacode(ProcessFileCtx ctx, string code)
		{
			_ctx = ctx;
			_code = code;

			// auto import same namespaces
			_namespacesFromOriginalFile = Regex.Matches(code, @"(?m)^using\s+([^;]+)").Cast<Match>().Select(x => x.Groups[1].Value).ToArray();

			var blocks = _rxBlock.Matches(code).Cast<Match>().ToArray();

			int from = 0;
			bool isFirstBlock = true;
			// indicate that it is "something /*! asd */ " but not "/*! asdsad */"
			bool isInsertion = false;
			// create metacode
			foreach (var block in blocks)
			{
				_ctx.NumberOfMetaBlocksProcessed++;
				SaveCaptureState(block);

				var type = block.Groups["type"].Value[0];
				var value = block.Groups["body"].Value;
				var pre = block.Groups["pre"].Value;

				isInsertion = pre.Trim().Any();

				var writeLineRemap = isFirstBlock && !isInsertion;
				// write previous text as plain
				PlainTextWriter(code.Substring(from, block.Index - from), _methodBody, writeLineRemap ? GetLineNumberByIndex(code, from) : 0);

				isFirstBlock = false;

				from = block.Index + block.Length;
				switch (type)
				{
					case '@':
						//ExecuteExtender(value);
						break;
					case '!':
						// TODO detect, is it new line or just some insertion
						if (_errorRemap)
						{
							_methodBody.AppendLine("#line " + GetLineNumberByIndex(code, _currentBlockIndex) + ' ' + '"' +
								_ctx.GetOriginalFileNameRelativeToIntermediatePath() + '"');
						}
						_methodBody.AppendLine(value);
						if (_errorRemap)
						{
							_methodBody.AppendLine("#line default");
						}
						break;
					case '=':
						_methodBody.AppendLine("Write(" + value + ");");
						break;
					case '+':
						_classBody.AppendLine("#line " + GetLineNumberByIndex(code, _currentBlockIndex) + ' ' + '"' + _ctx.GetOriginalFileNameRelativeToIntermediatePath() + '"');
						_classBody.AppendLine(value);
						_classBody.AppendLine("#line default");
						break;
					default:
						throw new Exception("Block with type " + type + " unexpected");
				}
			}

			PlainTextWriter(code.Substring(from), _methodBody, isInsertion ? 0 : GetLineNumberByIndex(code, from));


			var imports = _namespacesFromOriginalFile.OrEmpty().Concat(_defaultUsings).Concat(_namespaceImportsMetaAdditional.OrEmpty()).Distinct();
			var importsAsString = string.Join(Environment.NewLine, imports.Select(x => "using " + x + ";").ToArray());
			return _skeleton.Arg(importsAsString, _methodBody, _classBody);
		}

		public void SaveCaptureState(Capture match)
		{
			_currentBlockIndex = match.Index;
			_currentBlockLength = match.Length;
			_currentBlockFinishIndex = _currentBlockIndex + _currentBlockLength;
		}

		int _currentBlockIndex;
		int _currentBlockLength;
		int _currentBlockFinishIndex;

		public int GetLineNumberByIndex(string code, int i)
		{
			return code
				.Substring(0, i)
				.Split('\r').Length;
		}

		internal static readonly Regex _rxStringInterpolVerbatim = new Regex(@"@""([^""]+)""");
		internal static readonly Regex _rxStringInterpolInside = new Regex(@"{([^\d].*?)}");
		internal static readonly Regex _rxStringInterpolNoVerbatim = new Regex(@"(?<!@)""(.*?[^\\])""");

		string ProcessStringInterpolation(string code)
		{
			code = _rxStringInterpolNoVerbatim.Replace(code, match =>
			{
				var stringValue = match.Groups[1].Value;
				stringValue = _rxStringInterpolInside.Replace(stringValue, m =>
				{
					SaveCaptureState(match);
					var val = m.Groups[1].Value;
					if(string.IsNullOrEmpty(val))
					{
					   return null;
					}
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

			code = _rxStringInterpolVerbatim.Replace(code, match =>
			{
				var stringValue = match.Groups[1].Value;
				stringValue = _rxStringInterpolInside.Replace(stringValue, m =>
				{
					SaveCaptureState(match);
					var val = m.Groups[1].Value;
					if (string.IsNullOrEmpty(val))
					{
					   return null;
					}
					return "\"+" + val + "+@\"";
				});
				stringValue = "@\"" + stringValue + "\"";
				return stringValue;
			});
			return code;
		}

		readonly Dictionary<string, Action<string>> _mapExtenders;

		bool _enabledStringInterpolation;


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

		public void ExecuteExtender(string extender)
		{
			extender = extender.Trim();

			var name = CutFirstWord(ref extender);
			var args = extender;

			Action<string> value;
			if (_mapExtenders.TryGetValue(name.ToLowerInvariant(), out value))
			{
				value(args);
			}
			else
			{
				var knownExtenders = string.Join(string.Empty, _mapExtenders.Keys.Select(x => Environment.NewLine + x).ToArray());

				WriteWarning("Meta Extender '{0}' is unknown. Known extenders is:{1}", name, knownExtenders);
				//_ctx.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, _ctx.OriginalFileName,
				//GetLineNumberByIndex(_code, _currentBlockIndex), 0, 0, 0,
				//"Meta Extender '{0}' is unknown. Known extenders is:{1}".Arg(name.ToLowerInvariant(), knownExtenders), null, null));
			}
		}

		static bool ToBool(string val)
		{
			if (val == null)
			{
				return true;
			}
			switch (val.ToLowerInvariant().Trim())
			{
				case "enable":
				case "enabled":
				case "on":
				case "1":
				case "true":
				case "yes":
				case "":
					return true;
				case "disable":
				case "disabled":
				case "off":
				case "0":
				case "false":
				case "no":
					return false;
				default:
					throw new Exception("Can not convert '{0}' to bool".Arg(val));
			}
		}

		void StringInterpolation(string arg)
		{
			_enabledStringInterpolation = ToBool(arg);
		}

		void ErrorRemap(string arg)
		{
			_errorRemap = ToBool(arg);
		}

		bool _errorRemap = true;

		void Reference(string arg)
		{
			_ctx.BuildErrorLogger.LogDebug("@Reference = " + arg);
			_ctx.ReferencesMetaAdditional.Add(arg);
		}

		void GenerateBanner(string arg)
		{
			WriteWarning("GenerateBanner is deprecated. Remove that.");
			//ctx.GenerateBanner = ToBool(arg);
		}

		void WriteWarning(string message, params object[] args)
		{
			_ctx.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, _ctx.OriginalFileName, GetLineNumberByIndex(_code, _currentBlockIndex), 0, GetLineNumberByIndex(_code, _currentBlockFinishIndex), 0, message.Arg(args), null, "MetaCreator.dll"));
		}

		void WriteError(string message, params object[] args)
		{
			_ctx.BuildErrorLogger.LogErrorEvent(new BuildErrorEventArgs(null, null, _ctx.OriginalFileName, GetLineNumberByIndex(_code, _currentBlockIndex), 0, GetLineNumberByIndex(_code, _currentBlockFinishIndex), 0, message.Arg(args), null, "MetaCreator.dll"));
		}

		void Using(string arg)
		{
			_namespaceImportsMetaAdditional.Add(arg);
		}

		readonly List<string> _namespaceImportsMetaAdditional = new List<string>();

	}
}
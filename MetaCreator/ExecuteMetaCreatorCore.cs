using System;
using System.Diagnostics;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using MetaCreator.AppDomainIsolation;
using MetaCreator.Evaluation;
using MetaCreator.Utils;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

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
			IntermediateOutputPathRelative = intermediateOutputPath;
			ProjDir = projDir;
			BuildErrorLogger = buildErrorLogger;
		}

		#region INPUTS

		public ITaskItem[] Sources { get; set; }
		public ITaskItem[] References { get; set; }
		public string IntermediateOutputPathFull { get; set; }
		public string IntermediateOutputPathRelative { get; set; }
		public string ProjDir { get; set; }
		public string TargetsVersion { get; set; }
		public string TargetFrameworkVersion { get; set; }
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

		public void Initialize()
		{
			if (string.IsNullOrEmpty(ProjDir))
			{
				throw new Exception("ProjDir not defined");
			}
			
			if (string.IsNullOrEmpty(TargetFrameworkVersion))
			{
				throw new Exception("TargetFrameworkVersion not defined");
			}

			if (string.IsNullOrEmpty(TargetsVersion))
			{
				throw new Exception("TargetsVersion not defined");
			}

			if (string.IsNullOrEmpty(IntermediateOutputPathRelative))
			{
				throw new Exception("IntermediateOutputPathRelative not defined");
			}

			if (Sources == null)
			{
				throw new Exception("Sources is not specified");
			}

			if (ProjDir != Path.GetFullPath(ProjDir))
			{
				throw new Exception("ProjDir is not full path");
			}

			var dllVersion = typeof(ExecuteMetaCreatorCore).Assembly.GetName().Version.ToString();

			if (!dllVersion.StartsWith(TargetsVersion))
			{
				throw new Exception("MetaCreator.targets version is {0}. But MetaCreator.dll version is {1}".Arg(TargetsVersion, dllVersion));
			}

			IntermediateOutputPathFull = Path.Combine(ProjDir, IntermediateOutputPathRelative);

			if (References == null)
			{
				References = new ITaskItem[0];
			}

			_appDomFactory = AnotherAppDomFactory.AppDomainLiveScope();
		}

		internal string ProcessFile(string code, ProcessFileCtx ctx)
		{
			return EvaluateMetacode(code, ctx);
		}

		private string EvaluateMetacode(string code, ProcessFileCtx ctx)
		{
			var codeBuilder = new Code1Builder();
			var metacode = codeBuilder.Build(code, ctx);
			if (ctx.NumberOfMetaBlocksProcessed == 0)
			{
				return code;
			}
			BuildErrorLogger.LogOutputMessage(ctx.OriginalFileName + " - {0} macros processed. Evaluating...".Arg(ctx.NumberOfMetaBlocksProcessed));

			if (string.IsNullOrEmpty(ctx.CSharpVersion))
			{
				ctx.CSharpVersion = ctx.TargetFrameworkVersion;
				ctx.BuildErrorLogger.LogDebug("Automatic CSharpVersion using current target framework version = " + ctx.CSharpVersion);
			}

			var evaluationParameters = new AnotherAppDomInputData
			{
				Metacode = metacode,
				CSharpVersion = ctx.CSharpVersion,
				References = ConfigureReferences(ctx),
			};

			var evaluationResult = _appDomFactory.AnotherAppDomMarshal.Evaluate(evaluationParameters);
			_appDomFactory.MarkDirectoryPathToRemoveAfterUnloadDomain(evaluationResult.CompileTempPath);
			var codeAnalyzer = new Code4Analyze();
			codeAnalyzer.Analyze(evaluationResult, ctx);
			return (string)evaluationResult.ReturnedValue;
		}

		static string[] ConfigureReferences(ProcessFileCtx ctx)
		{
			var alreadyReferencedNames = new List<string>(16) { "mscorlib" }; // always ignore mscorlib

			var name = new Func<string, string>(Path.GetFileNameWithoutExtension);
			var already = new Func<string, bool>(x => alreadyReferencedNames.Contains(name(x), StringComparer.InvariantCultureIgnoreCase));
			var alreadyAdd = new Action<string>(x =>
			{
				ctx.BuildErrorLogger.LogDebug("Reference: " + x);
				alreadyReferencedNames.Add(name(x));
			});

			var referencesTotal = new List<string>(16);

			// first priority - explicit references
			foreach (var reference_ in ctx.ReferencesMetaAdditional)
			{
				var reference = reference_;
				if (reference.Contains("..") || reference.Contains(Path.DirectorySeparatorChar))
				{
					reference = Path.Combine(ctx.ProjDir, reference);
				}

				if (!already(reference))
				{
					alreadyAdd(reference);
					referencesTotal.Add(reference);
				}
			}

			// reference some required assemblies
			foreach (var reference in new[] { "System.dll", "System.Core.dll" })
			{
				if (!already(reference))
				{
					alreadyAdd(reference);
					referencesTotal.Add(reference);
				}
			}

			if(string.IsNullOrEmpty(ctx.CSharpVersion))
			{
				throw new Exception("CSharpVersion is unknown");
			}

			// required for dynamics in 4.0
			if (ctx.CSharpVersion.Equals("v4.0", StringComparison.InvariantCultureIgnoreCase))
			{
				foreach (var reference in new[] {"Microsoft.CSharp.dll"})
				{
					if (!already(reference))
					{
						alreadyAdd(reference);
						referencesTotal.Add(reference);
					}
				}
			}

			if (ctx.ReferencesOriginal == null)
			{
				throw new Exception("Original proj references if unknown");
			}

			// all other references from target project
			foreach (var reference in ctx.ReferencesOriginal)
			{
				if (!already(reference))
				{
					alreadyAdd(reference);	
					referencesTotal.Add(reference);
				}
			}

			return referencesTotal.ToArray();
		}

		AnotherAppDomFactory _appDomFactory;
		public bool Execute()
		{
			// Debugger.Launch();
			Initialize();

			using (_appDomFactory)
			{
				int totalMacrosProcessed = 0;
				var totalTime = Stopwatch.StartNew();
				foreach (var sourceFile in Sources)
				{
					var fileName = sourceFile.ItemSpec;
					var isExternalLink = fileName.StartsWith("..");
					var ext = Path.GetExtension(fileName);
					string replacementFile;
					if (isExternalLink)
					{
						replacementFile = Path.Combine("_Linked", Path.GetFileNameWithoutExtension(fileName) + ".g" + Path.GetExtension(fileName));
					}
					else
					{
						replacementFile = fileName.Substring(0, fileName.Length - ext.Length) + ".g" + ext;
					}
					var replacementFileRelativePath = Path.Combine(IntermediateOutputPathRelative, replacementFile);
					var replacementFileAbsolutePath = Path.GetFullPath(replacementFileRelativePath);

					var ctx = GetCtx(this, fileName, replacementFileRelativePath, replacementFileAbsolutePath);

					var code = File.ReadAllText(fileName);

					string processedCode;
					try
					{
						processedCode = ProcessFile(code, ctx);
					}
					catch (FailBuildingException)
					{
						// Already logged to msbuild. Just fail the building.
						return false;
					}

					if (ctx.NumberOfMetaBlocksProcessed > 0)
					{
						BuildErrorLogger.LogDebug("fileName = " + fileName);
						//BuildErrorLogger.LogDebug("replacementFile = " + replacementFile);
						//BuildErrorLogger.LogDebug("IntermediateOutputPathRelative = " + IntermediateOutputPathRelative);
						//BuildErrorLogger.LogDebug("replacementFileRelativePath = " + replacementFileRelativePath);
						//BuildErrorLogger.LogDebug("replacementFileAbsolutePath = " + replacementFileAbsolutePath);

						totalMacrosProcessed += ctx.NumberOfMetaBlocksProcessed;

						Directory.CreateDirectory(Path.GetDirectoryName(replacementFileAbsolutePath));

						var theSameContent = File.Exists(replacementFileAbsolutePath) &&
													File.ReadAllText(replacementFileAbsolutePath) == processedCode;

						if (!theSameContent)
						{
							//if (File.Exists(replacementFileName))
							//{
							//   File.SetAttributes(replacementFileName, File.GetAttributes(replacementFileName) & ~FileAttributes.ReadOnly);
							//}
							File.WriteAllText(replacementFileAbsolutePath, processedCode, new UTF8Encoding(true, true));
							//File.SetAttributes(replacementFileName, File.GetAttributes(replacementFileName) | FileAttributes.ReadOnly);
						}

						BuildErrorLogger.LogOutputMessage(fileName + " - " + ctx.NumberOfMetaBlocksProcessed + " macros processed to => " + replacementFileRelativePath + ". File " + (theSameContent ? "is up to date." : "updated"));

						_removeFiles.Add(sourceFile);
						_addFiles.Add(new TaskItem(replacementFileAbsolutePath));
					}
				}

				if (totalMacrosProcessed == 0)
				{
					BuildErrorLogger.LogOutputMessage("No macros found. Nothing changed. Duration = " + totalTime.ElapsedMilliseconds + "ms");
				}
				else
				{
					BuildErrorLogger.LogOutputMessage("Duration = " + totalTime.ElapsedMilliseconds + "ms");
				}

				return true;
			}
		}

		static public ProcessFileCtx GetCtx(ExecuteMetaCreatorCore core, string fileName, string replacementFileRelativePath, string replacementFileAbsolutePath)
		{
			return new ProcessFileCtx
			{
				//AppDomFactory = appDomFactory,
				BuildErrorLogger = core.BuildErrorLogger,
				OriginalFileName = fileName,
				ReplacementRelativePath = replacementFileRelativePath,
				ReplacementAbsolutePath = replacementFileAbsolutePath,
				IntermediateOutputPathRelative = core.IntermediateOutputPathRelative,
				IntermediateOutputPathFull = core.IntermediateOutputPathFull,
				ProjDir = core.ProjDir,
				TargetFrameworkVersion = core.TargetFrameworkVersion,
				ReferencesOriginal = core.References.Select(x => x.ItemSpec).ToArray(),
			};
		}
	}
}
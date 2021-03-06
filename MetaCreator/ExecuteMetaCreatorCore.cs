﻿using System;
using System.Diagnostics;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MetaCreator.AppDomainIsolation;
using MetaCreator.Evaluation;
using MetaCreator.Utils;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace MetaCreator
{
	internal class ExecuteMetaCreatorCore
	{
		internal ExecuteMetaCreatorCore()
		{
		}

/*
		public ExecuteMetaCreatorCore(ITaskItem[] sources, ITaskItem[] references, string intermediateOutputPath, string projDir, BuildErrorLoggerDelayedProxy buildErrorLogger)
			:this()
		{
			Sources = sources;
			References = references;
			IntermediateOutputPathRelative = intermediateOutputPath;
			ProjDir = projDir;
			BuildErrorLogger = buildErrorLogger;
		}
*/

		#region Meta Levels

		public MSBuild MSBuild { get; set; }

		private string _mLatestResult;
		private byte? _mLatestLevel;
		private readonly string[] _mResults = new string[256];

		public string CurrentProject;

		private string MBuild(ProcessFileCtx ctx, byte level)
		{
			//Debugger.Launch();
			if (level == 255)
			{
				throw new Exception("Level 255 is not possible to request");
			}
			if (level == MLevel)
			{
				throw new Exception("It is not possible to request the same level");
			}
			if (!string.IsNullOrEmpty(_mResults[level]))
			{
				return _mResults[level];
			}
			ctx.BuildErrorLogger.LogOutputMessage(string.Format("Inner Build Start. Current MLevel={0}. Requested level={1}", MLevel, level));
			//var dir = Path.Combine(IntermediateOutputPathFull, "MLevel" + MLevel) + Path.DirectorySeparatorChar;
			var pref = IntermediateOutputPathFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			// this is required because otherwise IntermediateOutputPathFull will be obj\Debug_MLevel_1_MLevel2...
			var i = pref.IndexOf('@');
			if (i > 0)
			{
				pref = pref.Substring(0, i);
			}
			var dir = Path.Combine(pref + "@" + level);
			var outp = dir + "_bin" + Path.DirectorySeparatorChar;
			var intp = dir + "_obj" + Path.DirectorySeparatorChar;
			Directory.CreateDirectory(outp);
			Directory.CreateDirectory(intp);
			MSBuild.Properties = new[]
			{
				"OutputPath=" + outp,
				"IntermediateOutputPath=" + intp,
				"Configuration=Debug",
				"Optimize=False",
				"MLevel=" + level,
				"AssemblyName=" +  CurrentProject + "_MLevel" + level,
			};
			var result = MSBuild.Execute();
			if (!result)
			{
				throw new FailBuildingException(null);
			}
			ctx.BuildErrorLogger.LogDebug("At level " + MLevel + ", Inner Build Success: ");
			foreach (var targetOutput in MSBuild.TargetOutputs)
			{
				ctx.BuildErrorLogger.LogDebug("Output: " + targetOutput.ItemSpec);
			}

			var resexe = MSBuild.TargetOutputs.FirstOrDefault(x => x.ItemSpec.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase) && x.ItemSpec.Contains("@" + level + "_"));
			var resdll = MSBuild.TargetOutputs.FirstOrDefault(x => x.ItemSpec.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) && x.ItemSpec.Contains("@" + level + "_"));
			string res = null;
			if (resexe != null)
			{
				res = resexe.ItemSpec;
			}
			if (resdll != null)
			{
				res = resdll.ItemSpec;
			}
			_mResults[level] = res;
			if (!_mLatestLevel.HasValue || _mLatestLevel.Value >= level)
			{
				_mLatestResult = res;
				_mLatestLevel = level;
			}
			ctx.BuildErrorLogger.LogDebug("_mLatestLevel: " + level);
			ctx.BuildErrorLogger.LogDebug("_mLatestResult: " + _mLatestResult);
			ctx.BuildErrorLogger.LogDebug("return: " + res);
			return res;
		}

		#endregion

		#region INPUTS

		public ITaskItem[] Sources { get; set; }
		public ITaskItem[] References { get; set; }
		public string IntermediateOutputPathFull { get; set; }
		public string IntermediateOutputPathRelative { get; set; }
		public string ProjDir { get; set; }
		public string TargetsVersion { get; set; }
		public string TargetFrameworkVersion { get; set; }

		private BuildErrorLoggerDelayedProxy _buildErrorLogger;
		public IBuildErrorLogger BuildErrorLogger
		{
			get { return _buildErrorLogger; }
			set { _buildErrorLogger = new BuildErrorLoggerDelayedProxy(value); }
		}

		public byte MLevel { get; set; }

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

		public BuildErrorLoggerConfig BuildErrorLoggerConfig { get; set; }


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

			if (!dllVersion.StartsWith(TargetsVersion + "."))
			{
				throw new Exception("MetaCreator.targets version is {0} But MetaCreator.dll version is {1}".Arg(TargetsVersion, dllVersion));
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
			BuildErrorLoggerConfig.Debug = ctx.EnableDebugLogging;
			BuildErrorLogger.LogOutputMessage(ctx.OriginalRelativeFileName + " - {0} macros processed. Evaluating...".Arg(ctx.NumberOfMetaBlocksProcessed));

			if (string.IsNullOrEmpty(ctx.CSharpVersion))
			{
				ctx.CSharpVersion = CsVersionFromFramework(ctx.TargetFrameworkVersion);
				ctx.BuildErrorLogger.LogDebug("Automatic CSharpVersion using current target framework version = " + ctx.CSharpVersion);
			}

			var evaluationParameters = new AnotherAppDomInputData
			{
				Metacode = metacode,
				CSharpVersion = ctx.CSharpVersion,
				References = ConfigureReferences(ctx),
				ImportsFromOriginalFile = ctx.ImportsFromOriginalFile,
				OuterNamespaceFromOriginalFile = ctx.OuterNamespaceFromOriginalFile,
				Timeout = ctx.Timeout,
				MetaAssemblyName = ctx.MetaAssemblyName,
			};
/*
			if (ctx.MLevel < 255 && string.IsNullOrEmpty(ctx.MetaAssemblyName))
			{
				var asmName = GetMLevelMetaAssemblyName(CurrentProject, ctx.ReplacementFileName, MLevel);
				ctx.BuildErrorLogger.LogDebug("Generated MetaAssemblyName: " + asmName);
				evaluationParameters.MetaAssemblyName = asmName;
			}
*/

			ctx.BuildErrorLogger.LogDebug("OuterSpace: " + ctx.OuterNamespaceFromOriginalFile);
			ctx.BuildErrorLogger.LogDebug("Imports: " + string.Join(", ", ctx.ImportsFromOriginalFile));

			var evaluationResult = _appDomFactory.AnotherAppDomMarshal.Evaluate(evaluationParameters);
			_appDomFactory.MarkDirectoryPathToRemoveAfterUnloadDomain(evaluationResult.CompileTempPath);
			var codeAnalyzer = new Code4Analyze();
			codeAnalyzer.Analyze(evaluationResult, ctx);
			return (string)evaluationResult.ReturnedValue;
		}

/*
		static string GetMLevelMetaAssemblyName(string assembly, string file, byte mLevel)
		{
			return "Meta." + assembly.Trim('.') + '.' + file.Trim('.') + '.' + mLevel;
		}
*/

		string CsVersionFromFramework(string targetFrameworkVersion)
		{
			switch (targetFrameworkVersion)
			{
				case "v4.5":
					return "v4.0";
				default:
					return targetFrameworkVersion;
			}
		}

		static string[] ConfigureReferences(ProcessFileCtx ctx)
		{
			// list for ignore already referenced assemblies
			var alreadyReferencedNames = new List<string>(16) { "mscorlib" }; // always ignore mscorlib

			var name = new Func<string, string>(x=>
			{
				//ctx.BuildErrorLogger.LogDebug("GetFileNameWithoutExtension: " + x + " " + string.Join(", ", x.Select(c => ((int)c).ToString() + " - " + c).ToArray()));
				return Path.GetFileNameWithoutExtension(x);
			});
			var already = new Func<string, bool>(x => alreadyReferencedNames.Contains(name(x), StringComparer.InvariantCultureIgnoreCase));
			var alreadyAdd = new Action<string>(x =>
			{
				ctx.BuildErrorLogger.LogDebug("Reference: " + x);
				alreadyReferencedNames.Add(name(x));
			});

			var referencesTotal = new List<string>(16);

			// metacreator
			var mc = typeof (ExecuteMetaCreatorCore).Assembly.Location;
			if (!already(mc))
			{
				alreadyAdd(mc);
				referencesTotal.Add(mc);
			}

			// first priority - explicit references
			foreach (var reference_ in ctx.ReferencesMetaAdditional)
			{
				var reference = reference_;
				if (reference.Contains("..") || reference.Contains(Path.DirectorySeparatorChar))
				{
					reference = Path.Combine(ctx.ProjDir, reference);
				}

				//ctx.BuildErrorLogger.LogDebug("CheckAlrready: " + reference);

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
			ctx.BuildErrorLogger.LogDebug("OriginalProjReferences:");
			foreach (var reference in ctx.ReferencesOriginal)
			{
				ctx.BuildErrorLogger.LogDebug("OriginalProjReference: " + reference);
				if (!already(reference))
				{
					alreadyAdd(reference);	
					referencesTotal.Add(reference);
				}
			}
			ctx.BuildErrorLogger.LogDebug("OriginalProjReference: DONE");

			return referencesTotal.ToArray();
		}

		AnotherAppDomFactory _appDomFactory;
		public string IsDevMode;

		public bool Execute()
		{
			//Debugger.Launch();
			Initialize();

			if (MLevel == 0)
			{
				// ignore all metablocks
				return true;
			}

			using (_appDomFactory)
			{
				int totalMacrosProcessed = 0;
				var totalTime = Stopwatch.StartNew();
				foreach (var sourceFile in Sources.Where(x=>x.ItemSpec.EndsWith(".cs",StringComparison.InvariantCultureIgnoreCase) || x.ItemSpec.EndsWith(".tt",StringComparison.InvariantCultureIgnoreCase)))
				{
					//BuildErrorLogger.LogDebug("Spec = " + sourceFile.ItemSpec);
					//BuildErrorLogger.LogDebug("\tMetadataNames = " + string.Join(", ", sourceFile.MetadataNames.Cast<string>().Select(x=>x+": "+ sourceFile.GetMetadata(x)).ToArray()));

					if (!string.IsNullOrEmpty(sourceFile.GetMetadata("Generator")))
					{
						BuildErrorLogger.LogDebug("This file has custom tool or generator. Ignore.");
						continue;
					}

					var fileName = sourceFile.ItemSpec;

					if (!File.Exists(fileName))
					{
						continue;
					}

					var ctx = GetCtx(this, fileName/*, replacementFileRelativePath, replacementFileAbsolutePath*/);

					var code = File.ReadAllText(fileName);

					string processedCode;
					try
					{
						_buildErrorLogger.DelayedStart();
						// ctx.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, ctx.OriginalRelativeFileName, 0, 0, 0, 0, "!!!!!", null, "MetaCreator.dll"));
						processedCode = ProcessFile(code, ctx);
					}
					catch (FailBuildingException ex)
					{
						switch (ex.IgnoreThisFile)
						{
							case EarlyPassMode.None:
								break;
							case EarlyPassMode.Exclude:
								_removeFiles.Add(sourceFile);
								continue;
							case EarlyPassMode.NoMeta:
								continue;
							default:
								throw new ArgumentOutOfRangeException();
						}
						// return false;
						// only one automatic level raise
						if (ex.Result != null && ex.Result.Errors != null &&
						    (ex.Result.Errors.Any(x => x.ErrorNumber == "CS0103")
						     || ex.Result.Errors.Any(x => x.ErrorNumber == "CS0246")
							    ) && MLevel == 255)
						{
							ctx.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, ctx.OriginalRelativeFileName, 0, 0, 0, 0, "MetaCreator: Auto raise meta level", null, "MetaCreator.dll"));

							var res = MBuild(ctx, 0);
							// add reference
							if (res != null)
							{
								ctx.ReferencesMetaAdditional.Add(res);
								try
								{
									processedCode = ProcessFile(code, ctx);
									// Successed
									_buildErrorLogger.DelayedCommitAsWarnings();
									BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, ctx.OriginalRelativeFileName, 0, 0, 0, 0, "MetaCreator: Auto raise succeed. Please, add /*@ requiresLevel min */ to speedup build and get rid of warnings", null, "MetaCreator.dll"));
								}
								catch (FailBuildingException)
								{
									BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, ctx.OriginalRelativeFileName, 0, 0, 0, 0, "MetaCreator: Auto raise failed. Consider first error!", null, "MetaCreator.dll"));
									return false;
								}
							}
							else
							{
								return false;
							}
						}
						else
						{
							return false;
						}
					}
					finally
					{
						_buildErrorLogger.DelayedCommit();
					}

					if (ctx.NumberOfMetaBlocksProcessed > 0)
					{
						totalMacrosProcessed += ctx.NumberOfMetaBlocksProcessed;
						var theSameContent = AddFileToCompile(processedCode, ctx);
						_removeFiles.Add(sourceFile);

						_additionalFileNameBase = ctx.OriginalRelativeFileName;
						_additionalFileNameIndex = 0;

						var addedFilesLog = AddedFilesLog(ctx, theSameContent);

						foreach (var newFile in ctx.NewFiles)
						{
							var ctxAdd = new SimpleNewFileCtx
							{
								OriginalRelativeFileName = newFile.FileName,
								ReplacementFileName = newFile.FileName,
								FileInProject = newFile.FileInProject,
							};
							var sc = AddFileToCompile(newFile.FileBody, ctxAdd);
							addedFilesLog += "\r\n" + AddedFilesLog(ctxAdd, sc);
						}

						BuildErrorLogger.LogOutputMessage(fileName + " - " + ctx.NumberOfMetaBlocksProcessed + " macros processed to => \r\n" + addedFilesLog);
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

		static string AddedFilesLog(IAddNewFileCtx ctx, bool theSameContent)
		{
			return (theSameContent ? "[Not changed]\t" : "[Updated]\t") + " " + ctx.ReplacementRelativePath;
		}

		/// <returns>
		/// Return 'TheSameContent'
		/// </returns>
		private bool AddFileToCompile(string body, IAddNewFileCtx ctx)
		{
			PrepareReplacementFileNames(ctx);

			// BuildErrorLogger.LogDebug("fileName = " + fileName);
			// BuildErrorLogger.LogDebug("ReplacementFileName = " + (string.IsNullOrEmpty(ctx.ReplacementFileName) ? "<NotSpecified>" : ctx.ReplacementFileName));
			// BuildErrorLogger.LogDebug("FileInProject = " + ctx.FileInProject);
			// BuildErrorLogger.LogDebug("IntermediateOutputPathRelative = " + IntermediateOutputPathRelative);
			// BuildErrorLogger.LogDebug("replacementFileRelativePath = " + ctx.ReplacementRelativePath);
			// BuildErrorLogger.LogDebug("replacementFileAbsolutePath = " + ctx.ReplacementAbsolutePath);

			Directory.CreateDirectory(Path.GetDirectoryName(ctx.ReplacementAbsolutePath));

			var theSameContent = File.Exists(ctx.ReplacementAbsolutePath) &&
										File.ReadAllText(ctx.ReplacementAbsolutePath) == body;

			if (!theSameContent)
			{
				var fn = ctx.ReplacementRelativePath;
				bool restoreReadonly = false;
				if (ctx.FileInProject && File.Exists(fn) && ((File.GetAttributes(fn) & FileAttributes.ReadOnly) != 0))
				{
					File.SetAttributes(fn, File.GetAttributes(fn) & ~FileAttributes.ReadOnly);
					restoreReadonly = true;
				}
				File.WriteAllText(fn, body, new UTF8Encoding(true, true));
				if (restoreReadonly)
				{
					File.SetAttributes(fn, File.GetAttributes(fn) | FileAttributes.ReadOnly);
				}
			}

			// только если файл помечен как "в проекте" и он действительно есть в списке исходников - ничего не делать
			// иначе - добавить на лету при компиляции
			if (!(ctx.FileInProject && Sources.Any(x => x.ItemSpec.Equals(ctx.ReplacementRelativePath))))
			{
				_addFiles.Add(new TaskItem(ctx.ReplacementRelativePath));
			}

			return theSameContent;
		}

		int _additionalFileNameIndex;
		string _additionalFileNameBase;

		void PrepareReplacementFileNames(IAddNewFileCtx ctx)
		{
			var originalRelativeName = ctx.OriginalRelativeFileName ?? NextAdditionalFileName();

			var isExternalLink = originalRelativeName.StartsWith("..");
			var ext = ctx.ReplacementExtension ?? ".cs";

			string replacementFileRelativeName = ctx.ReplacementFileName;
			// if not defined by dirrective - generate from original relative file name
			if (string.IsNullOrEmpty(replacementFileRelativeName))
			{
				replacementFileRelativeName = Path.GetFileNameWithoutExtension(originalRelativeName) + ".g" + ext;
			}

			// if defined by dirrective, or generated, but not linked - preppend a relative project path
			var dir = Path.GetDirectoryName(originalRelativeName);
			if (!isExternalLink && !string.IsNullOrEmpty(dir))
			{
				replacementFileRelativeName = Path.Combine(dir, replacementFileRelativeName);
			}

			// if linked - prepend with special folder
			if (isExternalLink)
			{
				replacementFileRelativeName = Path.Combine("_Linked", replacementFileRelativeName);
			}

			// prepend relative intermidieate path, like obj\debug\ or ..\..\binaries\commonobjs\
			if (!ctx.FileInProject)
			{
				ctx.ReplacementRelativePath = Path.Combine(IntermediateOutputPathRelative, replacementFileRelativeName);
			}
			else
			{
				ctx.ReplacementRelativePath = replacementFileRelativeName;
			}

			ctx.ReplacementAbsolutePath = Path.GetFullPath(ctx.ReplacementRelativePath);

		}

		private string NextAdditionalFileName()
		{
			return Path.Combine(
					Path.GetDirectoryName(_additionalFileNameBase),
					Path.GetFileNameWithoutExtension(_additionalFileNameBase) + "_add_" + (++_additionalFileNameIndex) + Path.GetExtension(_additionalFileNameBase));
		}

		public ProcessFileCtx GetCtx(ExecuteMetaCreatorCore core, string fileName/*, string replacementFileRelativePath, string replacementFileAbsolutePath*/)
		{
			var ctx = new ProcessFileCtx
			{
				//AppDomFactory = appDomFactory,
				BuildErrorLogger = core.BuildErrorLogger,
				OriginalRelativeFileName = fileName,
//				ReplacementRelativePath = replacementFileRelativePath,
//				ReplacementAbsolutePath = replacementFileAbsolutePath,
				IntermediateOutputPathRelative = core.IntermediateOutputPathRelative,
				IntermediateOutputPathFull = core.IntermediateOutputPathFull,
				ProjDir = core.ProjDir,
				CurrentProject = core.CurrentProject,
				TargetFrameworkVersion = core.TargetFrameworkVersion,
				ReferencesOriginal = core.References.Select(x => x.ItemSpec).ToArray(),
				MLevel = MLevel,
			};
			ctx.GetIntermMetaLevel = level => MBuild(ctx, level);
			return ctx;
		}
	}
}
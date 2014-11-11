using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System.Linq;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace MetaCreator
{
	/// <summary>
	/// MS Build Task
	/// </summary>
	[LoadInSeparateAppDomain]
	public class ExecuteMetaCreator : AppDomainIsolatedTask
	{
		/// <summary>
		/// Source code files with meta blocks
		/// </summary>
		[Required]
		public ITaskItem[] Sources { get; set; }

		/// <summary>
		/// Notify that task is running from MetaCreator.sln for additional tracing
		/// </summary>
		public string IsDevMode { get; set; }

		/// <summary>
		/// </summary>
		[Required]
		public ITaskItem[] References { get; set; }

		/// <summary>
		/// </summary>
		[Required]
		public string IntermediateOutputPath { get; set; }

		/// <summary>
		/// </summary>
		[Required]
		public string ProjDir { get; set; }

		public string MLevel { get; set; }

		public ITaskItem[] ProjectPaths { get; set; }

		[Required]
		public string TargetsVersion { get; set; }

		[Required]
		public string TargetFrameworkVersion { get; set; }

		[Output]
		public ITaskItem[] AddFiles { get { return _core.AddFiles.ToArray(); } }

		[Output]
		public ITaskItem[] RemoveFiles { get { return _core.RemoveFiles.ToArray(); } }

		readonly ExecuteMetaCreatorCore _core = new ExecuteMetaCreatorCore();

		public override bool Execute()
		{
			if (string.IsNullOrWhiteSpace(MLevel))
			{
				MLevel = "255";
			}
			var logConfig = new BuildErrorLoggerConfig();

			_core.Sources = Sources;
			_core.IsDevMode = IsDevMode;
			_core.References = References;
			_core.IntermediateOutputPathRelative = IntermediateOutputPath;
			_core.ProjDir = ProjDir;
			_core.TargetsVersion = TargetsVersion;
			_core.TargetFrameworkVersion = TargetFrameworkVersion;
			_core.ProjDir = ProjDir;
			_core.MLevel = byte.Parse(MLevel);
			_core.BuildErrorLoggerConfig = logConfig;
			_core.BuildErrorLogger = new BuildErrorLogger(BuildEngine, Log, logConfig);

			_core.MSBuild = new MSBuild
			{
				BuildEngine = BuildEngine,
				HostObject = HostObject,
				Projects = ProjectPaths,
				//!Properties = this.pro
				//!TargetOutputs
				Targets = new []{"Build"},
				UseResultsCache = true,
			};
			_core.CurrentProject = Path.GetFileNameWithoutExtension(_core.MSBuild.Projects.First().ItemSpec);

/*

						var buildEngineType = BuildEngine.GetType();
						_core.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, "", 0, 0, 0, 0, "Fields:"+buildEngineType+Guid.NewGuid(), null, "MetaCreator.dll"));
						foreach (var field in buildEngineType.GetFields(BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public))
						{
							_core.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, "", 0, 0, 0, 0, field.Name, null, "MetaCreator.dll"));
						}


						var projectInstance = BuildEngine.GetProjectInstance();
									var items = projectInstance.Items.Where(x => string.Equals(x.ItemType, key, StringComparison.InvariantCultureIgnoreCase)).ToList();
									if (items.Count > 0)
									{
										// return items.Select(x => x.EvaluatedInclude);
									}

						var properties = projectInstance.Properties;
						foreach (var property in properties)
						{
							_core.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, "", 0, 0, 0, 0, property.Name + " " + property.EvaluatedValue, null, "MetaCreator.dll"));
						}

			*/
			return _core.Execute();
		}

	}

	public static class BuildEngineExtensions
	{
		const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

		public static IEnumerable GetEnvironmentVariable(this IBuildEngine buildEngine, string key, bool throwIfNotFound)
		{
			var projectInstance = GetProjectInstance(buildEngine);

			var items = projectInstance.Items
				.Where(x => string.Equals(x.ItemType, key, StringComparison.InvariantCultureIgnoreCase)).ToList();
			if (items.Count > 0)
			{
				return items.Select(x => x.EvaluatedInclude);
			}


			var properties = projectInstance.Properties
				.Where(x => string.Equals(x.Name, key, StringComparison.InvariantCultureIgnoreCase)).ToList();
			if (properties.Count > 0)
			{
				return properties.Select(x => x.EvaluatedValue);
			}

			if (throwIfNotFound)
			{
				throw new Exception(string.Format("Could not extract from '{0}' environmental variables.", key));
			}

			return Enumerable.Empty<object>();
		}

		public static ProjectInstance GetProjectInstance(this IBuildEngine buildEngine)
		{
			var buildEngineType = buildEngine.GetType();

			var targetBuilderCallbackField = buildEngineType.GetField("targetBuilderCallback", bindingFlags);
			if (targetBuilderCallbackField == null)
			{
				throw new Exception("Could not extract targetBuilderCallback from " + buildEngineType.FullName);
			}
			var targetBuilderCallback = targetBuilderCallbackField.GetValue(buildEngine);
			var targetCallbackType = targetBuilderCallback.GetType();
			var projectInstanceField = targetCallbackType.GetField("projectInstance", bindingFlags);
			if (projectInstanceField == null)
			{
				throw new Exception("Could not extract projectInstance from " + targetCallbackType.FullName);
			}
			return (ProjectInstance)projectInstanceField.GetValue(targetBuilderCallback);
		}
	}
}
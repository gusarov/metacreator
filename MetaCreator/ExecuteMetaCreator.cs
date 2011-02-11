using System;
using System.Diagnostics;
using Microsoft.Build.Framework;
using System.Linq;
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

		[Output]
		public ITaskItem[] AddFiles { get { return _core.AddFiles.ToArray(); } }

		[Output]
		public ITaskItem[] RemoveFiles { get { return _core.RemoveFiles.ToArray(); } }

		static readonly ExecuteMetaCreatorCore _core = new ExecuteMetaCreatorCore();

		public override bool Execute()
		{
			// Debugger.Launch();

			_core.Sources = Sources;
			_core.References = References;
			_core.IntermediateOutputPath = IntermediateOutputPath;
			_core.ProjDir = ProjDir;
			_core.BuildErrorLogger = new BuildErrorLogger(BuildEngine, Log);
			return _core.Execute();
		}

	}
}
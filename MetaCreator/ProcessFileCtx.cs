using System;
using System.IO;
using System.Linq;
using System.Text;

using MetaCreator.Evaluation;
using MetaCreator.Utils;
using System.Collections.Generic;

namespace MetaCreator
{
	sealed class ProcessFileCtx
	{
		internal Code1Builder.BlockParser BlockParser;

		public string OriginalRelativeFileName;
		public string IntermediateOutputPathRelative;
		public string IntermediateOutputPathFull;
		public string ProjDir;

		public bool MacrosFailed;

		public int NumberOfMetaBlocksProcessed;
		public string ReplacementAbsolutePath;
		public string ReplacementRelativePath;
		public string ReplacementFileName;

		/// <summary>
		/// Specify that generated file should be placed into proj dir, instead of intermediate output
		/// </summary>
		public bool FileInProject;
		public string ReplacementExtension = ".cs";

		// references from project that is currently builging
		public string[] ReferencesOriginal;
		// references from /*@ reference meta dirrectives */
		public List<string> ReferencesMetaAdditional = new List<string>();


		/// <summary>
		/// Get file name relative to intermediate path (Now it returns absolute path to original file name)
		/// </summary>
		/// <returns></returns>
		//[Obsolete("Now it returns absolute path to original file name")]
		public string GetOriginalFileNameRelativeToIntermediatePath()
		{
			//return IntermediateOutputPathRelative
//			var interm = ReplacementFileName.Substring(0, IntermediateOutputPath.Length);
//			DebugAssert.That(interm==IntermediateOutputPath);
//			var intermFileName = ReplacementFileName.Substring(IntermediateOutputPath.Length).Trim('/', '\\');

			// todo Now it returns absolute path to original file name
			return Path.Combine(ProjDir, OriginalRelativeFileName);
		}

		public IBuildErrorLogger BuildErrorLogger;
		public string CSharpVersion;
		public string TargetFrameworkVersion;

		TimeSpan _timeout = TimeSpan.FromSeconds(10);

		public TimeSpan Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}
	}
}
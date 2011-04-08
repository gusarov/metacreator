using System;
using System.IO;
using System.Linq;
using System.Text;
using MetaCreator.Utils;
using System.Collections.Generic;

namespace MetaCreator
{
	sealed class ProcessFileCtx
	{

		public string OriginalFileName;
		public string IntermediateOutputPathRelative;
		public string IntermediateOutputPathFull;
		public string ProjDir;

		public bool MacrosFailed;

		public int NumberOfMetaBlocksProcessed;
		public string ReplacementAbsolutePath;
		public string ReplacementRelativePath;
		public string ReplacementFileName;

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
			return Path.Combine(ProjDir, OriginalFileName);
		}

		public IBuildErrorLogger BuildErrorLogger;
		public string CSharpVersion;
		public string TargetFrameworkVersion;
		public Encoding OverrideEncoding;
	}
}
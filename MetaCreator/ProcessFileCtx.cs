using System;
using System.IO;
using System.Linq;
using System.Text;

using MetaCreator.Evaluation;
using MetaCreator.Utils;
using System.Collections.Generic;

namespace MetaCreator
{
	interface IAddNewFileCtx
	{
		bool FileInProject { get; set; }
		string ReplacementAbsolutePath { get; set; }
		string ReplacementRelativePath { get; set; }
		string OriginalRelativeFileName { get; set; }
		string ReplacementExtension { get; set; }
		string ReplacementFileName { get; set; }
	}

	sealed class SimpleNewFileCtx : IAddNewFileCtx
	{
		public string OriginalRelativeFileName { get; set; }

		public bool FileInProject { get; set; }
		public string ReplacementAbsolutePath { get; set; }
		public string ReplacementRelativePath { get; set; }
		public string ReplacementExtension { get; set; }
		public string ReplacementFileName { get; set; }
	}

	sealed class ProcessFileCtx : IAddNewFileCtx
	{
		internal Code1Builder.BlockParser BlockParser;

		public string OriginalRelativeFileName { get; set; }
		public string IntermediateOutputPathRelative;
		public string IntermediateOutputPathFull;
		public string ProjDir;

		public bool MacrosFailed;

		public int NumberOfMetaBlocksProcessed;
		public string ReplacementAbsolutePath { get; set; }
		public string ReplacementRelativePath { get; set; }
		public string ReplacementFileName { get; set; }

		/// <summary>
		/// Specify that generated file should be placed into proj dir, instead of intermediate output
		/// </summary>
		public bool FileInProject { get; set; }

		public string ReplacementExtension { get; set; }

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

		TimeSpan? _timeout;
		public readonly List<EvaluationResult.NewFile> NewFiles = new List<EvaluationResult.NewFile>();

		// tis property filled by Code1Builder
		public string[] ImportsFromOriginalFile;

		public TimeSpan? Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}

		public string OuterNamespaceFromOriginalFile;
	}
}
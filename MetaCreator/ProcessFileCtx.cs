using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using MetaCreator.AppDomainIsolation;
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
		//public bool EnabledStringInterpolation;
		//public bool ErrorRemap = true;
		public string ReplacementAbsolutePath;
		public string ReplacementRelativePath;
		public string ReplacementFileName;

		public string[] ReferencesOriginal;
		public List<string> ReferencesMetaAdditional = new List<string>();
		//public string[] NamespaceImportsOriginal;
		//public List<string> NamespaceImportsMetaAdditional = new List<string>();

		public string[] References
		{
			get
			{
				foreach (var refer in ReferencesMetaAdditional.ToArray())
				{
					if(!Path.IsPathRooted(refer))
					{
						ReferencesMetaAdditional.Remove(refer);
						ReferencesMetaAdditional.Add(Path.Combine(ProjDir, refer));
					}
				}
				return ReferencesOriginal.OrEmpty().Concat(ReferencesMetaAdditional.OrEmpty()).ToArray();
			}
		}

//		public string[] Namespaces
//		{
//			get
//			{
//				return NamespaceImportsOriginal.OrEmpty().Concat(NamespaceImportsMetaAdditional.OrEmpty()).ToArray();
//			}
//		}

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

//		string _fileOriginalContent;
//		public string FileOriginalContent
//		{
//			get { return _fileOriginalContent; }
//			set
//			{
//				_fileOriginalContent = value;
//				_currentMacrosEndLineInOriginalFile = 0;
//				_currentMacrosLineInOriginalFile = 0;
//			}
//		}
//
//		public string FileProcessedContent;
//
//		int _currentMacrosLineInOriginalFile;
//		public int CurrentMacrosLineInOriginalFile
//		{
//			get
//			{
//				return _currentMacrosLineInOriginalFile != 0
//					? _currentMacrosLineInOriginalFile
//					: _currentMacrosLineInOriginalFile = FileOriginalContent
//						.Substring(0, _currentMacrosIndex)
//						.Split('\r').Length;
//			}
//		}


//


		// public bool GenerateBanner = true;
//		public TimeSpan MetaCodeExecutionTimeOut = TimeSpan.FromSeconds(10);
		//public AnotherAppDomFactory AppDomFactory;

	}
}
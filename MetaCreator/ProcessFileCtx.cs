using System;
using System.IO;
using System.Linq;
using MetaCreator.Utils;
using System.Collections.Generic;

namespace MetaCreator
{
	sealed class ProcessFileCtx
	{
		public int NumberOfMacrosProcessed;
		public bool EnabledStringInterpolation;
		public bool ErrorRemap = true;
		public string OriginalFileName;
		public string ReplacementFileName;
		public string IntermediateOutputPath;
		public string ProjDir;

		public string[] ReferencesOriginal;
		public List<string> ReferencesMetaAdditional = new List<string>();
		public string[] NamespaceImportsOriginal;
		public List<string> NamespaceImportsMetaAdditional = new List<string>();

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

		public string[] Namespaces
		{
			get
			{
				return NamespaceImportsOriginal.OrEmpty().Concat(NamespaceImportsMetaAdditional.OrEmpty()).ToArray();
			}
		}

		/// <summary>
		/// Get file name relative to intermediate path
		/// </summary>
		/// <returns></returns>
		public string GetOriginalFileNameRelativeToIntermediatePath()
		{
//			var interm = ReplacementFileName.Substring(0, IntermediateOutputPath.Length);
//			DebugAssert.That(interm==IntermediateOutputPath);
//			var intermFileName = ReplacementFileName.Substring(IntermediateOutputPath.Length).Trim('/', '\\');
			return Path.Combine(ProjDir, OriginalFileName);
		}

		public IBuildErrorLogger BuildErrorLogger;

		string _fileOriginalContent;
		public string FileOriginalContent
		{
			get { return _fileOriginalContent; }
			set
			{
				_fileOriginalContent = value;
				_currentMacrosEndLineInOriginalFile = 0;
				_currentMacrosLineInOriginalFile = 0;
			}
		}

		public string FileProcessedContent;

		int _currentMacrosLineInOriginalFile;
		public int CurrentMacrosLineInOriginalFile
		{
			get
			{
				return _currentMacrosLineInOriginalFile != 0
					? _currentMacrosLineInOriginalFile
					: _currentMacrosLineInOriginalFile = FileOriginalContent
						.Substring(0, _currentMacrosIndex)
						.Split('\r').Length;
			}
		}

		int _currentMacrosEndLineInOriginalFile;
		public int CurrentMacrosEndLineInOriginalFile
		{
			get
			{
				return _currentMacrosEndLineInOriginalFile != 0
					? _currentMacrosEndLineInOriginalFile
					: _currentMacrosEndLineInOriginalFile = FileOriginalContent
						.Substring(0, _currentMacrosIndex+_currentMacrosLength)
						.Split('\r').Length;
			}
		}

		int _currentMacrosIndex;
		public int CurrentMacrosIndex
		{
			get { return _currentMacrosIndex; }
			set
			{
				_currentMacrosIndex = value;
				_currentMacrosLineInOriginalFile = 0;
				_currentMacrosEndLineInOriginalFile = 0;
			}
		}

		int _currentMacrosLength;
		public int CurrentMacrosLength
		{
			get { return _currentMacrosLength; }
			set
			{
				_currentMacrosLength = value;
				_currentMacrosEndLineInOriginalFile = 0;
			}
		}

		public bool GenerateBanner = true;
		public TimeSpan MetaCodeExecutionTimeOut = TimeSpan.FromSeconds(10);
	}
}
using System;
using System.Collections.Generic;

namespace MetaCreator.AppDomainIsolation
{
	[Serializable]
	public class AnotherAppDomInputData
	{
		public string Metacode;
		public string[] References;
		public string CSharpVersion;
		public TimeSpan? Timeout;
		public bool DoNotRun;
		public string[] ImportsFromOriginalFile;
		public string OuterNamespaceFromOriginalFile;
		public string MetaAssemblyName;
	}
}
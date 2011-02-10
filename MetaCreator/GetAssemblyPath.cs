using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MetaCreator
{
	[LoadInSeparateAppDomain]
	public class GetAssemblyPath : AppDomainIsolatedTask
	{
		private string _assemblyFullPath = string.Empty;
		private string _assemblyDir = string.Empty;

		[Output]
		public string AssemblyFullPath
		{
			get { return _assemblyFullPath; }
			set { _assemblyFullPath = value; }
		}

		[Output]
		public string AssemblyDir
		{
			get { return _assemblyDir; }
			set { _assemblyDir = value; }
		}

		public override bool Execute()
		{
			AssemblyFullPath = typeof(GetAssemblyPath).Assembly.Location;
			AssemblyDir = Path.GetDirectoryName(AssemblyFullPath);
			return true;
		}
	}
}
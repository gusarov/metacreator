using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MetaCreator.Installer
{
	///<summary>
	///</summary>
	[RunInstaller(true)]
	public class NgenInstaller : System.Configuration.Install.Installer
	{
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);

			ExecuteNGen("install", true);
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);

			ExecuteNGen("uninstall", false);
		}

		void ExecuteNGen(string cmd, bool validate)
		{
			var ngenStr = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "ngen");
			var assemblyPath = Context.Parameters["assemblypath"];

			var process = new Process
				{
					StartInfo =
						{
							FileName = ngenStr,
							Arguments = string.Format(@"{0} ""{1}""", cmd, assemblyPath),
							CreateNoWindow = true,
							UseShellExecute = false,
						}
				};

			process.Start();
			process.WaitForExit();

			if (validate && process.ExitCode != 0)
			{
				throw new Exception("Ngen exit code: " + process.ExitCode);
			}
		}
	}
}
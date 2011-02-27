using System.Collections;
using System.ComponentModel;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MetaCreator.Installer
{
	/// <summary>
	/// Meta creator installer
	/// </summary>
	[RunInstaller(true)]
	public class PatchMSBuildTargets : System.Configuration.Install.Installer
	{
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
			ProcessFiles(true);
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
			ProcessFiles(false);
		}

		static IEnumerable<string> MsBuildTargetsToProcess
		{
			get
			{
				//yield return GetMsBuildTargetsPath("v2.0", false);
				yield return GetMsBuildTargetsPath("v3.5", false);
				yield return GetMsBuildTargetsPath("v4.0", false);
				//yield return GetMsBuildTargetsPath("v2.0", true);
				//yield return GetMsBuildTargetsPath("v3.5", true);
				//yield return GetMsBuildTargetsPath("v4.0", true);
			}
		}

		static string GetMsBuildTargetsPath(string ver, bool x64)
		{
			var p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "MSBuild");
			p = Path.Combine(p, ver);
			p = Path.Combine(p, "Custom.After.Microsoft.Common.targets");
			return p;
		}

		static void ProcessFile(string fileName, bool appendOrRemoveOnly)
		{
			var content = "<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'></Project>";
			var dir = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			if (File.Exists(fileName))
			{
				content = File.ReadAllText(fileName);
			}
			File.WriteAllText(fileName, ProcessFile_CustomAfterMicrosoftCommonCore(content, appendOrRemoveOnly));
		}

		internal static string AppendToCustomAfterMicrosoftCommonCore(string fileContent)
		{
			return ProcessFile_CustomAfterMicrosoftCommonCore(fileContent, true);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		static string ProcessFile_CustomAfterMicrosoftCommonCore(string fileContent, bool appendOrRemoveOnly)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(fileContent);
			var nsman = new XmlNamespaceManager(xmlDoc.NameTable);
			var ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			nsman.AddNamespace("x", ns);

			var root = xmlDoc.SelectSingleNode("x:Project", nsman);

			// remove old

			var arr = root.SelectNodes("x:Import[contains(@Project,'MetaCreator')]", nsman);

			if (arr != null)
			{
				foreach (XmlNode item in arr)
				{
					root.RemoveChild(item);
				}
			}

			// add new
			if (appendOrRemoveOnly)
			{
				var import = xmlDoc.CreateElement("Import", ns);

				var pathToMCT = @"$(ProgramFiles)\MetaCreator\MetaCreator.targets";

				import.SetAttribute("Project", pathToMCT);
				import.SetAttribute("Condition", @" '$(DontImportMetaCreator)' != 'True' AND Exists('" + pathToMCT + "')");

				root.AppendChild(import);
			}

			using (var ms = new MemoryStream())
			{
				using (var tw = new StreamWriter(ms))
				{
					using (var writer = new XmlTextWriter(tw))
					{
						writer.Indentation = 1;
						writer.IndentChar = '\t';
						writer.Formatting = Formatting.Indented;
						xmlDoc.Save(writer);
					}
				}
				return Encoding.UTF8.GetString(ms.ToArray());
			}

		}

		static void ProcessFiles(bool appendOrRemoveOnly)
		{
			foreach (var item in MsBuildTargetsToProcess)
			{
				ProcessFile(item, appendOrRemoveOnly);
			}
		}
	}
}
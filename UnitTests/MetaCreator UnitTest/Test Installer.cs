using System.Linq;
using MetaCreator.Installer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
//using MetaCreatorInstallerHelper;

namespace MetaCreator_UnitTest
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class TestInstaller
	{


		[TestMethod]
		public void TestMethod1()
		{
			var actual = PatchMSBuildTargets.AppendToCustomAfterMicrosoftCommonCore(@"<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
	<!-- some -->
	<Import Some='a' />
	<Some />
</Project>");

			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(actual);
			//xmlDoc.NamespaceURI;
			var nsman = new XmlNamespaceManager(xmlDoc.NameTable);
			//nsman.AddNamespace(string.Empty, "http://schemas.microsoft.com/developer/msbuild/2003");
			nsman.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
			Assert.IsTrue(xmlDoc.SelectNodes("//x:Import", nsman).Cast<XmlNode>().Any(x => x.OuterXml.Contains("MetaCreator")));

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
	<!-- some -->
	<Import Some=""a"" />
	<Some />
	<Import Project=""$(ProgramFiles)\MetaCreator\MetaCreator.targets"" Condition="" '$(DontImportMetaCreator)' != 'True' AND Exists('$(ProgramFiles)\MetaCreator\MetaCreator.targets')"" />
</Project>";
			//File.WriteAllText(Path.GetTempFileName(), expected);
			//File.WriteAllText(Path.GetTempFileName(), actual);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void TestMethod2()
		{
			var actual = PatchMSBuildTargets.AppendToCustomAfterMicrosoftCommonCore(@"<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
	<!-- some -->
	<Import Some='a' />
	<Some />
	<Import Project=""$(ProgramFiles)\PostSharp\PostSharp.targets"" Condition="" '$(DontImportPostSharp)' != 'True' AND Exists('$(ProgramFiles)\PostSharp\PostSharp.targets')"" />
	<Import Project=""$(ProgramFiles)\MetaCreator\MetaCreator.targets"" Condition="" '$(DontImportMetaCreator)' != 'True' AND Exists('$(ProgramFiles)\MetaCreator\MetaCreator.targets')"" />
</Project>");

//DebugAssert.AreEqual(@"<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
//<!--The following element has been added by PostSharp Setup Program.-->
//<Import Project=""C:\Program Files\PostSharp 2.0\PostSharp.targets"" Condition="" '$(DontImportPostSharp)' == '' AND Exists('C:\Program Files\PostSharp 2.0\PostSharp.targets')"" />
//<Import Project=""$(ProgramFiles)\MetaCreator\MetaCreator.targets"" Condition="" '$(DontImportMetaCreator)' != 'True' AND Exists('$(ProgramFiles)\MetaCreator\MetaCreator.targets')"" /></Project>", actual);

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
	<!-- some -->
	<Import Some=""a"" />
	<Some />
	<Import Project=""$(ProgramFiles)\PostSharp\PostSharp.targets"" Condition="" '$(DontImportPostSharp)' != 'True' AND Exists('$(ProgramFiles)\PostSharp\PostSharp.targets')"" />
	<Import Project=""$(ProgramFiles)\MetaCreator\MetaCreator.targets"" Condition="" '$(DontImportMetaCreator)' != 'True' AND Exists('$(ProgramFiles)\MetaCreator\MetaCreator.targets')"" />
</Project>";
			//File.WriteAllText(Path.GetTempFileName(), expected);
			//File.WriteAllText(Path.GetTempFileName(), actual);
			Assert.AreEqual(expected, actual);
		}
	}
}

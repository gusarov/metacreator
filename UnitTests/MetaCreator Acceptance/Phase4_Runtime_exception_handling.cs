using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{

	[TestClass]
	public class Phase4_Runtime_exception_handling : Error_handling_base_tests
	{

		[TestMethod]
		public void Should_jump_to_generated_source_on_exception_in_generated_code()
		{
			Assert.Inconclusive();
			File.WriteAllText("sample.cs", @"
class q
{
	static q()
	{
		/*!
			WriteLine(@""throw new System.Exception(""""test13"""");"");
		*/
	}

	static void Main()
	{
	}
}");

			RunMsbuild(true);
			Run("bin\\Debug\\sample", null, false);
			Console.WriteLine(File.ReadAllText(ParsedMsBuildError.FileName));

			Assert.AreEqual(@"obj\Debug\sample.g.cs", ParsedMsBuildError.FileName);
			Assert.AreEqual(10, ParsedMsBuildError.Line);
			Assert.AreEqual(11, ParsedMsBuildError.Column);
			Assert.AreEqual(@"obj\Debug\sample.g.cs(10,11): System.Exception: test13", _output.Trim());
		}

		[TestMethod]
		public void Should_jump_to_original_source_on_exception_in_user_code_before()
		{
			Assert.Inconclusive();
			File.WriteAllText("sample.cs", @"
class q
{

	static q()
	{
		throw new System.Exception(""test13"");
	}

	/*!
		//string q = 5;
	*/

	static void Main()
	{
	}
}");

			RunMsbuild(false);

			Assert.AreEqual(Path.GetTempPath() + "sample.cs", ParsedMsBuildError.FileName, _output);
			Assert.AreEqual(9, ParsedMsBuildError.Line, _output);
			Assert.AreEqual(2, ParsedMsBuildError.Column, _output);

			Assert.AreEqual(Path.GetTempPath() + "sample.cs(9,2): System.Exception: test13", _output.Trim());
		}

		[TestMethod]
		public void Should_jump_to_original_source_on_exception_in_user_code_after()
		{
			File.WriteAllText("sample.cs", @"
class q
{

	/*!
		//string q = 5;
	*/

	static void Main()
	{
		try
		{
			throw new System.Exception(""test13"");
		}
		catch (System.Exception ex)
		{
			System.Console.WriteLine(ex.ToString());
		}
	}
}");

			RunMsbuild(true);

			Run("bin\\debug\\sample.exe", "", true);

			StringAssert.Contains(_output, "test13");
			StringAssert.Contains(_output, "sample.cs:line 13");
		}

		[TestMethod]
		public void Should_jump_to_generated_source_on_exception_in_user_code_marked_with_extender()
		{
			Assert.Inconclusive();

			File.WriteAllText("sample.cs", @"
/*@ ErrorRemap off */
class q
{
	/*!
		//string q = 5;
	*/
	static q()
	{
		throw new Exception(""test13"");
	}

	static void Main()
	{
	}
}");

			RunMsbuild(false);

			Assert.AreEqual(@"bin\Debug\sampleg.g.cs", ParsedMsBuildError.FileName);
			Assert.AreEqual(5, ParsedMsBuildError.Line);
			Assert.AreEqual(17, ParsedMsBuildError.Column);

			Assert.AreEqual(@"bin\Debug\sampleg.g.cs(5,17): System.Exception: test13", _output.Trim());
		}

	}
}

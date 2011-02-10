using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{
	[TestClass]
	public class Phase1_Meta_compile_error_handling : Error_handling_base_tests
	{

		[TestMethod]
		public void Should_jump_to_original_source_on_error_in_meta_code()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		string q = 5;
	*/
	static void Main()
	{
	}
}");


			RunMsbuild(false);

			Assert.AreEqual("sample.cs", ParsedMsBuildError.FileName);
			Assert.AreEqual(5, ParsedMsBuildError.Line);
			Assert.AreEqual(14, ParsedMsBuildError.Column);

			Assert.AreEqual("sample.cs(5,14): error : MetaCode: Cannot implicitly convert type 'int' to 'string'", _output.Trim());
		}

		[TestMethod]
		public void Should_jump_to_meta_source_on_error_in_non_user_meta_code()
		{
			File.WriteAllText("sample.cs", @"
/*@ using someThingWrong */
class q
{
	/*!
		//string q = 5;
	*/
	static void Main()
	{
	}
}");


			RunMsbuild(false);

			Assert.AreEqual(Path.GetTempPath() + "sample.meta.cs", ParsedMsBuildError.FileName, _output);
			Assert.AreEqual(6, ParsedMsBuildError.Line, _output);
			Assert.AreEqual(7, ParsedMsBuildError.Column, _output);

			Assert.AreEqual(Path.GetTempPath() + "sample.meta.cs(6,7): error : MetaCode: The type or namespace name 'someThingWrong' could not be found (are you missing a using directive or an assembly reference?)", _output.Trim());
		}

		[TestMethod]
		public void Should_jump_to_generated_source_on_error_in_user_meta_code_marked_with_extender()
		{
			File.WriteAllText("sample.cs", @"
/*@ ErrorRemap disable */
class q
{
	/*!
		string q = 5;
	*/
	static void Main()
	{
	}
}");


			RunMsbuild(false);

			Assert.AreEqual(Path.GetTempPath() + "sample.meta.cs", ParsedMsBuildError.FileName, _output);
			Assert.AreEqual(47, ParsedMsBuildError.Line, _output);
			Assert.AreEqual(14, ParsedMsBuildError.Column, _output);

			Assert.AreEqual(Path.GetTempPath() + "sample.meta.cs(47,14): error : MetaCode: Cannot implicitly convert type 'int' to 'string'", _output.Trim());
		}

		[TestMethod]
		public void Should_compile_successful_sample()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		WriteLine(""// hello world"");
	*/
	static void Main()
	{
	}
}");


			RunMsbuild(true);

			Assert.AreEqual("", _output.Trim(), _output);
		}


		[TestMethod]
		public void Should_report_correct_errors_when_several_metablocks_Exists()
		{
			File.WriteAllText("sample.cs", @"
/*@ some unknown extender */

#if /*=
""DE""+""BUG""
*/
#endif
class q
{
	/*!
		// string q = 5;
		// string q = 5;
	*/
	/*!
		string q = 5;
	*/
	static void Main()
	{
	}
}");


			RunMsbuild(false);

			Assert.AreEqual("sample.cs", ParsedMsBuildError.FileName);
			Assert.AreEqual(15, ParsedMsBuildError.Line);
			Assert.AreEqual(14, ParsedMsBuildError.Column);

			Assert.AreEqual("sample.cs(15,14): error : MetaCode: Cannot implicitly convert type 'int' to 'string'", _output.Trim());
		}
	}
}

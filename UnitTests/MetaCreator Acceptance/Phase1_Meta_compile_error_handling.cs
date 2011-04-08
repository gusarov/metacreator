using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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

			Assert.AreEqual("sample.cs",Path.GetFileName(ParsedMsBuildError.FileName));
			Assert.AreEqual(5, ParsedMsBuildError.Line);
			Assert.AreEqual(14, ParsedMsBuildError.Column);

			EndsWith("sample.cs(5,14): error : Metacode Compilation: Cannot implicitly convert type 'int' to 'string'");
		}

		void EndsWith(string sample)
		{
			var tr = _output.Trim();
			try
			{
				Assert.IsTrue(tr.EndsWith(sample));
			}
			catch
			{
				Console.WriteLine("A: "+sample);
				try
				{
					Console.WriteLine("E: " + tr.Substring(tr.Length - sample.Length));
				}
				catch {}
				throw;
			}
		}

		[TestMethod]
		public void Should_jump_to_original_source_on_error_in_meta_code_with_multiple_blocks()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		for(int i=0;i<10;i++) {
	*/

	public int i/*=
i */;

	/*!
		}

		#error test
	*/
	static void Main()
	{
	}
}");


			RunMsbuild(false);

			Assert.AreEqual("sample.cs", Path.GetFileName(ParsedMsBuildError.FileName));
			Assert.AreEqual(14, ParsedMsBuildError.Line);
			Assert.AreEqual(10, ParsedMsBuildError.Column);
			EndsWith(@"sample.cs(14,10): error : Metacode Compilation: #error: 'test'");
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
			Assert.AreEqual(7, ParsedMsBuildError.Column, _output);
			Assert.AreEqual(40, ParsedMsBuildError.Line, _output);

			Assert.AreEqual(Path.GetTempPath() + "sample.meta.cs(40,7): error : Metacode Compilation: The type or namespace name 'someThingWrong' could not be found (are you missing a using directive or an assembly reference?)", _output.Trim());
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
			Assert.AreEqual(55, ParsedMsBuildError.Line, _output);
			Assert.AreEqual(14, ParsedMsBuildError.Column, _output);

			Assert.AreEqual(Path.GetTempPath() + "sample.meta.cs(55,14): error : Metacode Compilation: Cannot implicitly convert type 'int' to 'string'", _output.Trim());
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
}
");


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

			Assert.AreEqual("sample.cs", Path.GetFileName(ParsedMsBuildError.FileName));
			Assert.AreEqual(15, ParsedMsBuildError.Line);
			Assert.AreEqual(14, ParsedMsBuildError.Column);

			Assert.IsTrue(_output.Trim().EndsWith("sample.cs(15,14): error : Metacode Compilation: Cannot implicitly convert type 'int' to 'string'"));
		}
	}
}

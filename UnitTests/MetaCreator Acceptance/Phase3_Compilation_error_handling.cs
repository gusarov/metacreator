using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{

	[TestClass]
	public class Phase3_Compilation_error_handling : Error_handling_base_tests
	{

		[TestMethod]
		public void Should_jump_to_generated_source_on_error_in_generated_code()
		{
			File.WriteAllText("sample.cs", @"
class q
{
/*!
	WriteLine(""string q1 = 5;"");
*/
	static void Main()
	{
	}
}");

			RunMsbuild(false);

			Assert.AreEqual(@"obj\Debug\sample.g.cs", ParsedMsBuildError.FileName);
			Assert.AreEqual(9, ParsedMsBuildError.Line, File.ReadAllText(ParsedMsBuildError.FileName));
			Assert.AreEqual(13, ParsedMsBuildError.Column);

			Assert.AreEqual(@"obj\Debug\sample.g.cs(9,13): error CS0029: Cannot implicitly convert type 'int' to 'string'", _output.Trim());
		}

		[TestMethod]
		public void Should_generate_banner_for_meta_result()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		WriteLine(""string q1 = 5;"");
	*/
	static void Main()
	{
	}
}");

			RunMsbuild(false);
			StringAssert.Contains(File.ReadAllText(ParsedMsBuildError.FileName), @"
// <MetaCreator>
// DO NOT MODIFY THIS GENERATED CODE
// You can use /*@ GenerateBanner off */ to disable this message
string q1 = 5;
// </MetaCreator>");
			CheckBanner(true);
		}

		[TestMethod]
		public void Should_not_generate_banner_for_meta_expression_result()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	static void Main()
	{
		Console.WrilteLine(""/*= new TimeSpan() */"");
	}
}");
			RunMsbuild(false);
			Console.WriteLine(ParsedMsBuildError.FileName);
			CheckBanner(false);
		}

		void CheckBanner(bool exp)
		{
			var body = File.ReadAllText(ParsedMsBuildError.FileName);
			Assert.AreEqual(exp, body.Contains("<MetaCreator>"), body);
		}

		[TestMethod]
		public void Should_not_generate_banner_by_request()
		{
			File.WriteAllText("sample.cs", @"
/*@ GenerateBanner off */
class q
{
	/*!
		WriteLine(""string q1 = 5;"");
	*/
	static void Main()
	{
	}
}");

			RunMsbuild(false);
			CheckBanner(false);
		}

		[TestMethod]
		public void Should_jump_to_original_source_on_error_in_user_code_before()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		string q = ""y"";
	*/
	static void Main()
	{
		int q = ""asd"";
	}
}");


			RunMsbuild(false);

			Assert.AreEqual("sample.cs", ParsedMsBuildError.FileName, _output);
			Assert.AreEqual(9, ParsedMsBuildError.Line, _output);
			Assert.AreEqual(11, ParsedMsBuildError.Column, _output);

			Assert.AreEqual("sample.cs(9,11): error CS0029: Cannot implicitly convert type 'string' to 'int'", _output.Trim());
		}

		[TestMethod]
		public void Should_jump_to_original_source_on_error_in_user_code_after()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	static void Main()
	{
		int q = ""asd"";
	}
	/*!
		string q = ""y"";
	*/
}");


			RunMsbuild(false);

			Assert.AreEqual("sample.cs", ParsedMsBuildError.FileName, _output);
			Assert.AreEqual(6, ParsedMsBuildError.Line, _output);
			Assert.AreEqual(11, ParsedMsBuildError.Column, _output);

			Assert.AreEqual("sample.cs(6,11): error CS0029: Cannot implicitly convert type 'string' to 'int'", _output.Trim());
		}

		[TestMethod]
		public void Should_jump_to_generated_source_on_error_in_user_code_marked_with_extender()
		{
			File.WriteAllText("sample.cs", @"
/*@ ErrorRemap disable */
class q
{
/*!
	string q = ""y"";
*/
	static void Main()
	{
		int q = ""asd"";
	}
}");


			RunMsbuild(false);

			Assert.AreEqual(@"obj\Debug\sample.g.cs", ParsedMsBuildError.FileName, _output);
			Assert.AreEqual(12, ParsedMsBuildError.Line, _output);
			Assert.AreEqual(11, ParsedMsBuildError.Column, _output);

			Assert.AreEqual(@"obj\Debug\sample.g.cs(12,11): error CS0029: Cannot implicitly convert type 'string' to 'int'", _output.Trim());
		}

	}
}

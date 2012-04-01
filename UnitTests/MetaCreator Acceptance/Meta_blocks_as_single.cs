using System.IO;

using MetaCreator_Acceptance.Properties;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MetaCreator_Acceptance
{


	[TestClass]
	public class Static_extensions : Acceptance_base_tests
	{

		[TestMethod]
		public void Should_allow_static_extensions()
		{
			File.WriteAllText("common.cs", Resources._Meta_blocks_as_single_common);
			Build("common");
			KillCs();

			File.WriteAllText("sample.cs", @"
class q
{
	static void Main()
	{
		MyMetaExtensions.Test();
		/*# MakeMyMagic */
	}
}");

			Build("test.exe", "common");
			Run("test.exe");

			Matches(_output, "!!");
		}
	}

	[TestClass]
	public class Meta_blocks_as_single : Acceptance_base_tests
	{

		[TestMethod]
		public void Should_use_blocks_from_different_parts()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	static void Main()
	{
		/*! for (var i=0; i < 10; i++) { */
		System.Console.WriteLine(""test "" + /*= i */);
		/*! } */
	}
}");


			BuildExe();
			Run();

			Assert.AreEqual(
@"test 0
test 1
test 2
test 3
test 4
test 5
test 6
test 7
test 8
test 9
", _output);
		}

		[TestMethod]
		public void Issue_38_should_work_with_empty_blocks()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	static void Main()
	{
		/*@ errorremap off */
		/*! */
		/*!*/
		/*! */
	}
}");

			BuildExe();
		}

		[TestMethod]
		public void Should_use_several_groups_of_block()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*! const int j = 5; */

	static void Main()
	{
		/*! for (var i=0; i < 3; i++) { */
		System.Console.WriteLine(""testA "" + /*= i */);
		/*! } */

		/*! for (var i=0; i < 3; i++) { */
		System.Console.WriteLine(""testB "" + /*= i */);
		/*! } */

		System.Console.WriteLine(""/*= j */"");

	}
}
");

			BuildExe();
			Run();

			Assert.AreEqual(
@"testA 0
testA 1
testA 2
testB 0
testB 1
testB 2
5
", _output);
		}
		
	}
}
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{
	[TestClass]
	public class Meta_blocks_as_single : Error_handling_base_tests
	{
		[TestInitialize]
		public void Init()
		{
			try
			{
				File.Delete("bin/debug/sample.exe");
			}
			catch { }
			Assert.IsFalse(File.Exists("bin/debug/sample.exe"));
		}

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


			RunMsbuild(true);

			Assert.IsTrue(File.Exists("bin/debug/sample.exe"));

			Run("bin/debug/sample.exe", "", true);

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
}");


			RunMsbuild(true);

			Assert.IsTrue(File.Exists("bin/debug/sample.exe"));

			Run("bin/debug/sample.exe", "", true);

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
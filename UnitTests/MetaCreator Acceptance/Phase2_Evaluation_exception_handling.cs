using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{

	[TestClass]
	public class Phase2_Evaluation_exception_handling : Error_handling_base_tests
	{

		[TestMethod]
		public void Should_jump_to_original_source_on_runtime_exception_in_meta_code()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		throw new ApplicationException(""test13"");
	*/
	static void Main()
	{
	}
}");

			RunMsbuild(false);

			Assert.AreEqual("sample.cs", Path.GetFileName(ParsedMsBuildError.FileName));
			Assert.AreEqual(5, ParsedMsBuildError.Line);
			Assert.IsTrue(_output.Trim().EndsWith("sample.cs(5): error : Metacode Execution: System.ApplicationException: test13"));

		}


		[TestMethod]
		public void Should_workout_a_loop_in_meta_code()
		{
			Assert.Inconclusive("+");
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		while(true);
	*/
	static void Main()
	{
	}
}");

			RunMsbuild(false);

			Assert.AreEqual("sample.cs", ParsedMsBuildError.FileName);
			Assert.AreEqual(4, ParsedMsBuildError.Line);
			Assert.AreEqual("sample.cs(4): error : MetaCode: System.Exception: MetaCodeExecutionTimeOut", _output.Trim());

		}


		[TestMethod]
		public void Should_workout_stack_owerflow_in_meta_code()
		{
			Assert.Inconclusive("+");
			File.WriteAllText("sample.cs", @"
class q
{
	/*!
			Action q = null;
			q = delegate { q(); };
			q();
	*/
	static void Main()
	{
	}
}");

			RunMsbuild(false);

			Assert.AreEqual("sample.cs", Path.GetFileName(ParsedMsBuildError.FileName));
			Assert.AreEqual(4, ParsedMsBuildError.Line);
			Assert.AreEqual("sample.cs(4): error : MetaCode: System.ApplicationException: test13", _output.Trim());

		}


		[TestMethod]
		public void Should_workout_out_of_memory_in_meta_code()
		{
			Assert.Inconclusive("+");

			File.WriteAllText("sample.cs", @"
class q
{
	/*!
		var list =new List<int[]>();
		while(true)
			list.Add(new int[short.MaxValue]);
	*/
	static void Main()
	{
	}
}");

			RunMsbuild(false);

			Assert.AreEqual("sample.cs", Path.GetFileName(ParsedMsBuildError.FileName));
			Assert.AreEqual(7, ParsedMsBuildError.Line);
			Assert.AreEqual("sample.cs(7): error : MetaCode: System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown.", _output.Trim());

		}
		
	}
}

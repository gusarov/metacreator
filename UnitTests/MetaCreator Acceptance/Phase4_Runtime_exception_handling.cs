using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{

	[TestClass]
	public class Phase4_Runtime_exception_handling : Acceptance_base_tests
	{

		[TestMethod]
		public void Should_jump_to_generated_source_on_exception_in_generated_code()
		{
			File.WriteAllText("sample.cs", @"
using System;
static class q
{
	static void Main()
	{
		try
		{
/*!
			WriteLine(@""throw new System.Exception(""""test13"""");"");
*/
		} catch (Exception ex) {
			Console.WriteLine(ex.StackTrace);
		}
	}
}");

			BuildExe();
			Run();
			Matches(_output, @"at q.Main() in *\obj\Debug\sample.g.cs:line 13");
		}

		[TestMethod]
		public void Should_jump_to_original_source_on_exception_in_user_code_before()
		{
			File.WriteAllText("sample.cs", @"
using System;
static class q
{
	static void Main()
	{
		try
		{
			throw new System.Exception(""test13"");
		} catch (Exception ex) {
			Console.WriteLine(ex.StackTrace);
		}
	}

	/*!
		//string q = 5;
	*/
}");

			BuildExe();
			Run();
			Matches(_output, @"at q.Main() in *sample.cs:line 9");
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

			BuildExe();
			Run();

			StringAssert.Contains(_output, "test13");
			StringAssert.Contains(_output, "sample.cs:line 13");
			Matches(_output, @"at q.Main() in *sample.cs:line 13");
		}

		[TestMethod]
		public void Should_jump_to_generated_source_on_exception_in_user_code_marked_with_extender()
		{

			File.WriteAllText("sample.cs", @"
/*@ ErrorRemap off */
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

			BuildExe();
			Run();

			Matches(_output, @"at q.Main() in *sample.g.cs:line 10");
		}

	}
}

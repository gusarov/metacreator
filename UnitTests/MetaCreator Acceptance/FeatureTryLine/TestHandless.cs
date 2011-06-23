using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance.FeatureTryLine
{
	[TestClass]
	public class TestHandless :Error_handling_base_tests
	{

		[TestMethod]
		public void Should_check_that_exceptions_are_catched()
		{
			// Setup
			File.WriteAllText("sample.cs", @"
/*! */
class q
{
	static int Main()
	{
		try
		{
			/*# trap
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			*/
			{
				new Object1().Object2.Object.ToString();
				new Object1().Object2.Object.ToString();
			}
			/*# stop */
		}
		catch
		{
			return 1;
		}
		return 0;
	}
}
class Object1
{
	Object2 _object2 = new Object2();
	public Object2 Object2
	{
		get
		{
			System.Console.WriteLine(""Good"");
			return _object2;
		}
		set { _object2 = value; }
	}
}

class Object2
{
	public object Object;
}

");

			// Execute
			RunMsbuild(true);
			Console.WriteLine(File.ReadAllText("obj\\Debug\\sample.g.cs"));
			Run("bin\\Debug\\sample", null, true);

			// Verify
			Assert.AreEqual("Good\r\nGood\r\n", _output);
		}


	}
}

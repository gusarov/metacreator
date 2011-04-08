using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetaCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class Code1Builder : ExecuteMetaCreatorBase
	{
		readonly MetaCreator.Evaluation.Code1Builder _builder = new MetaCreator.Evaluation.Code1Builder();

		[TestMethod, Ignore]
		public void Should_generate_metacode_from_surogate_syntax()
		{
			const string surogate = @"
using SomeSuper;

public class Class1
{
	public static void Main()
	{
		var message = ""hello world"";
		/*! Write(""System.Console.WriteLine(message);""); */
	}
}
";

			SimulateBuild(surogate);

			Assert.Inconclusive();
		}

	}
}

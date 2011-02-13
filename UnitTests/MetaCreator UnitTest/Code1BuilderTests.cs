using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MetaCreator;
using MetaCreator.Evaluation;

using MetaCreator_UnitTest.Properties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class Code1BuilderTests
	{
		readonly Code1Builder _builder = new Code1Builder();

		[TestMethod]
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

			var metacode = _builder.Build(surogate, new ProcessFileCtx());
			File.WriteAllText(Path.GetTempFileName(), metacode);

			Assert.AreEqual(Resources.Should_generate_metacode_from_surogate_syntax, metacode);
		}
	}
}

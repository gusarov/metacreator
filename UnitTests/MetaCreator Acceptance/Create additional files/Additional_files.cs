using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MetaCreator_Acceptance.Properties;

namespace MetaCreator_Acceptance.Create_additional_files
{
	[TestClass]
	public class Additional_files : Acceptance_base_tests
	{
		[TestMethod]
		public void Should_do_something()
		{
			File.WriteAllText("test.cs", Resources._sample_additional_files);
			Build();

			var test = LoadAssembly().GetType("Test");
			Assert.IsNotNull(test.GetMethod("Method1"));
			Assert.IsNotNull(test.GetMethod("Method2"));
		}
	}
}

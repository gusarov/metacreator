using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MetaCreator_Acceptance.Properties;

namespace MetaCreator_Acceptance.Extensions
{
	[TestClass]
	public class ExtensionsTests : Acceptance_base_tests
	{
		[TestMethod]
		public void Should_call_extensions()
		{
			File.WriteAllText("common.cs", Resources._sample_extensions_common);
			Build("common");
			KillCs();

			File.WriteAllText("test.cs", Resources._sample_extensions_test);
			Build("test", "common");

			var asm = LoadAssembly("test");
			Assert.IsNotNull(asm);
			var test = asm.GetType("Test");
			Assert.IsNotNull(test);
			var mi1 = test.GetMethod("TestExtensionSample1");
			var mi2 = test.GetMethod("TestExtensionSample2");
			var mi3 = test.GetMethod("TestExtensionSample3_ok");
			var mi4 = test.GetMethod("TestExtensionSample3_ko");

			Assert.IsNotNull(mi1);
			Assert.IsNotNull(mi2);
			Assert.IsNotNull(mi3);
			Assert.IsNotNull(mi4);

			Assert.AreEqual("done", mi1.Invoke(null, null));
			Assert.AreEqual("done", mi2.Invoke(null, null));
			Assert.AreEqual("done", mi3.Invoke(null, null));
			Assert.AreEqual("fail", mi4.Invoke(null, null));
		}
	}
}

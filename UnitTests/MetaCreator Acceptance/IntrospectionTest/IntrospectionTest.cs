using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using MetaCreator_Acceptance.Properties;

namespace MetaCreator_Acceptance.IntrospectionTest
{
	[TestClass]
	public class IntrospectionTest : Acceptance_base_tests
	{
		[TestMethod]
		public void Should_allow_insert_csharp_type_identifier()
		{
			File.WriteAllText("common.cs", Resources._sampleIntroscopeCommon);
			Build("Common");
			KillCs(); 

			File.WriteAllText("test.cs", Resources._sampleIntroscopeTests);
			Build(null, "Common");

			var test = LoadAssembly().GetType("Test");
			Assert.IsNotNull(test);

			var testA = test.GetMethod("get_TestA");
			var testB = test.GetMethod("get_TestB");
			var testC = test.GetMethod("get_TestC");
			var testD = test.GetMethod("get_TestD");
			Assert.IsNotNull(testA);
			Assert.IsNotNull(testB);
			Assert.IsNotNull(testC);
			Assert.IsNotNull(testD);

			Assert.AreEqual("T<T<int, Version>, Text.RegularExpressions.Regex>", testA.Invoke(null, null));
			Assert.AreEqual("T<T<int, Version>, Text.RegularExpressions.Regex>", testB.Invoke(null, null));
			Assert.AreEqual("n.T<n.T<int, System.Version>, RegularExpressions.Regex>", testC.Invoke(null, null));
			Assert.AreEqual("n.T<n.T<int, System.Version>, System.Text.RegularExpressions.Regex>", testD.Invoke(null, null));
		}
	}
}

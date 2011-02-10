using MetaCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UsageTest
{
	[TestClass]
	public class BasicTemplating_UnitTests
	{

		[TestMethod]
		public void Should_generate_properties()
		{
			var obj = new Sample();
			obj.Pro0 = "asd";
		}

		[TestMethod]
		public void Should_allow_access_to_public_Eval_API()
		{
			Assert.AreEqual(5, Evaluator.EvaluateExpression("3+2"));
		}

		[TestMethod]
		public void Should_preprocess_code_and_run_macros()
		{
			var value = MetaCode.SomeMetaCalculations;
			Assert.AreNotEqual("2*2 = 2*2", value, "add instead of calc");
			Assert.AreEqual("2*2 = 4", value);
		}
	}
}
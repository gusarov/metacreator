using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeSubProject;

namespace MetaCreator_UsageTest
{
	[TestClass]
	public partial class MacrosWithExternalReference
	{
		[TestMethod]
		public void Should_run_macros_with_external_referance()
		{
			Assert.AreEqual("SomeThirdPartyData", Run());
		}
	}
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeSubProject;

namespace MetaCreator_UsageTest
{
	[TestClass]
	public partial class MacrosWithMetaOnlyExternalReference
	{
		[TestMethod, Ignore]
		public void Should_run_macros_with_external_referance()
		{
			Assert.Inconclusive("+"); // this is the same implementation as for internal reference for now
			Assert.IsNotNull(WellKnownClass.GetSomeResult());
			Assert.AreEqual(WellKnownClass.GetSomeResult(), Run());
		}
	}
}

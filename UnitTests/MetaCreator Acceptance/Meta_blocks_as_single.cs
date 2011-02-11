using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{
	[TestClass]
	public class Meta_blocks_as_single : Error_handling_base_tests
	{
		[TestMethod]
		public void Should_use_blocks_from_different_parts()
		{
			File.WriteAllText("sample.cs", @"
class q
{
	/*! for(var i=0;i<10;i++) { */
	public int v/*= i */
	/*! } */

	static void Main()
	{
	}
}");


			RunMsbuild(true);

			Assert.Inconclusive();
		}
		
	}
}
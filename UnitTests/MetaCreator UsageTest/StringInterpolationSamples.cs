using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UsageTest
{
	partial class StringInterpolationSamples
	{
		/*@ StringInterpolation enable */
		public static void Run()
		{
			var youHaveSomeLocalVariable = 123; 

			Assert.AreEqual("And it can be interpolated with 123 string", "And it can be interpolated with {youHaveSomeLocalVariable} string");

			for (int i=0;i<10 ;i++ ) {
				Trace.WriteLine("Currently is {i} iteration");
			}
			

		}
	}
}

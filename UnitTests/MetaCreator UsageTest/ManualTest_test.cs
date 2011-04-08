using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SomeTestSolution
{
	partial class ManualTests
	{

		static void Main()
		{
			var someValue = 15;

			var q = /*=
				"\"asd\"+"
			*/
			"";

			// var qqqqqq = 5 /*= + new TimeSpan() */;

			/*!
				// Phase1 - compile error on meta code
				// #error 

				// Phase2 - runtime error on meta code
				// throw null; 

				// out of mem
				// var list =new List<int[]>();
				// 	while(true)
				// 	list.Add(new int[short.MaxValue]);

				// Phase3 A - compile error on generated code
				// WriteLine("#error"); 

				// Phase3 B - compile error on user code
				// uncomment a bad code after meta block

				// Phase4 A - run time error on generated code
				// WriteLine("throw null;"); 

				// Phase4 B - run time error on user code
				// uncomment a bad code after meta block
			*/

			//int qq = 1/0; // compiletime
			//int qq = 1/(int)(object)0; //runtime

			Assert.Inconclusive(someValue.ToString());
		}

		static void Use(int value)
		{

		}

		static void Use(string value)
		{

		}

		static void Use(Guid value)
		{

		}
	}
}

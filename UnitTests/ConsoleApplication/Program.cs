using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication
{
	class Program
	{
		public string /*= File.ReadAllText("SomeDataForMetacode.txt") */ { get; set; }

		static void Main()
		{
			//Console.WriteLine("/*= File.ReadAllText("SomeDataForMetacode.txt") */");

			foreach(var pi in typeof(Program).GetProperties())
			{
				Console.WriteLine(pi.Name);
			}

//	 		/* ! for (int i = 0; i < 10; i++) { */
//			Console.WriteLine("test /* = i */");
//			/* ! } */
//
//			/* ! for (int i = 0; i < 10; i++) { */
//			Console.WriteLine("test /* = i */");
//			/* ! } */

		}
	}
}

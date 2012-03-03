using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomeTestSolution
{
	public class Program
	{
		static void Main(string[] args)
		{
			object someValue = 15;

			var q = /*=
				"asd+"
			*/
			"";

			/*!
				int a = 1/(int)(object)0;
				WriteLine("someValue = 'a';");
			*/

			Console.WriteLine(someValue);
			Console.ReadLine(); 
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

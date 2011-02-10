using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaCreator;

namespace SampleWithReferenceToMC
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(/*= new Random().Next()*/);
			Console.ReadLine(); 
			
			/*	
		 	while(true)
			{
				var cmd = Console.ReadLine();
				if(string.IsNullOrEmpty(cmd))
				{
					break;
				}
				try
				{
					var result = Evaluator.EvaluateExpression(cmd);
					Console.WriteLine(result);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			*/
		}
	}
}

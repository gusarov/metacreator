using System;
using System.Text;
using MetaCreator;

class q
{
	static void Main()
	{
		/*@ errorremap off */
		Console.OutputEncoding = Encoding.UTF8;
		Console.WriteLine(/*!   Write(DummyEvaluator.Test("asd")); */);
		Console.WriteLine("/*!     Write(typeof(IGenerator).FullName); */");

		Console.WriteLine(/*# MyTest */);
	}
}
using System;
using MetaCreator.AppDomainIsolation;


class q
{
	static void Main()
	{
//		using(var appDom = MetaCreator.AppDomainIsolation.AnotherAppDomFactory.AppDomainLiveScope())
//		{
//			appDom.AnotherAppDomMarshal.Evaluate(null);
//		}

		var q = 5;

		/*!
			WriteLine("q= 6;");
		*/

		Console.WriteLine(q);
	}
}
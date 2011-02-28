using System;

/*@ reference System.Xml.dll */
/*@ using System.Xml */

/*@ reference ..\..\ThirdParty\SomeThirdPartyProject.dll */
/*@ using SomeThirdPartyProject */


class q
{
	static void Main()
	{
		string macroResult = null;

		var q = SomeThirdPartyProject.TestThirdPartyAPI.Test();
		
		/*!
			var doc = new XmlDocument();

			WriteLine("macroResult = \"" + TestThirdPartyAPI.Test()+"\";");
		*/

		Console.WriteLine(macroResult);
	}
}
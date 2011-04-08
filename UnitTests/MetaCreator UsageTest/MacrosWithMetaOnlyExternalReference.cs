/*@ reference ..\..\ThirdParty\SomeThirdPartyProject.dll */
/*@ using SomeThirdPartyProject */

namespace MetaCreator_UsageTest
{
	partial class MacrosWithExternalReference
	{
		string Run()
		{
			string macroResult = null;

			/*!
				WriteLine("macroResult = \"" + TestThirdPartyAPI.Test()+"\";");
			*/

			return macroResult;
		}
	}
}
/*@ ErrorRemap off */

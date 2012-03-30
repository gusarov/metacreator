/* @ ErrorRemap off */

namespace MetaCreator_UsageTest
{
	partial class MacrosWithMetaOnlyExternalReference
	{
		string Run()
		{
			string macroResult = null;

			// / *@ reference bla-bla */
			/*!
				// WriteLine("macroResult = \"" + WellKnownClass.GetSomeResult()+"\";");
			*/

			return macroResult;
		}
	}
}

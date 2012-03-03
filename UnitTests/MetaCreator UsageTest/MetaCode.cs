/*@ ErrorRemap disable  */

namespace MetaCreator_UsageTest
{
	class MetaCode
	{
		public static bool IsProcessed
		{
			get
			{
				///*return true;*/
				/*!
					//throw null;
					//1/(int)(object)0 
				*/
				return false;
			}
		}

		public static string SomeMetaCalculations
		{
			get
			{
				return "2*2 = /*=2*2*/";
			}
		}
	}
}

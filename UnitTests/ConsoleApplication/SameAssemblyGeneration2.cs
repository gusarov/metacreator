using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
	/*@ fileInProject */
	/*@ requiresLevel 1 */

	public static class SameAssemblyGeneration2
	{
		public static string Data
		{
			get { return "3_/*= ConsoleApplication.SameAssemblyGeneration1.Data */"; }
		}
	}
}

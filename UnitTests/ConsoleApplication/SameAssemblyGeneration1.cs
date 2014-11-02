using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
	/*@ fileInProject */
	/*@ metaLevel 1 */
	/*@ requiresLevel 255 */

	public static class SameAssemblyGeneration1
	{
		public static string Data
		{
			get { return "2_/*= ConsoleApplication.SameAssemblyGenerationUtils.Data */"; }
		}
	}
}

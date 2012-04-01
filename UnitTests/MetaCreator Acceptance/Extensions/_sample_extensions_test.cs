using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MetaCreator;

public static class Test
{
	static object _ref1 = typeof(SampleExtensions);

	public static object TestExtensionSample1()
	{
		/*# ExtensionSample1 */
		return null;
	}

	public static object TestExtensionSample2()
	{
		/*# ExtensionSample2 done */
		return null;
	}

	public static object TestExtensionSample3_ok()
	{
		/*# ExtensionSample3 ok */
		return null;
	}

	public static object TestExtensionSample3_ko()
	{
		/*# ExtensionSample3 ko */
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaCreator;

public static class TestExtension
{
	public static void MyTest(this IMetaWriter writer)
	{
		writer.WriteLine(@" "" haha! MyTest "" ");
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaCreator;

public static class TestExtension
{
	public static void MyTest(this IWriter writer)
	{
		writer.WriteLine(@" "" haha! MyTest "" ");
	}
}

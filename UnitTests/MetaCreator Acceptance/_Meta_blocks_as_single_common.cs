using System;
using MetaCreator;

public static class MyMetaExtensions
{
	public static void Test(){}

	public static void MakeMyMagic(this IMetaWriter writer)
	{
		string msg = "!!";
		writer.WriteLine(@"System.Console.WriteLine(""" + msg + @""");");
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MetaCreator;

public static class SampleExtensions
{
	public static void ExtensionSample1(this IMetaWriter writer)
	{
		writer.WriteLine("return \"done\";");
	}

	public static void ExtensionSample2(this IMetaWriter writer, string arg1)
	{
		writer.WriteLine("return \"" + arg1 + "\";");
	}

	public static void ExtensionSample3(this IMetaWriter writer, string arg1)
	{
		writer.WriteLine("return \"" + ((arg1 == "ok")?"done":"fail") + "\";");
	}
}

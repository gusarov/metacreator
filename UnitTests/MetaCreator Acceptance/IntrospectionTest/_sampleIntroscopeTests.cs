using System;
using n;

public static class Test
{
	public static string TestA
	{
		get { return "/*= MixinExtension.CSharpTypeIdentifier(typeof(T<T<int, string>, System.Text.RegularExpressions.Regex>)) */"; }
	}

	public static string TestB
	{
		get { return "/*= MixinExtension.CSharpTypeIdentifier(typeof(T<T<int, string>, System.Text.RegularExpressions.Regex>), Engine.Imports) */"; }
	}
}


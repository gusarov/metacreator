using System;
using n;

public static class Test
{
	public static string TestA
	{
		get { return "/*= SharpGenerator.CSharpTypeIdentifier(typeof(T<T<int, System.Version>, System.Text.RegularExpressions.Regex>)) */"; }
	}

	public static string TestB
	{
		get { return "/*= SharpGenerator.CSharpTypeIdentifier(typeof(T<T<int, System.Version>, System.Text.RegularExpressions.Regex>), Engine.Imports) */"; }
	}

	public static string TestC
	{
		get { return "/*= SharpGenerator.CSharpTypeIdentifier(typeof(T<T<int, System.Version>, System.Text.RegularExpressions.Regex>), false, "System.Text") */"; }
	}

	public static string TestD
	{
		get { return "/*= SharpGenerator.CSharpTypeIdentifier(typeof(T<T<int, System.Version>, System.Text.RegularExpressions.Regex>), false) */"; }
	}
}


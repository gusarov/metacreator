using System;
using System.Collections.Generic;
using System.Linq;

using MetaCreator;
using MetaCreator.Utils;

public static class SharpGenerator
{
	public static string CSharpTypeIdentifier(this Type type, params string[] imports)
	{
		return CSharpTypeIdentifier(type, true, imports);
	}

	public static string CSharpTypeIdentifier(this Type type, bool useEngineImports = true, params string[] imports)
	{
		imports = imports ?? Enumerable.Empty<string>().ToArray();

		if (imports.Length == 0 && useEngineImports)
		{
			imports = imports.Concat(EngineState.Imports ?? Enumerable.Empty<string>()).Distinct().ToArray();
		}

		var keyword = TrySubstitudeWithKeyword(type);
		if (keyword != null)
		{
			return keyword;
		}

		// namespace
		var ns = GetNamespace(type.Namespace, imports);
		// type name
		var i = type.Name.IndexOf('`');
		var name = type.Name;
		if (i > 0)
		{
			name = name.Substring(0, i);
		}
		// generics
		var generics = type.GetGenericArguments().Select(x => CSharpTypeIdentifier(x, useEngineImports, imports)).Join(", ");
		if (generics.Length > 0)
		{
			generics = "<" + generics + ">";
		}
		return ns + name + generics;
	}

	internal static string GetNamespace(string reflectedNamespace, params string[] imports)
	{
		if (imports.Any(x => x == reflectedNamespace))
		{
			reflectedNamespace = "";
		}
		else
		{
			var closesSpace = imports.Where(x => reflectedNamespace.StartsWith(x + ".")).OrderByDescending(x => x.Length).FirstOrDefault();
			if (closesSpace != null)
			{
				reflectedNamespace = reflectedNamespace.Substring(closesSpace.Length + 1);
			}
		}
		return reflectedNamespace + (string.IsNullOrEmpty(reflectedNamespace) ? "" : ".");
	}

	static readonly Dictionary<Type, string> _keywords = new Dictionary<Type, string>
		{
			{typeof(void), "void"},

			{typeof(bool), "bool"},
			{typeof(byte), "byte"},
			{typeof(char), "char"},
			{typeof(decimal), "decimal"},
			{typeof(double), "double"},
			{typeof(float), "float"},
			{typeof(int), "int"},
			{typeof(long), "long"},
			{typeof(sbyte), "sbyte"},
			{typeof(short), "short"},
			{typeof(uint), "uint"},
			{typeof(ulong), "ulong"},
			{typeof(ushort), "ushort"},
			{typeof(object), "object"},
			{typeof(string), "string"},
		};

	internal static string TrySubstitudeWithKeyword(Type type)
	{
		string keyword;
		return _keywords.TryGetValue(type, out keyword) ? keyword : null;
	}
}

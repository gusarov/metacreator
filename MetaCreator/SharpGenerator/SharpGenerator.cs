using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MetaCreator;
using MetaCreator.Utils;

public static class SharpGenerator
{
	public static string CSharpTypeIdentifier(this Type type, params string[] imports)
	{
		return CSharpTypeIdentifier(type, true, null, imports);
	}

	public static string CSharpTypeIdentifier(this Type type, string outerSpace, params string[] imports)
	{
		return CSharpTypeIdentifier(type, true, outerSpace, imports);
	}

	public static string CSharpTypeIdentifier(this Type type, bool useEngineImports = true, string outerSpace = null, params string[] imports)
	{
		imports = imports ?? Enumerable.Empty<string>().ToArray();

		if (useEngineImports)
		{
			imports = imports.Concat(EngineState.Imports ?? Enumerable.Empty<string>()).Distinct().ToArray();
		}

		if (string.IsNullOrEmpty(outerSpace) && useEngineImports)
		{
			outerSpace = EngineState.OuterNamespace;
		}

		var keyword = TrySubstitudeWithKeyword(type);
		if (keyword != null)
		{
			return keyword;
		}

		// namespace
		var ns = GetNamespace(type.Namespace, outerSpace, imports);
		// type name
		var i = type.Name.IndexOf('`');
		var name = type.Name;
		if (i > 0)
		{
			name = name.Substring(0, i);
		}
		// generics
		var generics = type.GetGenericArguments().Select(x => CSharpTypeIdentifier(x, useEngineImports, outerSpace, imports)).Join(", ");
		if (generics.Length > 0)
		{
			generics = "<" + generics + ">";
		}
		return ns + name + generics;
	}

	// todo if you have
	// Root1.Foo.Type1
	// Root2.Foo.Type2
	// using Root1
	// using Root2
	// than you can not just specify Foo.Type1 (!)
	// more information is required to generate that - list of referenced assemblies and cached tree of namespaces
	// NEW:
	// namespace xxx - is allways performs as shortcut
	// using xxx - should be specified completely
	internal static string GetNamespace(string reflectedNamespace, string outerSpace = null, params string[] imports)
	{
		if (reflectedNamespace == null)
		{
			reflectedNamespace = string.Empty;
		}
		if (imports.Any(x => x == reflectedNamespace))
		{
			reflectedNamespace = string.Empty;
		}
		else if (outerSpace == reflectedNamespace)
		{
			reflectedNamespace = string.Empty;
		}
		else
		{
			// only for outerspace!
			// var closesSpace = imports.Where(x => reflectedNamespace.StartsWith(x + ".")).OrderByDescending(x => x.Length).FirstOrDefault();
			if (!string.IsNullOrEmpty(outerSpace))
			{
				var outers = new LinkedList<string>();
				outers.AddLast(outerSpace);
				int i = 0;
				while (true)
				{
					i = outerSpace.IndexOf('.', i+1);
					if (i > 0)
					{
						outers.AddFirst(outerSpace.Substring(0, i));
					}
					else
					{
						break;
					}
				}
				foreach (var outer in outers)
				{
					if (reflectedNamespace.StartsWith(outer + "."))
					{
						reflectedNamespace = reflectedNamespace.Substring(outer.Length + 1);
					}
				}
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

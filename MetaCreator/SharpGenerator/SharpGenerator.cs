using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MetaCreator;
using MetaCreator.Utils;

public static class SharpGenerator
{
	public class TypeIdentifierConfig
	{
		public TypeIdentifierConfig(bool useEngineImports = true, string outerSpace = null, params string[] imports)
		{
			UseEngineImports = useEngineImports;
			OuterSpace = outerSpace;
			Imports = imports;
		}

		public TypeIdentifierConfig()
		{
		}

		/// <summary>
		/// Use namespace imports (usings) provided by meta creator builder from the file with metacode
		/// </summary>
		public bool UseEngineImports;

		/// <summary>
		/// Specify namespace at which type identifier will be placed. This allows to shorten a namespace. UseEngineImports provides that automatically from metafile.
		/// </summary>
		public string OuterSpace;

		/// <summary>
		/// Specify existing namespace imports in a scope at which type identifier will be placed. This allows to shorten a namespace. UseEngineImports provides that automatically from metafile.
		/// </summary>
		public string[] Imports;

		/// <summary>
		/// Specify whether to use named type parameters or leave just generic type definition
		/// </summary>
		public bool UseNamedTypeParameters = true;
	}

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
		return CSharpTypeIdentifier(type, new TypeIdentifierConfig(useEngineImports, outerSpace, imports));
	}

	public static string CSharpTypeIdentifier(this Type type, TypeIdentifierConfig config)
	{
		return CSharpTypeIdentifier(type, config, default(StackContext));
	}

	struct StackContext
	{
		public string Postfix{get;set;}
	}

	static string CSharpTypeIdentifier(this Type type, TypeIdentifierConfig config, StackContext ctx)
	{
		var useEngineImports = config.UseEngineImports;
		var outerSpace = config.OuterSpace;
		var imports = config.Imports;

		imports = imports ?? Enumerable.Empty<string>().ToArray();

		if (useEngineImports)
		{
			imports = imports.Concat(EngineState.Imports ?? Enumerable.Empty<string>()).Distinct().ToArray();
		}

		if (string.IsNullOrEmpty(outerSpace) && useEngineImports)
		{
			outerSpace = EngineState.OuterNamespace;
		}

		if (type.IsGenericParameter && !config.UseNamedTypeParameters)
		{
			return string.Empty;
		}

		if (type.IsArray)
		{
			var arr = '[' + new string(',', type.GetArrayRank() - 1) + ']';
			ctx.Postfix += arr;
			return type.GetElementType().CSharpTypeIdentifier(config, ctx);
		}

		var keyword = TrySubstitudeWithKeyword(type);
		if (keyword != null)
		{
			return keyword + ctx.Postfix;
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
		if (type.IsNested)
		{
			name = type.DeclaringType.CSharpTypeIdentifier(config) + "." + name;
		}
		// generics
		var genericArgs = type.GetGenericArguments();
		var generics = genericArgs.Select(x => CSharpTypeIdentifier(x, config/*, ctx*/)).Join(", ");
		if (genericArgs.Length > 0)
		{
			generics = "<" + generics + ">";
		}
		return ns + name + generics + ctx.Postfix;
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
		var answers = new List<string>();
		if (reflectedNamespace == null)
		{
			reflectedNamespace = string.Empty;
		}
		if (imports.Any(x => x == reflectedNamespace))
		{
			answers.Add(string.Empty);
		}
		else if (outerSpace == reflectedNamespace)
		{
			answers.Add(string.Empty);
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
					i = outerSpace.IndexOf('.', i + 1);
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
						answers.Add(reflectedNamespace.Substring(outer.Length + 1));
					}
				}
			}
		}
		var answer = answers.Any() ? answers.First(x => x.Length == answers.Min(y => y.Length)) : reflectedNamespace;
		return answer + (string.IsNullOrEmpty(answer) ? "" : ".");
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

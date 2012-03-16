using System.Linq;
using System.Collections.Generic;
using System;

using MetaCreator.Utils;

namespace MetaCreator.Extensions
{
	public static class DummyEvaluator
	{
		public static string Test(string a)
		{
			return '"' + a.ToUpperInvariant() + '"';
		}
	}

	public static class MixinExtension
	{
		const string _aggregatorPattern = @"{0} {1} = new {2}();";

		const string _methodPattern = @"public {0} {2}({3}) {{
{5} {1}.{2}({4});
}}";
		const string _eventPattern = @"public event {0} {2} {{
add {{ {1}.{2} += value; }}
remove {{ {1}.{2} -= value; }}
}}";
		const string _propertyPattern = @"public {0} {2} {{
get {{ return {1}.{2}; }}
set {{ {1}.{2} = value; }}
}}";

		public static void Mixin<TImpl>(this IMetaWriter writer)
		{
			Mixin<TImpl, TImpl>(writer);
		}
		public static void Mixin<TFace, TImpl>(this IMetaWriter writer)
		{
			writer.WriteLine(_aggregatorPattern,
				CSharpTypeIdentifier(typeof(TFace)),
				typeof(TFace).Name,
				CSharpTypeIdentifier(typeof (TImpl)));

			foreach (var mi in typeof(TFace).GetEvents())
			{
				writer.WriteLine(_eventPattern,
					CSharpTypeIdentifier(mi.EventHandlerType), // 0
					CSharpTypeIdentifier(typeof(TFace)), // 1
					mi.Name // 2
					);
			}
//
//			foreach (var mi in typeof(TFace).GetProperties())
//			{
//				writer.WriteLine(_methodPattern,
//					CSharpTypeIdentifier(mi.ReturnType), // 0
//					CSharpTypeIdentifier(typeof(TFace)), // 1
//					mi.Name, // 2
//					mi.GetParameters().Select(x => "{0} {1}".Arg(CSharpTypeIdentifier(x.ParameterType), x.Name)).Join(", "), // 3 parameters
//					mi.GetParameters().Select(x => x.Name).Join(", "), // 4 arguments
//					mi.ReturnType != typeof(void) ? "return " : "" // 5 return?
//					);
//			}

			foreach (var mi in typeof(TFace).GetMethods().Where(x=>!x.IsSpecialName))
			{
				writer.WriteLine(_methodPattern,
					CSharpTypeIdentifier(mi.ReturnType), // 0
					CSharpTypeIdentifier(typeof(TFace)), // 1
					mi.Name, // 2
					mi.GetParameters().Select(x => "{0} {1}".Arg(CSharpTypeIdentifier(x.ParameterType), x.Name)).Join(", "), // 3 parameters
					mi.GetParameters().Select(x => x.Name).Join(", "), // 4 arguments
					mi.ReturnType!=typeof(void)?"return ":"" // 5 return?
					);
			}


		}

		public static string CSharpTypeIdentifier(Type type)
		{
			return CSharpTypeIdentifier(type, null);
		}

		public static string CSharpTypeIdentifier(Type type, params string[] imports)
		{
			imports = imports ?? Enumerable.Empty<string>().ToArray();

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
			var generics = type.GetGenericArguments().Select(x => CSharpTypeIdentifier(x, imports)).Join(", ");
			if (generics.Length > 0)
			{
				generics = "<" + generics + ">";
			}
			return ns + name + generics;
		}

		public static string GetNamespace(string reflectedNamespace, params string[] imports)
		{
			if (imports.Any(x => x == reflectedNamespace))
			{
				reflectedNamespace = "";
			}
			else
			{
				var closesSpace = imports.Where(x => reflectedNamespace.StartsWith(x + ".")).OrderByDescending(x => x.Length).FirstOrDefault();
				if(closesSpace != null)
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

		public static string TrySubstitudeWithKeyword(Type type)
		{
			string keyword;
			return _keywords.TryGetValue(type, out keyword) ? keyword : null;
		}
	}
}

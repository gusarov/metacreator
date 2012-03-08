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
		const string _aggregatorPattern = @"{0} __mixin{0} = new {1}();";

		const string _methodPattern = @"public {0} {2}({3}) {{
{5} __mixin{1}.{2}({4});
}}";
		// const string _methodPatternExplicit = "{0} {1}.{2}({3});";

		public static void Mixin<TFace, TImpl>(this IMetaWriter writer)
		{
			writer.WriteLine(_aggregatorPattern, CSharpTypeIdentifier(typeof(TFace)), CSharpTypeIdentifier(typeof(TImpl)));
			foreach (var mi in typeof(TFace).GetMethods())
			{
				writer.WriteLine(_methodPattern,
					CSharpTypeIdentifier(mi.ReturnType), // 0
					CSharpTypeIdentifier(typeof(TFace)), // 1
					mi.Name, // 2
					mi.GetParameters().Select(x => "{0} {1}".Arg(CSharpTypeIdentifier(x.ParameterType), x.Name)).Join(", "), // 3 parameters
					mi.GetParameters().Select(x => x.Name).Join(", "), // 4 arguments
					mi.ReturnType != typeof(void) ? "return " : "" // 5 return?
					);
			}
		}

		public static void Mixin<TImpl>(this IMetaWriter writer)
		{
			Mixin<TImpl, TImpl>(writer);
		}

		static string CSharpTypeIdentifier(Type type)
		{
			return type.FullName;
		}
	}
}
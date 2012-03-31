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
								  CSharpTypeIdentifier(typeof(TImpl)));

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

			foreach (var mi in typeof(TFace).GetMethods().Where(x => !x.IsSpecialName))
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

		public static string CSharpTypeIdentifier(Type type, params string[] imports)
		{
			return type.CSharpTypeIdentifier(imports);
		}

	}
}

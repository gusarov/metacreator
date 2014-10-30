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
		public static void Mixin<TImpl>(this IMetaWriter writer)
		{
			Mixin<TImpl, TImpl>(writer);
		}

		public static void Mixin<TFace, TImpl>(this IMetaWriter writer)
		{
			const string _aggregatorPattern = @"{0} {1} = new {2}();";

			var aggregatorName = "_" + typeof(TFace).CSharpTypeIdentifier();

			writer.WriteLine(_aggregatorPattern,
				typeof(TFace).CSharpTypeIdentifier(),
				aggregatorName,
				typeof(TImpl).CSharpTypeIdentifier());

			#region Events

			foreach (var mi in typeof(TFace).GetEvents())
			{
				writer.WriteLine(_eventPattern,
					mi.EventHandlerType.CSharpTypeIdentifier(), // 0
					aggregatorName, // 1
					mi.Name // 2
					);
			}

			#endregion

			#region Properties

			foreach (var pi in typeof(TFace).GetProperties())
			{
				writer.WriteLine(_propertyPattern,
					pi.PropertyType.CSharpTypeIdentifier(), // 0
					aggregatorName, // 1
					pi.Name // 2
//					pi.GetParameters().Select(x => "{0} {1}".Arg(CSharpTypeIdentifierCore(x.ParameterType), x.Name)).Join(", "), // 3 parameters
//					pi.GetParameters().Select(x => x.Name).Join(", "), // 4 arguments
//					pi.ReturnType != typeof(void) ? "return " : "" // 5 return?
					);
			}

			#endregion

			//todo indexers
			//todo readonly/writeonly pros

			#region Methods

			const string _methodPattern = @"public {0} {2}({3}) {{
{5} {1}.{2}({4});
}}";

			foreach (var mi in typeof(TFace).GetMethods().Where(x => !x.IsSpecialName && x.DeclaringType != typeof(object)))
			{
				writer.WriteLine(_methodPattern,
									  mi.ReturnType.CSharpTypeIdentifier(), // 0
									  aggregatorName, // 1
									  mi.Name, // 2
									  mi.GetParameters().Select(x => "{0} {1}".Arg(x.ParameterType.CSharpTypeIdentifier(), x.Name)).Join(", "), // 3 parameters
									  mi.GetParameters().Select(x => x.Name).Join(", "), // 4 arguments
									  mi.ReturnType != typeof(void) ? "return " : "", // 5 return?
									  aggregatorName
					);
			}

			#endregion
		}


		const string _eventPattern = @"public event {0} {2} {{
add {{ {1}.{2} += value; }}
remove {{ {1}.{2} -= value; }}
}}";
		const string _propertyPattern = @"public {0} {2} {{
get {{ return {1}.{2}; }}
set {{ {1}.{2} = value; }}
}}";

	}
}

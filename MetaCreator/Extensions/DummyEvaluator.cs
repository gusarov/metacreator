using System.Linq;
using System.Collections.Generic;
using System;

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
		public static void Mixin<TFace, TImpl>(this IWriter writer)
		{
			writer.WriteLine("// IMPL!");
		}
	}
}
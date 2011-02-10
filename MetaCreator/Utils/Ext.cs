using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MetaCreator.Utils
{
	static class Ext
	{
		public static string SafeToString(this object obj)
		{
			return obj == null ? null : obj.ToString();
		}

		public static string Arg(this string str, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, str, args);
		}

		public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
		{
			return source ?? Enumerable.Empty<T>();
		}
	}
}

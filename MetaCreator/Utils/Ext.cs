using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;

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

		public static string Join(this IEnumerable<string> args, string separator = null)
		{
			return string.Join(separator ?? "", args.ToArray());
		}

		public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
		{
			return source ?? Enumerable.Empty<T>();
		}

		static readonly Random _rnd = new Random();

		public static int Random(this int value)
		{
			return _rnd.Next(value);
		}

		public static string GenerateId()
		{
			// return Guid.NewGuid().ToString("N");
			return _rnd.Next().ToString("X");
		}

		public static BuildWarningEventArgs ConvertToWarning(this BuildErrorEventArgs e)
		{
			return new BuildWarningEventArgs(e.Subcategory, e.Code, e.File, e.LineNumber, e.ColumnNumber, e.EndLineNumber, e.EndColumnNumber, e.Message, e.HelpKeyword, e.SenderName, e.Timestamp);
		}
	}
}

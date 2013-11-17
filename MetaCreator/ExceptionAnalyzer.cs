using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text;

namespace MetaCreator
{
	public static class ExceptionAnalyzer
	{
		public static string ExceptionDetails(Exception exception)
		{
			var sb = new StringBuilder();
			sb.AppendLine(exception.Message);
			if (exception.InnerException != null)
			{
				var innerDetails = ExceptionDetails(exception.InnerException);
				if (!string.IsNullOrWhiteSpace(innerDetails))
				{
					sb.AppendLine("Inner: " + innerDetails);
				}
			}
			var rtlEx = exception as ReflectionTypeLoadException;
			if (rtlEx != null)
			{
				foreach (var item in rtlEx.LoaderExceptions)
				{
					sb.AppendLine("LoaderException: " + ExceptionDetails(item));
				}
			}
			var str = sb.Length == 0 ? null : sb.ToString();
			if (str != null)
			{
				str = str.TrimEnd('\r', '\n');
			}
			return str;
		}
	}
}
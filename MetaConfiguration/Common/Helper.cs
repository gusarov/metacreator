using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MetaConfiguration.Common
{
	internal static class Helper
	{
		public static string ToLowerCase(this string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;
			var firstChar = str.Trim()[0];
			return firstChar.ToString(CultureInfo.InvariantCulture).ToLowerInvariant() + str.Trim().Remove(0, 1);
		}
	}
}
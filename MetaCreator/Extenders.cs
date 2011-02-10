using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaCreator.Utils;
using Microsoft.Build.Framework;

namespace MetaCreator
{
	using E = Action<string, ProcessFileCtx>;

	static class Extenders
	{
		static readonly Dictionary<string, E> _mapExtenders = new Dictionary<string, E>
			{
				{"stringinterpolation", StringInterpolation},
				{"errorremap", ErrorRemap},
				{"reference", Reference},
				{"using", Using},
				{"generatebanner", GenerateBanner},
			};

		public static void ExecuteExtender(string name, string args, ProcessFileCtx ctx)
		{
			E value;
			if (_mapExtenders.TryGetValue(name.ToLowerInvariant(), out value))
			{
				value(args, ctx);
			}
			else
			{
				var knownExtenders = string.Join(string.Empty, _mapExtenders.Keys.Select(x => Environment.NewLine + x).ToArray());
				ctx.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, ctx.OriginalFileName, 0, 0, 0, 0,
					"Meta Extender '{0}' is unknown. Known extenders is:{1}".Arg(name.ToLowerInvariant(), knownExtenders), null, null));
			}
		}

		static bool ToBool(string val)
		{
			if(val == null)
			{
				return true;
			}
			switch (val.ToLowerInvariant().Trim())
			{
				case "enable":
				case "enabled":
				case "on":
				case "1":
				case "true":
				case "yes":
				case "":
					return true;
				case "disable":
				case "disabled":
				case "off":
				case "0":
				case "false":
				case "no":
					return false;
				default:
					throw new Exception("Can not convert '{0}' to bool".Arg(val));
			}
		}

		static void StringInterpolation(string arg, ProcessFileCtx ctx)
		{
			ctx.EnabledStringInterpolation = ToBool(arg);
		}

		static void ErrorRemap(string arg, ProcessFileCtx ctx)
		{
			ctx.ErrorRemap = ToBool(arg);
		}

		static void Reference(string arg, ProcessFileCtx ctx)
		{
			ctx.ReferencesMetaAdditional.Add(arg);
		}

		static void GenerateBanner(string arg, ProcessFileCtx ctx)
		{
			ctx.GenerateBanner = ToBool(arg);
		}

		static void Using(string arg, ProcessFileCtx ctx)
		{
			ctx.NamespaceImportsMetaAdditional.Add(arg);
		}
	}
}

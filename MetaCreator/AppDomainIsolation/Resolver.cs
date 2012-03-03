using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using MetaCreator.Utils;

namespace MetaCreator.AppDomainIsolation
{
	sealed class Resolver : IDisposable
	{
		readonly Func<ResolveEventArgs, Assembly> _func;
		readonly IEnumerable<string> _currentDomainAdditionalReferences;
		readonly AppDomain _domain;

		Resolver(AppDomain domain)
		{
			if (domain == null)
			{
				_domain = AppDomain.CurrentDomain;
			}
			else
			{
				_domain = domain;
			}
			_domain.AssemblyResolve += AssemblyResolve;
		}

		public Resolver(IEnumerable<string> additionalReferences, AppDomain domain = null)
			: this(domain)
		{
			_currentDomainAdditionalReferences = additionalReferences.OrEmpty();
		}

		public Resolver(Func<ResolveEventArgs, Assembly> func, AppDomain domain = null)
			: this(domain)
		{
			_func = func;
		}

		private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if(_func!=null)
			{
				return _func(args);
			}

			// search in additional references
			var i = args.Name.IndexOf(',');
			var name = i > 0 ? args.Name.Substring(0, i).TrimEnd(',').Trim() : args.Name;
			foreach (var currentDomainAdditionalReference in _currentDomainAdditionalReferences)
			{
				if (Path.GetFileNameWithoutExtension(currentDomainAdditionalReference) == name)
				{
					return Assembly.LoadFile(currentDomainAdditionalReference);
				}
			}
			return null;
		}

		void IDisposable.Dispose()
		{
			_domain.AssemblyResolve -= AssemblyResolve;
		}

	}
}
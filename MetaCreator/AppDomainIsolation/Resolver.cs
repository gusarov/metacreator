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

		Resolver()
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
		}

		public Resolver(IEnumerable<string> additionalReferences)
			: this()
		{
			_currentDomainAdditionalReferences = additionalReferences.OrEmpty();
		}

		public Resolver(Func<ResolveEventArgs, Assembly> func)
			: this()
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
			AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
		}

	}
}
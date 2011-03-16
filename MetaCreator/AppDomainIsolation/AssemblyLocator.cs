using System;
using System.Collections.Generic;
using System.Reflection;

namespace MetaCreator.AppDomainIsolation
{
	public static class AssemblyLocator
	{

		static Dictionary<string, Assembly> _assemblies;

		public static void Init()
		{
			if (_assemblies == null)
			{
				_assemblies = new Dictionary<string, Assembly>();
				AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainAssemblyLoad;
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
			}
		}

		static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly assembly;
			_assemblies.TryGetValue(args.Name, out assembly);
			return assembly;
		}

		static void CurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			var assembly = args.LoadedAssembly;
			_assemblies[assembly.FullName] = assembly;
		}

	}
}
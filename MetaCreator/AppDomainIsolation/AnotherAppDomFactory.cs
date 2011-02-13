using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using MetaCreator.Utils;

namespace MetaCreator.AppDomainIsolation
{
	class AnotherAppDomFactory : IDisposable
	{
		readonly string _id;
		readonly List<string> _tempDirrectoriesToRemoveAfterUnloadAppDomain = new List<string>();

		AnotherAppDomFactory()
		{
			_id = Ext.GenerateId();
			_anotherAppDomMarshal = Lazy.New(Initialize);
		}

		IAnotherAppDomMarshalApi Initialize()
		{
			_appDomain = AppDomain.CreateDomain("MetaCreator Evaluation " + _id);
			IAnotherAppDomMarshalApi result;
			using (new Resolver(delegate { throw null; }))
			{
				//result = (IAnotherAppDomMarshalApi)_appDomain.CreateInstanceFromAndUnwrap(typeof(AnotherAppDomMarshalApi).Assembly.Location, typeof(AnotherAppDomMarshalApi).FullName);
				result = new AnotherAppDomMarshalApi();
			}
			if (result == null)
			{
				throw new Exception("Can not create another app domain");
			}
			return result;
		}

		AppDomain _appDomain;
		readonly Lazy<IAnotherAppDomMarshalApi> _anotherAppDomMarshal;
		public IAnotherAppDomMarshalApi AnotherAppDomMarshal { get { return _anotherAppDomMarshal.Value; } }

		public static AnotherAppDomFactory AppDomainLiveScope()
		{
			return new AnotherAppDomFactory();
		}

//		static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
//		{
//			return Assembly.LoadFrom(typeof(AnotherAppDomMarshalApi).Assembly.Location);
//		}

		public void MarkDirectoryPathToRemoveAfterUnloadDomain(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("path is null or empty");
			}
			if(!Path.IsPathRooted(path))
			{
				throw new ArgumentException("Path is not rooted: " + path);
			}
			if (!Directory.Exists(path))
			{
				throw new ArgumentException("Directory does not exists: " + path);
			}
			_tempDirrectoriesToRemoveAfterUnloadAppDomain.Add(path);
		}

		public void Dispose()
		{
			if (_appDomain != null)
			{
				AppDomain.Unload(_appDomain);
				foreach (var dir in _tempDirrectoriesToRemoveAfterUnloadAppDomain)
				{
					try
					{
						Directory.Delete(dir, true);
					}
					catch
					{
					}
				}
				_tempDirrectoriesToRemoveAfterUnloadAppDomain.Clear();
			}
		}
	}
}
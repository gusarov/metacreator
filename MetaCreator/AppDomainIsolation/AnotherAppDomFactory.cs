using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
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
			_anotherAppDomMarshal = new Lazy<IAnotherAppDomMarshalApi>(Initialize);
		}

		IAnotherAppDomMarshalApi Initialize()
		{
			// supposed that if current domain is not default - we can skip creation of another app dom

			if (AppDomain.CurrentDomain.IsDefaultAppDomain())
			{
				_appDomain = AppDomain.CreateDomain("MetaCreator Evaluation " + _id, AppDomain.CurrentDomain.Evidence,
					new AppDomainSetup
					{
						ApplicationBase = Path.GetDirectoryName(typeof(AnotherAppDomFactory).Assembly.Location),
					});

				// _appDomain.AssemblyResolve += Resolver2._appDomain_TypeResolve;
				// _appDomain.AssemblyLoad += Resolver2._appDomain_TypeResolve2;
				// _appDomain.Load(File.ReadAllBytes(typeof(AnotherAppDomFactory).Assembly.Location));
				// _appDomain.Load()
				//			var basee = _appDomain.BaseDirectory;
				//
				//			var si = _appDomain.SetupInformation;

				//			if(typeof(IAnotherAppDomMarshalApi).Assembly.HostContext>0)
				//			{
				//				throw new Exception("Assembly.HostContext>0");
				//			}

				var remoteType = typeof(AnotherAppDomMarshalApi);

				var remoteObj = _appDomain.CreateInstanceAndUnwrap(remoteType.Assembly.FullName, remoteType.FullName);
				var result = (AnotherAppDomMarshalApi)remoteObj;
#if DEBUG
				if (result == null)
				{
					throw new Exception("Can not create another app domain");
				}
#endif
				return result;
			}
			else
			{
				return new AnotherAppDomMarshalApi();
			}
		}

		AppDomain _appDomain;
		bool _isDisposed;
		readonly Lazy<IAnotherAppDomMarshalApi> _anotherAppDomMarshal;
		public IAnotherAppDomMarshalApi AnotherAppDomMarshal
		{
			get
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException("App dom is not initialized or already disposed");
				}
				var dom = _anotherAppDomMarshal.Value;

				// var test = _appDomain.GetAssemblies();

				return dom;
			}
		}

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
			_isDisposed = true;
			if (_appDomain != null)
			{
				AppDomain.Unload(_appDomain);
				_appDomain = null;
			}
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
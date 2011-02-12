using System;
using System.Collections.Generic;
using System.Linq;

namespace MyUtils
{
	static class Lazy
	{
		public static Lazy<T> New<T>(Func<T> factory) where T : class
		{
			return new Lazy<T>(factory);
		}

		public static Lazy<T> New<T>() where T : class, new()
		{
			return new Lazy<T>(Activator.CreateInstance<T>);
		}

	}

	class Lazy<T> where T : class
	{
		Func<T> _factory;
		T _value;
		readonly object _factorySync = new object();

		public Lazy()
		{
#if PocketPC || Smartphone
			_factory = () => (T)typeof(T).GetType().GetConstructor(new Type[0]).Invoke(null);
#else
			_factory = Activator.CreateInstance<T>;
#endif
		}

		public Lazy(Func<T> factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			_factory = factory;
		}

		public Lazy(T value)
		{
			_value = value;
		}


		public T Value
		{
			get
			{
				if (_factory != null)
				{
					lock (_factorySync)
					{
						if (_factory != null)
						{
							_value = _factory();
							_factory = null;
						}
					}
				}
				return _value;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return _factory == null;
			}
		}

		public static implicit operator T(Lazy<T> lazy)
		{
			if (lazy == null)
			{
				throw new ArgumentNullException("lazy");
			}
			return lazy.Value;
		}

		public static implicit operator Lazy<T>(Func<T> factory)
		{
			return new Lazy<T>(factory);
		}

		public static implicit operator Lazy<T>(T value)
		{
			return new Lazy<T>(value);
		}

	}
}
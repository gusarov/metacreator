using System;
using System.Diagnostics;

namespace MetaCreator.Utils
{
	static class Assert
	{
		[Conditional("DEBUG")]
		static public void That(bool condition, string message)
		{
			ThatCore(condition, message);
		}

		[Conditional("DEBUG")]
		static public void That(bool condition)
		{
			ThatCore(condition, "Contract violated");
		}

		[Conditional("DEBUG")]
		static public void EnsureExists(this object obj, string message)
		{
			ThatCore(!ReferenceEquals(null, obj), "Contract violated");
		}

		[Conditional("DEBUG")]
		static public void EnsureExists(this object obj)
		{
			ThatCore(!ReferenceEquals(null, obj), "Contract violated - object is null");
		}

		static void ThatCore(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception(message);
			}
		}
	}
}

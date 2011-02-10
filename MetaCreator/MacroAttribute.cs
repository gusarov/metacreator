using System;

namespace MetaCreator
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public abstract class MacroAttribute : Attribute
	{
		protected abstract void Decorate(string body);
	}
}
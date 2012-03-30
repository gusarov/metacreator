using System;

namespace MetaCreator
{
	/// <summary>
	/// beta
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public abstract class MacroAttribute : Attribute
	{
		/// <summary>
		/// beta
		/// </summary>
		/// <param name="body"></param>
		protected abstract void Decorate(string body);
	}
}
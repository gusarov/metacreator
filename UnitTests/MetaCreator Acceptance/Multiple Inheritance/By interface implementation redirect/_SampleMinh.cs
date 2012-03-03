public class Base
{
	public string BaseMethod(string arg)
	{
		return "Base_" + arg;
	}
}

public class Derived : Base /*= ", IBeh" */
{
	/*# Mixin<IBeh, IBehImpl> */

	public string DerivedMethod(string arg)
	{
		return "Derived_" + arg;
	}
}


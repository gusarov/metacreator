public class Base
{
	public string BaseMethod(string arg)
	{
		return "Base_" + arg;
	}
}

public class Derived : Base /* = ", IBeh" */
{
	/*= "// " + Directory.GetCurrentDirectory() */
	/* # Mixin<IBeh, BehImpl> */

	public string DerivedMethod(string arg)
	{
		return "Derived_" + arg;
	}
}


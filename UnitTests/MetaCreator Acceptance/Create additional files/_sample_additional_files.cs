public partial class Test
{
	public void Method1(){}

	/*!

	Engine.AddToCompile(@"

public partial class Test
{
	public void Method2(){}
}

");

	*/
}
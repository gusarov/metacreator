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

	Engine.AddToCompile("my_file_name.cs", @"

public partial class Test
{
	public void MethodNamed(){}
}

");

	Engine.AddToCompile(@"

public partial class Test
{
	public void Method3(){}
}

");

	*/
}
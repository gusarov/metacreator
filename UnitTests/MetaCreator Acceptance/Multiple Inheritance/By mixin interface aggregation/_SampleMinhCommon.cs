public interface IBeh
{
	string BehMethod(string arg);
}

public class BehImpl : IBeh
{
	public string BehMethod(string arg)
	{
		return "BehImpl_" + arg;
	}
}
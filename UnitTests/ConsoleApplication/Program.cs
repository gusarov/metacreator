using System;
using System.Diagnostics;
using System.Text;

class q
{
	static int Main()
	{
		try
		{
			/*# trap
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			*/
			new Object1().Object2.Object.ToString();
			new Object1().Object2.Object.ToString();
			new Object1().Object2.Object.ToString();
			/*# stop */
		}
		catch
		{
			Console.WriteLine("Error");
			return 1;
		}
		return 0;
	}
}
class Object1
{
	Object2 _object2 = new Object2();
	public Object2 Object2
	{
		get
		{
			System.Console.WriteLine("Good");
			return _object2;
		}
		set { _object2 = value; }
	}
}

class Object2
{
	public object Object;
}
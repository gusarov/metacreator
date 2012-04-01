using System;
using System.ComponentModel;

/* ! Debugger.Launch(); */

public class Base
{
}

public class Test : Base /*= ", INotifyPropertyChanged " */
{
	/*# Mixin<Observable> */

	int _testProperty;

	public int TestProperty
	{
		get { return _testProperty; }
		set
		{
			_testProperty = value;
			OnPropertyChanged("TestProperty");
		}
	}

}


using System;
using System.ComponentModel;

public class Observable : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged(string name)
	{
		OnPropertyChanged(new PropertyChangedEventArgs(name));
	}

	protected void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		var handler = PropertyChanged;
		if (handler != null)
		{
			handler(this, e);
		}
	}

	int _testProperty2;

	public int TestProperty2
	{
		get { return _testProperty2; }
		set
		{
			_testProperty2 = value;
			OnPropertyChanged("TestProperty2");
		}
	}

}


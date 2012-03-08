
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
}


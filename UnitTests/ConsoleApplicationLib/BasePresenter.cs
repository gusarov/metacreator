using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplicationLib
{
	public class SomeWorkItem
	{
		public string ConnectionString { get; set; }

		public SomeWorkItem(string connectionString)
		{
			ConnectionString = connectionString;
		}
	}

	public abstract class BasePresenter
	{
		protected readonly SomeWorkItem _workItem;
		protected readonly string _prefix;

		protected BasePresenter()
		{
			
		}

		protected BasePresenter(SomeWorkItem workItem)
		{
			_workItem = workItem;
			_prefix = workItem.ConnectionString;
		}
	}

}

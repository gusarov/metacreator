using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplicationLib;

namespace ConsoleApplication
{
	public class MyPresenter : BasePresenter
	{
		public MyPresenter(string connectionString)
			: base(new SomeWorkItem(connectionString))
		{
			
		}

		/*# Mixin<SecurityBasePresenter> */
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

/*@ fileInProject */

namespace ConsoleApplicationLib
{

	public class SecurityBasePresenter : BasePresenter
	{
		protected SecurityBasePresenter(BasePresenter @base)
			: base(@base)
		{
		}

		public string S()
		{
			return _prefix + "S";
		}
	}

	public class UdfBasePresenter : BasePresenter
	{
		public string U()
		{
			return _prefix + "U";
		}
	}

	public class NotificationAwareBasePresenter : BasePresenter
	{
		public string N()
		{
			return _prefix + "N";
		}
	}
}

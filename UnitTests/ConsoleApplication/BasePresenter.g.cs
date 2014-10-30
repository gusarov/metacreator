
#line 1 "C:\Users\DGusarov\Dropbox\__Projects\Metacreator\UnitTests\ConsoleApplication\BasePresenter.cs"
using System;
using System.Collections.Generic;
using System.Linq;


#line default

#line 5 "C:\Users\DGusarov\Dropbox\__Projects\Metacreator\UnitTests\ConsoleApplication\BasePresenter.cs"


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

#line default

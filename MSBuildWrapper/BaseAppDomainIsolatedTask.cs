using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

[LoadInSeparateAppDomainAttribute]
public abstract class BaseAppDomainIsolatedTask : AppDomainIsolatedTask
{
	public Action<string> LogMessage = delegate { };
}

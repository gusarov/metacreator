using System;
using Microsoft.Build.Framework;
using System.Diagnostics;

namespace MetaCreator
{
	internal interface IBuildErrorLogger
	{
		void LogDebug(string msg);
		void LogOutputMessage(string msg);
		void LogErrorEvent(BuildErrorEventArgs ev);
		void LogWarningEvent(BuildWarningEventArgs ev);
		bool ErrorsExists { get;  }
	}
}

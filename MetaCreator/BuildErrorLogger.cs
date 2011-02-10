using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace MetaCreator
{
	class BuildErrorLogger : IBuildErrorLogger
	{
		readonly IBuildEngine _buildEngine;
		readonly TaskLoggingHelper _taskLoggingHelper;

		public BuildErrorLogger(IBuildEngine buildEngine, TaskLoggingHelper taskLoggingHelper)
		{
			_buildEngine = buildEngine;
			_taskLoggingHelper = taskLoggingHelper;
		}

		public bool ErrorsExists { get; private set; }

		public void LogDebug(string msg)
		{
#if DEBUG // it is required, because this method usually calls by interface. It can not be conditional.
			LogOutputMessage(msg);
#endif
		}

		public void LogOutputMessage(string msg)
		{
			_taskLoggingHelper.LogMessage(MessageImportance.High, "* MetaCreator: " + msg);
		}

		public void LogErrorEvent(BuildErrorEventArgs ev)
		{
			ErrorsExists = true;
			_buildEngine.LogErrorEvent(ev);
		}

		public void LogWarningEvent(BuildWarningEventArgs ev)
		{
			_buildEngine.LogWarningEvent(ev);
		}
	}
}

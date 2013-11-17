using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace MetaCreator
{
	class BuildErrorLoggerConfig
	{
		public bool Debug { get; set; }
	}

	class BuildErrorLogger : IBuildErrorLogger
	{
		readonly IBuildEngine _buildEngine;
		readonly TaskLoggingHelper _taskLoggingHelper;
		private readonly BuildErrorLoggerConfig _config;

		public BuildErrorLogger(IBuildEngine buildEngine, TaskLoggingHelper taskLoggingHelper, BuildErrorLoggerConfig config)
		{
			_buildEngine = buildEngine;
			_taskLoggingHelper = taskLoggingHelper;
			_config = config;
		}

		public bool ErrorsExists { get; private set; }

		private const bool _isDebug =
#if DEBUG
			true;
#else
			false;
#endif

		public void LogDebug(string msg)
		{
			if (_config.Debug || _isDebug)
			{
				LogOutputMessage("DEBUG: " + msg);
			}
		}

		public void LogOutputMessage(string msg)
		{
			_taskLoggingHelper.LogMessage(MessageImportance.High, "* MetaCreator.dll: " + msg);
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

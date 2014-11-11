using System.Collections.Generic;
using MetaCreator.Utils;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace MetaCreator
{
	class BuildErrorLoggerConfig
	{
		public bool Debug { get; set; }
	}

	internal class BuildErrorLoggerDelayedProxy : IBuildErrorLogger
	{
		private readonly IBuildErrorLogger _logger;

		public BuildErrorLoggerDelayedProxy(IBuildErrorLogger logger)
		{
			_logger = logger;
		}

		private bool _delayed;
		private bool _warningsMode;

		public void DelayedStart()
		{
			_delayed = true;
		}

		public void DelayedCommit()
		{
			_delayed = false;
			lock (_events)
			{
				foreach (var ev in _events)
				{
					ev.MakeCall(this);
				}
				_events.Clear();
			}
		}

		public void DelayedCommitAsWarnings()
		{
			_warningsMode = true;
			try
			{
				DelayedCommit();
			}
			finally
			{
				_warningsMode = false;
			}
		}

		private readonly List<Call> _events = new List<Call>();

		private abstract class Call
		{
			public abstract void MakeCall(BuildErrorLoggerDelayedProxy logger);
		}

		private class CallDebug : Call
		{
			public string Message { get; set; }

			public override void MakeCall(BuildErrorLoggerDelayedProxy logger)
			{
				logger.LogDebug(Message);
			}
		}

		private class CallErrorEvent : Call
		{
			public BuildErrorEventArgs Ev { get; set; }

			public override void MakeCall(BuildErrorLoggerDelayedProxy logger)
			{
				if (logger._warningsMode)
				{
					logger.LogWarningEvent(Ev.ConvertToWarning());
				}
				else
				{
					logger.LogErrorEvent(Ev);
				}
			}
		}

		private class CallOutputMessage : Call
		{
			public string Message { get; set; }

			public override void MakeCall(BuildErrorLoggerDelayedProxy logger)
			{
				logger.LogOutputMessage(Message);
			}
		}

		private class CallWarningEvent : Call
		{
			public BuildWarningEventArgs Ev { get; set; }

			public override void MakeCall(BuildErrorLoggerDelayedProxy logger)
			{
				logger.LogWarningEvent(Ev);
			}
		}

		public void LogDebug(string msg)
		{
			if (_delayed)
			{
				lock (_events)
				{
					_events.Add(new CallDebug
					{
						Message = msg,
					});
				}
			}
			else
			{
				_logger.LogDebug(msg);
			}
		}

		public void LogErrorEvent(BuildErrorEventArgs ev)
		{
			if (_delayed)
			{
				lock (_events)
				{
					_events.Add(new CallErrorEvent
					{
						Ev = ev,
					});
				}
			}
			else
			{
				_logger.LogErrorEvent(ev);
			}
		}

		public void LogOutputMessage(string msg)
		{
			if (_delayed)
			{
				lock (_events)
				{
					_events.Add(new CallOutputMessage
					{
						Message = msg,
					});
				}
			}
			else
			{
				_logger.LogOutputMessage(msg);
			}
		}

		public void LogWarningEvent(BuildWarningEventArgs ev)
		{
			if (_delayed)
			{
				lock (_events)
				{
					_events.Add(new CallWarningEvent
					{
						Ev = ev,
					});
				}
			}
			else
			{
				_logger.LogWarningEvent(ev);
			}
		}

		public bool ErrorsExists { get { return _logger.ErrorsExists; }}
	}

	internal class BuildErrorLogger : IBuildErrorLogger
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

		public virtual void LogDebug(string msg)
		{
			if (_config.Debug || _isDebug)
			{
				LogOutputMessage("DEBUG: " + msg);
			}
		}

		public virtual void LogOutputMessage(string msg)
		{
			_taskLoggingHelper.LogMessage(MessageImportance.High, "* MetaCreator.dll: " + msg);
		}

		public virtual void LogErrorEvent(BuildErrorEventArgs ev)
		{
			ErrorsExists = true;
			_buildEngine.LogErrorEvent(ev);
		}

		public virtual void LogWarningEvent(BuildWarningEventArgs ev)
		{
			_buildEngine.LogWarningEvent(ev);
		}


	}

}

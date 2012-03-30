using System;
using System.Collections.Generic;
using MetaCreator;
using Microsoft.Build.Framework;

namespace MetaCreator_UnitTest
{
	class FakeErrorLogger : IBuildErrorLogger
	{
		public List<BuildErrorEventArgs> Errors = new List<BuildErrorEventArgs>();
		public List<BuildWarningEventArgs> Warnings = new List<BuildWarningEventArgs>();
		public List<string> Output = new List<string>();

		public void Clear()
		{
			Errors.Clear();
			Warnings.Clear();
		}

		public void LogDebug(string msg)
		{
			
		}

		public void LogOutputMessage(string msg)
		{
			Output.Add(msg);
		}

		public void LogErrorEvent(BuildErrorEventArgs ev)
		{
			Errors.Add(ev);
		}

		public void LogWarningEvent(BuildWarningEventArgs ev)
		{
			Warnings.Add(ev);
		}

		public bool ErrorsExists
		{
			get { return Errors.Count > 0; }
		}

	}
}

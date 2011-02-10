using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MetaCreator.Utils;

namespace MetaCreator
{
	[Serializable]
	class EvaluationResult
	{
		public object ResultBody { get; set; }
		public CompilerError[] Errors { get; set; }
		public CompilerError[] Warnings { get; set; }
		public string SourceCode { get; set; }
		public string CompileError { get; set; }
		public Exception EvaluationException { get; set; }
		public int EvaluationExceptionAtLine
		{
			get
			{
				if (EvaluationException == null)
				{
					return 0;
				}
				var i = EvaluationException.StackTrace.IndexOf('\r');
				if (i <= 0)
				{
					i = EvaluationException.StackTrace.Length;
				}
				var stack = EvaluationException.StackTrace.Substring(0, i).Trim();
				var match = Regex.Match(stack, "(?i)" + Regex.Escape(Path.GetTempPath()) + @"[^:]+\.cs:\s?line (?'L'\d+)");
				if (match.Success)
				{
					return int.Parse(match.Groups["L"].Value);
				}
				return -1;
			}
		}

		public string CompileTempPath { get; set; }

		[NonSerialized]
		public string NonUserCode;
		[NonSerialized]
		public Assembly Assembly;

		public bool IsSuccess
		{
			get
			{
				return Errors.OrEmpty().Count() == 0 && CompileError == null && EvaluationException == null;
			}
		}
	}
}
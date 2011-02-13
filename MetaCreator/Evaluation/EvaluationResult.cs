using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using MetaCreator.Utils;

namespace MetaCreator.Evaluation
{
	[Serializable]
	public class EvaluationResult
	{
		public string ResultBody { get; set; }
		public CompilerError[] Errors { get; set; }
		public CompilerError[] Warnings { get; set; }
		public string SourceCode { get; set; }
		public string CompileError { get; set; }
		public Exception EvaluationException { get; set; }
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
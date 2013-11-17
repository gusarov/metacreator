using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Check for errors
	/// </summary>
	class Code4Analyze
	{
		public void Analyze(EvaluationResult evaluationResult, ProcessFileCtx ctx)
		{
			_ctx = ctx;
			_buildErrorLogger = ctx.BuildErrorLogger;

			ProcessEvaluationResult(evaluationResult);
		}

		void ProcessEvaluationResult(EvaluationResult result)
		{
			// attach new files
			foreach (var newFile in result.NewFiles ?? new EvaluationResult.NewFile[0])
			{
				AttachNewFile(result, newFile);
			}

#if DEBUG
			if (!string.IsNullOrEmpty(result.DebugLog))
			{
				_buildErrorLogger.LogDebug("Code4Analyze: Message from evaluation: " + Environment.NewLine + result.DebugLog);
			}
#endif

			// Log imformation about metacode and generated code
			//_buildErrorLogger.LogErrorEvent

			var macrosFailed = false;

			// Log meta code compile time errors
			if (result.Errors != null)
			{
				foreach (var error in result.Errors)
				{
					macrosFailed = true;
					_buildErrorLogger.LogErrorEvent(CreateBuildError(result, error));
				}
			}

			// Log meta code compile time warnings
			if (result.Warnings != null)
			{
				foreach (var error in result.Warnings)
				{
					_buildErrorLogger.LogWarningEvent(CreateBuildWarning(result, error));
				}
			}

			// Log meta code run time exceptions
			if (result.EvaluationException != null)
			{
				macrosFailed = true;
				// var linenumber = result.EvaluationException.
				var details = ExceptionAnalyzer.ExceptionDetails(result.EvaluationException);
				var message = result.EvaluationException.GetType().FullName + ": " + details;
				_buildErrorLogger.LogOutputMessage(result.EvaluationException.ToString() + "\r\n" + details);

				var i = result.EvaluationException.StackTrace.IndexOf('\r');
				if (i <= 0)
				{
					i = result.EvaluationException.StackTrace.Length;
				}
				var stack = result.EvaluationException.StackTrace.Substring(0, i).Trim();

				// at Generator.Run() in c:\Kip\Projects\MetaCreatorRep\UnitTests\ConsoleApplication\Program.cs:line 19
				var match = Regex.Match(stack, @"(?i)at (?'method'[^\s]+) in (?'file'.+):line (?'line'\d+)");
//				if (match.Success)
//				{
//				}
				var lineString = match.Groups["line"].Value;
				int line;
				int.TryParse(lineString, out line);

				var fileName = match.Groups["file"].Value;
				if(TempFiles.IsTemp(fileName))
				{
					fileName = GetMetaCodeFile(result);
				}

				_buildErrorLogger.LogErrorEvent(new BuildErrorEventArgs(null, null, fileName, line, 0, 0, 0, "Metacode Execution: " + message, null, null));

			}

			// terminate
			if (macrosFailed)
			{
				throw new FailBuildingException("$ terminating, jump to global catch and return false...");
			}
		}

		private string NullConcat(string a, string b)
		{
			if (a == null || b == null)
			{
				return null;
			}
			return a + b;
		}

		void AttachNewFile(EvaluationResult result, EvaluationResult.NewFile newFile)
		{
			_ctx.NewFiles.Add(newFile);
		}

		BuildWarningEventArgs CreateBuildWarning(EvaluationResult result, CompilerError error)
		{
			BuildError_GetLineNumber(error, result); // init non user code
			return new BuildWarningEventArgs(null, null, BuildError_GetFile(result, error), BuildError_GetLineNumber(error, result),
				BuildError_GetColumnNumber(error), 0, 0, BuildError_GetMessage(error, result),
				null, null);
		}

		BuildErrorEventArgs CreateBuildError(EvaluationResult result, CompilerError error)
		{
			return new BuildErrorEventArgs(null, null, BuildError_GetFile(result, error), BuildError_GetLineNumber(error, result),
			                               BuildError_GetColumnNumber(error), 0, 0, BuildError_GetMessage(error, result),
			                               null, null);
		}

		static int BuildError_GetColumnNumber(CompilerError error)
		{
			return error.Column;
		}

		int BuildError_GetLineNumber(CompilerError error, EvaluationResult result)
		{
			return BuildError_GetLineNumberCore(error, result);
		}

		static int BuildError_GetLineNumberCore(CompilerError error, EvaluationResult result)
		{
			return error.Line;
		}

		string BuildError_GetMessage(CompilerError error, EvaluationResult result)
		{
			// Log a message here. It is not very convinient considering a role of this procedure
			{
				var source = error.IsWarning ? "" : result.SourceCode;
				// Annotate source code with line numbers
				{
					source = string.Join(Environment.NewLine, source.Split('\r').Select((x, i) => (i + 1).ToString("000") + "| " + x.Trim('\n')).ToArray());
				}
				var references = string.Join(Environment.NewLine, result.References);

				var fullLogEntry = error.ErrorText + " at line " + error.Line + " col " + error.Column + Environment.NewLine + "References:" + Environment.NewLine + references + Environment.NewLine + source;
				_ctx.BuildErrorLogger.LogOutputMessage(fullLogEntry);
			}
			return "Metacode Compilation: " + error.ErrorText;
		}

		string GetMetaCodeFile(EvaluationResult result)
		{
			var fileName = TempFiles.GetNewTempFile(Path.GetFileNameWithoutExtension(_ctx.OriginalRelativeFileName) + ".meta.cs" /*+ Path.GetExtension(_ctx.OriginalFileName)*/);
			File.WriteAllText(fileName, result.SourceCode);
			return fileName;
		}

		string BuildError_GetFile(EvaluationResult result, CompilerError error)
		{
			// detect that error is redirrected from random name of dynamic file
			var errorFileName = Path.GetFileName(error.FileName).ToLowerInvariant();
			var intermFileName = Path.GetFileName(_ctx.GetOriginalFileNameRelativeToIntermediatePath()).ToLowerInvariant();
			var originalFileName = Path.GetFileName(_ctx.OriginalRelativeFileName).ToLowerInvariant();
			if (errorFileName == originalFileName || errorFileName == intermFileName)
			{
				return error.FileName;
			}



			result.NonUserCode = GetMetaCodeFile(result);
			return result.NonUserCode;

		//if (result.NonUserCode != null || !_ctx.ErrorRemap)
//			{
//				result.NonUserCode = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(_ctx.OriginalFileName) + ".meta" + Path.GetExtension(_ctx.OriginalFileName));
//				File.WriteAllText(result.NonUserCode, result.SourceCode);
//				return result.NonUserCode;
//			}
//			return _ctx.OriginalFileName;
		}

		ProcessFileCtx _ctx;
		IBuildErrorLogger _buildErrorLogger;

		//const string _metacreatorErrorPrefix = "MetaCode: ";

	}
}

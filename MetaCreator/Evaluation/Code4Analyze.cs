using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
		}

		BuildWarningEventArgs CreateBuildWarning(EvaluationResult result, CompilerError error, ProcessFileCtx ctx)
		{
			BuildError_GetLineNumber(error, result, ctx); // init non user code
			return new BuildWarningEventArgs(null, null, BuildError_GetFile(ctx, result), BuildError_GetLineNumber(error, result, ctx),
				BuildError_GetColumnNumber(error), 0, 0, BuildError_GetMessage(error, result),
				null, null);
		}

		BuildErrorEventArgs CreateBuildError(EvaluationResult result, CompilerError error, ProcessFileCtx ctx)
		{
			BuildError_GetLineNumber(error, result, ctx); // init non user code
			return new BuildErrorEventArgs(null, null, BuildError_GetFile(ctx, result), BuildError_GetLineNumber(error, result, ctx),
				BuildError_GetColumnNumber(error), 0, 0, BuildError_GetMessage(error, result),
				null, null);
		}

		static int BuildError_GetColumnNumber(CompilerError error)
		{
			return error.Column;
		}

		const int _nonUserCodeSpecialLineNumber = -777;

		int BuildError_GetLineNumber(CompilerError error, EvaluationResult result, ProcessFileCtx ctx)
		{
			var line = BuildError_GetLineNumberCore(error, result, ctx);
			if (line == _nonUserCodeSpecialLineNumber)
			{
				if (ctx.ErrorRemap)
				{
					result.NonUserCode = "...";
				}
			}
			return line;
		}

		static int BuildError_GetLineNumberCore(CompilerError error, EvaluationResult result, ProcessFileCtx ctx)
		{
			if (!ctx.ErrorRemap)
			{
				return error.Line;
			}
			if (result.NonUserCode != null)
			{
				return error.Line;
			}

			return RemapErrorLineNumber(error.Line, ctx, result);
		}

		/// <summary>
		/// Allow to receive line number for compile time or run time error
		/// </summary>
		/// <param name="errorLine"></param>
		/// <param name="ctx"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private static int RemapErrorLineNumber(int errorLine, ProcessFileCtx ctx, EvaluationResult result)
		{
			var userCodeIndex = result.SourceCode.IndexOf("// <UserCode>");
			var userCodeLine = result.SourceCode.Substring(0, userCodeIndex).ToCharArray().Count(x => x == '\r') + 2; // +\r + next line

			if (userCodeIndex < 1)
			{
				return _nonUserCodeSpecialLineNumber;
			}
			if (errorLine < userCodeLine)
			{
				return _nonUserCodeSpecialLineNumber;
			}
			if (userCodeLine < 0)
			{
				return _nonUserCodeSpecialLineNumber;
			}
			return errorLine -
				userCodeLine + ctx.CurrentMacrosLineInOriginalFile;
		}

		string BuildError_GetMessage(CompilerError error, EvaluationResult result)
		{
			// Log a message here. It is not very convinient considering a role of this procedure
			{
				var source = result.SourceCode;
				// Annotate source code with line numbers
				{
					source = string.Join(Environment.NewLine, source.Split('\r').Select((x, i) => (i + 1).ToString("00") + "| " + x.Trim('\n')).ToArray());
				}
				var fullLogEntry = error.ErrorText + " at line " + error.Line + " col " + error.Column + "\r\n" + source;
				_ctx.BuildErrorLogger.LogOutputMessage(fullLogEntry);
			}
			return _metacreatorErrorPrefix + error.ErrorText;
		}

		static string BuildError_GetFile(ProcessFileCtx ctx, EvaluationResult result)
		{
			if (result.NonUserCode != null || !ctx.ErrorRemap)
			{
				result.NonUserCode = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(ctx.OriginalFileName) + ".meta" + Path.GetExtension(ctx.OriginalFileName));
				File.WriteAllText(result.NonUserCode, result.SourceCode);
				return result.NonUserCode;
			}
			return ctx.OriginalFileName;
		}

		ProcessFileCtx _ctx;
		IBuildErrorLogger _buildErrorLogger;

		void ProcessEvaluationResult(EvaluationResult result, ProcessFileCtx ctx)
		{

			// Log meta code compile time warnings
			if (result.Warnings != null)
			{
				foreach (var error in result.Warnings)
				{
					_buildErrorLogger.LogWarningEvent(CreateBuildWarning(result, error, ctx));
				}
			}

			var macrosFailed = false;

			// Log meta code compile time errors
			if (result.Errors != null)
			{
				foreach (var error in result.Errors)
				{
					macrosFailed = true;
					_buildErrorLogger.LogErrorEvent(CreateBuildError(result, error, ctx));
				}
			}

			// Log meta code run time exceptions
			if (result.EvaluationException != null)
			{
				macrosFailed = true;
				// var linenumber = result.EvaluationException.
				var message = result.EvaluationException.GetType().FullName + ": " + result.EvaluationException.Message;
				_buildErrorLogger.LogOutputMessage(message);
				var line = RemapErrorLineNumber(result.EvaluationExceptionAtLine, ctx, result);
				_buildErrorLogger.LogErrorEvent(new BuildErrorEventArgs(null, null, ctx.OriginalFileName, line, 0, 0, 0, _metacreatorErrorPrefix + message, null, null));
			}

			// terminate
			if (macrosFailed)
			{
				throw new FailBuildingException("$ terminating, jump to global catch and return false...");
			}
		}

		const string _metacreatorErrorPrefix = "MetaCode: ";


	}
}

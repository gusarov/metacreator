using System;
using System.Reflection;
using System.Threading;
using MetaCreator.Evaluation;
using System.Diagnostics;

namespace MetaCreator.AppDomainIsolation
{
	///<summary>
	/// Allows you to evaluate and run code in another app domain
	///</summary>
	class AnotherAppDomMarshalApi : MarshalByRefObject, IAnotherAppDomMarshalApi
	{
		readonly object _sync = new object();

		public EvaluationResult Evaluate(AnotherAppDomInputData input)
		{
			lock (_sync)
			{
				_input = input;
				var thread = new Thread(Body)
				{
					IsBackground = true,
					Name = "Metacode Evaluation Thread"
				};

				thread.Start();

				if (!thread.Join(input.Timeout))
				{
					if (!Debugger.IsAttached)
					{
						try
						{
							thread.Abort();
						}
						catch { }
						throw new Exception("Metacode Evaluation timeout");
					}
					else
					{
						thread.Join();
					}
				}
				if (_ex != null)
				{
					throw _ex;
				}
				return _result;
			}
		}

		EvaluationResult _result;
		AnotherAppDomInputData _input;
		Exception _ex;

		void Body()
		{
			_ex = null;
			try
			{
				var codeCompiler = new Code2Compiler();
				var result = codeCompiler.Compile(_input);

				if (result.IsSuccess && !_input.DoNotRun)
				{
					var codeRunner = new Code3Runer();
					codeRunner.Run(result, "Generator", "Run", _input);
				}

				_result = result;
			}
			catch (Exception ex)
			{
				_ex = ex;
			}
		}
	}

	public interface IAnotherAppDomMarshalApi
	{
		EvaluationResult Evaluate(AnotherAppDomInputData input);
	}
}
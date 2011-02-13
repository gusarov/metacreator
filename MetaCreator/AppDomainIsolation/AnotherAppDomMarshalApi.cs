using System;
using System.Reflection;

using MetaCreator.Evaluation;

namespace MetaCreator.AppDomainIsolation
{
	///<summary>
	/// Allows you to evaluate and run code in another app domain
	///</summary>
	public class AnotherAppDomMarshalApi : MarshalByRefObject, IAnotherAppDomMarshalApi
	{
		public EvaluationResult Evaluate(AnotherAppDomInputData input)
		{
			var codeCompiler = new Code2Compiler();
			var result = codeCompiler.Compile(input.Metacode, input.References);

			if (result.IsSuccess)
			{
				var codeRunner = new Code3Runer();
				codeRunner.Run(result);
			}

			return result;
		}
	}

	public interface IAnotherAppDomMarshalApi
	{
		EvaluationResult Evaluate(AnotherAppDomInputData input);
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using MetaCreator.AppDomainIsolation;
using MetaCreator.Utils;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Inmemory code executor. Consider to execute this in separate app domain.
	/// </summary>
	class Code3Runer
	{
		internal void Run(EvaluationResult evaluationResult)
		{
			evaluationResult.EnsureExistsDebug();
			evaluationResult.Assembly.EnsureExistsDebug();

			var type = evaluationResult.Assembly.GetType("Generator", true);
			type.EnsureExistsDebug("Generator method not found");

			var method = type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
			method.EnsureExistsDebug("Generator method not found");

			if (method.GetParameters().Count() != 0 || method.GetGenericArguments().Count() != 0)
			{
				throw new Exception("Method has unexpected parameters");
			}
			if (!method.IsStatic)
			{
				throw new Exception("Method is not static");
			}

			string returnedValue = null;
			// using (new Resolver(evaluationResult.))
			{
				try
				{
					returnedValue = (string)method.Invoke(null, null);
				}
				catch (TargetInvocationException ex)
				{
					evaluationResult.EvaluationException = ex.InnerException ?? ex;
				}
			}

			evaluationResult.ResultBody = returnedValue;
		}
	}
}

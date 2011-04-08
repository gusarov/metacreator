using System;

using MetaCreator.AppDomainIsolation;
using MetaCreator.Utils;

namespace MetaCreator
{
	public static class Evaluator
	{
		public static object EvaluateExpression(string expressionCode, string[] references = null, string cSharpVersion = null)
		{
			using(var appDom = AnotherAppDomFactory.AppDomainLiveScope())
			{
				var args = new AnotherAppDomInputData
				{
					Metacode = @"
static class Generator
{{
	public static object Run()
	{{
		return {0};
	}}
}}
".Arg(expressionCode),
					References = references,
				};
				if (cSharpVersion != null)
				{
					args.CSharpVersion = cSharpVersion;
				}
				var result = appDom.AnotherAppDomMarshal.Evaluate(args);
				if(!result.IsSuccess)
				{
					throw new Exception(result.CompileError + result.EvaluationException);
				}
				return result.ReturnedValue;
			}
		}
	}
}
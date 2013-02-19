using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MetaCreator.AppDomainIsolation;
using MetaCreator.Utils;

namespace MetaCreator
{
	public static class Evaluator
	{
		const string _defaultCsVersion = null; // based on executing CLR

		public static T EvaluateExpression<T>(string expressionCode, string[] references = null, string[] usings = null, string cSharpVersion = _defaultCsVersion)
		{
			return (T)EvaluateExpressionCore(typeof(T), expressionCode, references, usings, cSharpVersion);
		}

		public static object EvaluateExpression(string expressionCode, string[] references = null, string[] usings = null, string cSharpVersion = _defaultCsVersion)
		{
			return EvaluateExpressionCore(typeof(object), expressionCode, references, usings, cSharpVersion);
		}

		static object EvaluateExpressionCore(Type returnType, string expressionCode, string[] references = null, string[] usings = null, string cSharpVersion = _defaultCsVersion)
		{
			return EvaluateMethodBodyCore(returnType, "return " + expressionCode + ";", references, usings, cSharpVersion);
		}

		public static T EvaluateMethodBody<T>(string methodBody, string[] references = null, string[] usings = null, string cSharpVersion = _defaultCsVersion)
		{
			return (T)EvaluateMethodBodyCore(typeof(T), methodBody, references, usings, cSharpVersion);
		}

		public static object EvaluateMethodBody(string methodBody, string[] references = null, string[] usings = null, string cSharpVersion = _defaultCsVersion)
		{
			return EvaluateMethodBodyCore(typeof(object), methodBody, references, usings, cSharpVersion);
		}

		static object EvaluateMethodBodyCore(Type returnType, string methodBody, string[] references = null, IEnumerable<string> usings = null, string cSharpVersion = _defaultCsVersion)
		{
			return EvaluateFile(string.Format(@"class Generator
{{
{1}
	public object Run()
	{{
		{0}
	}}
}}", methodBody, usings.OrEmpty().Select(x => "	using " + x + ";").Join("\r\n")), references);
		}

		public static T EvaluateFile<T>(string fileContent, string[] references = null, string cSharpVersion = _defaultCsVersion)
		{
			return (T)EvaluateFile(fileContent, references, cSharpVersion);
		}

		/// <summary>
		/// Compile file, instantiate class, execute method and return a value. Class - the one or 'Generator', Method - the one or 'Run'.
		/// </summary>
		public static object EvaluateFile(string fileContent, string[] references = null, string cSharpVersion = _defaultCsVersion)
		{
			using (var appDom = AnotherAppDomFactory.AppDomainLiveScope())
			{
				var args = new AnotherAppDomInputData
				{
					Metacode = fileContent,
					References = references,
					CSharpVersion = cSharpVersion,
				};
				var result = appDom.AnotherAppDomMarshal.Evaluate(args);
				appDom.MarkDirectoryPathToRemoveAfterUnloadDomain(result.CompileTempPath);
				if (!result.IsSuccess)
				{
					throw new Exception(result.CompileError + result.EvaluationException);
				}
				return result.ReturnedValue;
			}
		}

		/// <summary>
		/// Compile any file and return an assembly
		/// </summary>
		public static Assembly CompileFile(string fileContent, string[] references = null, string cSharpVersion = _defaultCsVersion)
		{
			using (var appDom = AnotherAppDomFactory.AppDomainLiveScope())
			{
				var args = new AnotherAppDomInputData
				{
					Metacode = fileContent,
					References = references,
					CSharpVersion = cSharpVersion,
				};
				var result = appDom.AnotherAppDomMarshal.Evaluate(args);
				appDom.MarkDirectoryPathToRemoveAfterUnloadDomain(result.CompileTempPath);
				if (!result.IsSuccess)
				{
					throw new Exception(result.CompileError + result.EvaluationException);
				}
				return result.Assembly;
			}
		}

		public static Type CompileType(string typeDeclaration, string[] references = null, string cSharpVersion = _defaultCsVersion)
		{
			throw new NotImplementedException();
		}

		public static T CompileMethod<T>(string methodDeclaration, string[] references = null, string cSharpVersion = _defaultCsVersion)
		{
			throw new NotImplementedException();
		}

		public static MethodInfo CompileMethod(string methodDeclaration, string[] references = null, string cSharpVersion = _defaultCsVersion)
		{
			throw new NotImplementedException();
		}


	}
}
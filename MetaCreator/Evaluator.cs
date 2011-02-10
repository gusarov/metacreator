using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MetaCreator.Utils;
using Microsoft.CSharp;
using System.IO;

namespace MetaCreator
{
	public static class Evaluator
	{
		const string _generatorMethodName = "Run";

		const string _generatorResultPropertyName = "Result";

		/// <summary>
		/// Make this name unique to avoid collisions.
		/// </summary>
		static readonly string _generatorClassName = "Generator" + new Random().Next(1000, 9999);

		static readonly string[] _defaultUsings = new[] {
			"System",
			"System.CodeDom.Compiler",
			"System.Collections",
			"System.Collections.Generic",
			"System.Linq",
			"System.Reflection",
			"System.Runtime.CompilerServices",
			"System.Text.RegularExpressions",
			"System.Diagnostics",
			"System.IO",
			"System.Text",
			"Microsoft.CSharp",
		};

		static readonly Random _rnd = new Random();

		/// <summary>
		/// Create New Assembly inside current AppDomain, and execute an expression
		/// </summary>
		/// <param name="expression"></param>
		/// <returns>Expression result</returns>
		public static object EvaluateExpression(string expression)
		{
			return EvaluateExpression(expression, null, null);
		}

		/// <summary>
		/// Create New Assembly inside current AppDomain, and execute an expression
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="additionalReferences"></param>
		/// <param name="additionalNamespaces"></param>
		/// <returns>Expression result</returns>
		public static object EvaluateExpression(string expression, IEnumerable<string> additionalReferences, IEnumerable<string> additionalNamespaces)
		{
			var result = EvaluateExpressionCore(expression, additionalReferences, additionalNamespaces);
			if (!result.IsSuccess)
			{
				throw new Exception(result.CompileError ?? result.EvaluationException.Message);
			}
			return result.ResultBody;
		}

		/// <summary>
		/// Create New Assembly inside current AppDomain, and execute a code for method body
		/// </summary>
		/// <param name="code"></param>
		/// <returns>Expression result</returns>
		public static object EvaluateMethodBody(string code)
		{
			return EvaluateMethodBody(code, null, null);
		}

		/// <summary>
		/// Create New Assembly inside current AppDomain, and execute a code for method body
		/// </summary>
		/// <param name="code"></param>
		/// <param name="additionalReferences"></param>
		/// <param name="additionalNamespaces"></param>
		/// <returns>Expression result</returns>
		public static object EvaluateMethodBody(string code, IEnumerable<string> additionalReferences, IEnumerable<string> additionalNamespaces)
		{
			return EvaluateMethodBodyCore(code, "object", false, additionalReferences, additionalNamespaces);
		}

		static EvaluationResult EvaluateExpressionCore(string expression, IEnumerable<string> additionalReferences, IEnumerable<string> additionalNamespaces)
		{
			return EvaluateGeneratorCore(expression, true, additionalReferences, additionalNamespaces);
		}

		internal static EvaluationResult EvaluateGeneratorCore(string code)
		{
			return EvaluateGeneratorCore(code, false, null, null);
		}

		internal static EvaluationResult EvaluateGeneratorCore(string code, IEnumerable<string> additionalReferences, IEnumerable<string> additionalNamespaces)
		{
			return EvaluateGeneratorCore(code, false, additionalReferences, additionalNamespaces);
		}

		internal static EvaluationResult EvaluateGeneratorCore(string code, bool isExpression, IEnumerable<string> additionalReferences, IEnumerable<string> additionalNamespaces)
		{
			if (isExpression)
			{
				code = "return " + code + ";";
			}
			return EvaluateMethodBodyCore(code, isExpression ? "object" : "void", !isExpression,  additionalReferences, additionalNamespaces);
		}

		static EvaluationResult EvaluateMethodBodyCore(string code, string returnTypeName, bool useGeneratorResult, IEnumerable<string> additionalReferences, IEnumerable<string> additionalNamespaces)
		{

			var imports = additionalNamespaces.OrEmpty().Concat(_defaultUsings).Distinct();
			var result = Compile(GetCode(code, returnTypeName, imports), additionalReferences);
			var asm = result.Assembly;
			if (asm == null)
			{
				return result;
			}
			var type = asm.GetType(_generatorClassName, true, true);
			var method = type.GetMethod(_generatorMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
			method.EnsureExists("Generator method not found");
			var instance = Activator.CreateInstance(type);

			if (method.GetParameters().Count() != 0 || method.GetGenericArguments().Count() != 0)
			{
				throw new Exception("Method has unexpected parameters");
			}
			if (method.IsStatic)
			{
				throw new Exception("Method is static");
			}

			object returnedValue = null;
			using (new Resolver(additionalReferences))
			{
				try
				{
					returnedValue = method.Invoke(instance, null);
				}
				catch (TargetInvocationException ex)
				{
					result.EvaluationException = ex.InnerException ?? ex;
				}
			}

			if (useGeneratorResult)
			{
				var resultPi = type.GetField(_generatorResultPropertyName);
				resultPi.EnsureExists("Generator - Result field not found");
				var value = resultPi.GetValue(instance).ToString();
				result.ResultBody = value;
			}
			else
			{
				result.ResultBody = returnedValue;
			}
			return result;

		}

		static EvaluationResult Compile(string source, IEnumerable<string> additionalReferences)
		{
			additionalReferences = additionalReferences ?? Enumerable.Empty<string>(); 
			var result = new EvaluationResult {SourceCode = source};

			var metaTempPath = Path.Combine(Path.GetTempPath(), "MetaCreator");
			var tempPath = Path.Combine(metaTempPath, _rnd.Next().ToString());
			Directory.CreateDirectory(tempPath);

			var dtn = DateTime.UtcNow;
			foreach (var dir in new DirectoryInfo(metaTempPath).GetDirectories().Where(x => (dtn - x.CreationTimeUtc) > TimeSpan.FromDays(1)))
			{
				try
				{
					dir.Delete(true);
				}catch
				{
				}
			}

			result.CompileTempPath = tempPath;

			var options = new CompilerParameters
				{
					//GenerateInMemory = true,
					//IncludeDebugInformation = true,
					CompilerOptions = "/debug:full",
					TempFiles = new TempFileCollection(tempPath) { KeepFiles = true },
					MainClass = _generatorClassName,
				};
			foreach (var reference in typeof(Evaluator).Assembly.GetReferencedAssemblies())
			{
				options.ReferencedAssemblies.Add(reference.Name + ".dll");
			}
			foreach (var reference in additionalReferences ?? new string[0])
			{
				options.ReferencedAssemblies.Add(reference);
			}
			var compiler = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });

			var compilerResults = compiler.CompileAssemblyFromSource(options, source);

			if (compilerResults.Errors.HasWarnings)
			{
				result.Warnings = compilerResults.Errors.OfType<CompilerError>().Where(x => x.IsWarning).ToArray();
			}
			if (compilerResults.Errors.HasErrors)
			{
				var errors = compilerResults.Errors.OfType<CompilerError>().Where(x => !x.IsWarning).ToArray();
				result.Errors = errors;
				result.CompileError = errors[0] + Environment.NewLine + source;
				return result;
			}
			result.Assembly = compilerResults.CompiledAssembly;
			return result;
		}

		static string GetCode(string code, string returnType, IEnumerable<string> imports)
		{
			return
				string.Format(@"//
// This file is generated and compiled in-memory.
// If there are any compilation errors - file can be saved to temporary path for working with IDE.
//

{1}

public sealed class {5}
{{
	public StringBuilder {4} = new StringBuilder();

	public void Write(string msg, params object[] args)
	{{
		Result.AppendFormat(msg, args);
	}}

	public void WriteLine(string msg, params object[] args)
	{{
		Result.AppendFormat(msg + Environment.NewLine, args);
	}}

	public void Write(string msg)
	{{
		Result.Append(msg);
	}}

	public void WriteLine(string msg)
	{{
		Result.AppendLine(msg);
	}}

	public {3} {2}()
	{{
// <UserCode>
		{0}
// </UserCode>
	}}
}}

",
					code,
					string.Join(Environment.NewLine, imports.Select(x => "using " + x + ";").ToArray()),
						_generatorMethodName,
						returnType,
						_generatorResultPropertyName,
						_generatorClassName
					);
		}

		private sealed class Resolver : IDisposable
		{
			public Resolver(IEnumerable<string> additionalReferences)
			{
				_currentDomainAdditionalReferences = additionalReferences.OrEmpty();
				AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			}

			private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				var i = args.Name.IndexOf(',');
				var name = i > 0 ? args.Name.Substring(0, i).TrimEnd(',').Trim() : args.Name;
				foreach (var currentDomainAdditionalReference in _currentDomainAdditionalReferences)
				{
					if (Path.GetFileNameWithoutExtension(currentDomainAdditionalReference) == name)
					{
						return Assembly.LoadFile(currentDomainAdditionalReference);
					}
				}
				return null;
			}

			readonly IEnumerable<string> _currentDomainAdditionalReferences;

			void IDisposable.Dispose()
			{
				AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
			}
		}
	}
}
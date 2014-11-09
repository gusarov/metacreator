using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using MetaCreator.AppDomainIsolation;
using MetaCreator.Utils;

using Microsoft.CSharp;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Dynamic code compiler in separate app domain
	/// </summary>
	class Code2Compiler
	{
		public EvaluationResult Compile(AnotherAppDomInputData input)
		{
			var result = new EvaluationResult();
			var source = input.Metacode;
			var references = new List<string>(input.References ?? Enumerable.Empty<string>());
			var cSharpVersion = input.CSharpVersion;

			if (cSharpVersion == null)
			{
				// It is very common (from current runtime version). It should be done based on currently compiling project version
				if (Environment.Version.Major >= 4)
				{
					cSharpVersion = "v4.0";
				}
				else
				{
					cSharpVersion = "v3.5";
				}
				result.DebugLog += "Automatic CSharpVersion using CLR Version = " + cSharpVersion + Environment.NewLine;
			}

			result.DebugLog += "cSharpVersion = " + cSharpVersion + Environment.NewLine;

			var tempPath = TempFiles.GetNewTempFolder();

			using (var tempFiles = new TempFileCollection(tempPath, true))
			{
				var options = new CompilerParameters
				{
					// GenerateInMemory = true,
					IncludeDebugInformation = true,
					CompilerOptions = "/debug:full /optimize-",
					TempFiles = tempFiles,
					// MainClass = _generatorClassName,
				};
				if (!string.IsNullOrWhiteSpace(input.MetaAssemblyName))
				{
					options.OutputAssembly = input.MetaAssemblyName;
				}

				foreach (var reference in references)
				{
					options.ReferencedAssemblies.Add(reference);
				}
				result.References = references.ToArray();
				result.SourceCode = source;
				result.CompileTempPath = tempPath;

				using (var compiler = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", cSharpVersion } }))
				{
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
					//var path = result.Assembly.Location;
					//var ini = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".ini");
//					File.WriteAllText(ini, @"[.NET Framework Debugging Control] 
//GenerateTrackingInfo=1 
//AllowOptimize=0 ");
					//Thread.Sleep(5000);
					return result;
				}
			}
		}

	}
}
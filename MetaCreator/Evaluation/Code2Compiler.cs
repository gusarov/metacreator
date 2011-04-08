using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

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


			string source = input.Metacode;
			string[] references = input.References ?? new string[0];
			string cSharpVersion = input.CSharpVersion;

			if(cSharpVersion == null)
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

			var metaTempPath = Path.Combine(Path.GetTempPath(), "MetaCreator");
			var tempPath = Path.Combine(metaTempPath, Ext.GenerateId());
			Directory.CreateDirectory(tempPath);

			var dtn = DateTime.UtcNow;
			foreach (var dir in new DirectoryInfo(metaTempPath).GetDirectories().Where(x => (dtn - x.CreationTimeUtc) > TimeSpan.FromDays(1)))
			{
				try
				{
					dir.Delete(true);
				}
				catch
				{
				}
			}

			using (var tempFiles = new TempFileCollection(tempPath))
			{
				tempFiles.KeepFiles = true;
				var options = new CompilerParameters
				{
					//GenerateInMemory = true,
					//IncludeDebugInformation = true,
					CompilerOptions = "/debug:full",
					TempFiles = tempFiles,
					//MainClass = _generatorClassName,
				};

				foreach (var reference in references)
				{
					options.ReferencedAssemblies.Add(reference);
				}

				result.References = references;
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
					return result;
				}
			}
		}

	}
}
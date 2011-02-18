using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

using MetaCreator.Utils;

using Microsoft.CSharp;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Dynamic code compiler in separate app domain
	/// </summary>
	class Code2Compiler
	{
		public EvaluationResult Compile(string source, IEnumerable<string> additionalReferences)
		{
			additionalReferences = additionalReferences ?? Enumerable.Empty<string>();
			var result = new EvaluationResult { SourceCode = source, AdditionalReferences = additionalReferences.ToArray() };

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

			result.CompileTempPath = tempPath;

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


				var alreadyReferencedNames = new List<string>(16);
				foreach (var reference in typeof(Code2Compiler).Assembly.GetReferencedAssemblies())
				{
					alreadyReferencedNames.Add(reference.Name + ".dll");
					options.ReferencedAssemblies.Add(reference.Name + ".dll");
				}
				foreach (var reference in additionalReferences)
				{
					if (!alreadyReferencedNames.Contains(Path.GetFileName(reference), StringComparer.InvariantCultureIgnoreCase))
					{
						options.ReferencedAssemblies.Add(reference);
					}
				}

				result.ReferencesUsed = options.ReferencedAssemblies.Cast<string>().ToArray();
				using (var compiler = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } }))
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{
	[TestClass]
	public abstract class Acceptance_base_tests
	{
		protected Assembly LoadAssembly(string fileName = "sample")
		{
//			if (fileName == null)
//			{
//				var fileNameDll = "sample.dll";
//				var fileNameExe = "sample.exe";
//				if (File.Exists(fileNameDll))
//				{
//					fileName = fileNameDll;
//				}
//				else if (File.Exists(fileNameExe))
//				{
//					fileName = fileNameExe;
//				}
//				else
//				{
//					throw new Exception("File not found");
//				}
//			}
//			if(!File.Exists(fileName))
//			{
//				throw new Exception("File not found");
//			}

			return Assembly.Load(Path.GetFileNameWithoutExtension(fileName)/*Path.GetFullPath(fileName)*/);
		}

		[TestInitialize]
		public void Error_handling_base_tests_Init()
		{
			KillCs();

			netFxPath = Path.Combine(Environment.GetEnvironmentVariable("WinDir"), @"Microsoft.NET\Framework\v3.5\");
			msBuildPath = Path.Combine(netFxPath, "msbuild.exe");

			CreateTargets();
		}

		protected static void KillCs()
		{
			foreach (var csFile in new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.cs"))
			{
				csFile.IsReadOnly = false;
				csFile.Delete();
			}
		}

		static void CreateTargets()
		{
			File.WriteAllText("sample.targets", @"
<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
	<PropertyGroup>
		<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>
		<AssemblyName>{0}</AssemblyName>
		<OutputType>{1}</OutputType>
		<OutputPath>.\</OutputPath>
	</PropertyGroup>
	<ItemGroup>
		<Compile Include='*.cs' />
{3}
	</ItemGroup>
	<ItemGroup>
{2}
	</ItemGroup>
	<Import Project='$(MSBuildToolsPath)\Microsoft.CSharp.targets' />
	<Import Project='bin\Debug\MetaCreator.targets' Condition=""Exists('bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\..\bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\..\..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\..\..\bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\..\..\..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\..\..\..\bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\..\..\..\..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\..\..\..\..\bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\..\..\..\..\..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\..\..\..\..\..\bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\..\..\..\..\..\..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\..\..\..\..\..\..\bin\Debug\MetaCreator.targets')""/>
	<Import Project='..\..\..\..\..\..\..\..\bin\Debug\MetaCreator.targets' Condition=""Exists('..\..\..\..\..\..\..\..\bin\Debug\MetaCreator.targets')""/>
</Project>");
		}

		string netFxPath;
		string msBuildPath;
		protected string _output;

		public class Params
		{
			public bool IsExpectedSuccess = true, IsExe;
			public string AssemblyName = "sample";
			public string[] References;
			public string[] Compile;
		}

		public static IEnumerable<T> OrEmpty<T>(IEnumerable<T> source)
		{
			return source ?? Enumerable.Empty<T>();
		}

		protected void Build()
		{
			Build(new Params());
		}

		protected void Build(Params options)
		{
			var refs = string.Join("\r\n", (options.References ?? Enumerable.Empty<string>()).Select(x => string.Format(@"	<Reference Include=""{0}"">
		<HintPath>{1}</HintPath>
	</Reference>", Path.GetFileNameWithoutExtension(x), x)));

			var compile = string.Join("\r\n", (options.Compile ?? Enumerable.Empty<string>()).Select(x => string.Format(@"	<Compile Include=""{0}"" />", x)));

			CreateTargets();
			File.WriteAllText("sample.targets", string.Format(File.ReadAllText("sample.targets"), options.AssemblyName ?? "sample", options.IsExe? "Exe" : "Library", refs, compile);
			Run(msBuildPath, "/nologo /clp:errorsonly /fl /flp:Verbosity=minimal sample.targets", options.IsExpectedSuccess);
		}

		protected void Build(string asmName = null, params string[] references)
		{
			Build(new Params
			{
				AssemblyName = asmName,
				References = references,
			});
		}

		protected void RunMsbuild(bool expectedSuccess, bool outputExe = false, string asmName = null, params string[] references)
		{
			Build(new Params
			{
				IsExe = outputExe,
				IsExpectedSuccess = expectedSuccess,
				AssemblyName = asmName,
				References = references,
			});
		}

		protected void Run(string prog, string arg, bool expectedSuccess)
		{
			var p = Process.Start(new ProcessStartInfo
				{
					FileName = prog,
					Arguments = arg,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				});

			bool timeOut = false;
			if (!p.WaitForExit(15000))
			{
				try
				{
					p.Kill();
				}
				catch
				{
				}
				timeOut = true;
			}

			_output = p.StandardOutput.ReadToEnd();
			_error = p.StandardError.ReadToEnd();
			Console.WriteLine("STDOUT:\r\n" + _output);
			Console.WriteLine("===");
			Console.WriteLine("STDERR:\r\n" + _error);
			Console.WriteLine("===");

			if (timeOut)
			{
				Assert.Fail("Time Out");
			}

			if (expectedSuccess)
			{
				Assert.AreEqual(0, p.ExitCode, _output + _error);
			}
			else
			{
				Assert.AreNotEqual(0, p.ExitCode, _output + _error);
			}
		}

		readonly Regex _rxMsbuildError = new Regex(@"(?imx)^(?'fn'[^(]+)
(
	\(
		(
			(?'L'\d+),(?'C'\d+)
		|
			(?'L'\d+)
		)
	\)
)?
\s*:\s*error.*");

		string _parsedMsBuildErrorSource;
		Match _parsedMsBuildError;
		protected string _error;

		protected class ParsedError
		{
			public ParsedError(Match match)
			{
				Match = match;
			}
			public readonly Match Match;
			public int Line;
			public int Column;
			public string FileName;
		}

		protected ParsedError ParsedMsBuildError
		{
			get
			{
				Assert.IsNotNull(_output);
				if (_parsedMsBuildErrorSource != _output)
				{
					_parsedMsBuildError = _rxMsbuildError.Match(_output);
					if (!_parsedMsBuildError.Success)
					{
						Assert.Fail("Can not parse: " + _output);
					}
					_parsedMsBuildErrorSource = _output;
				}
				return new ParsedError(_parsedMsBuildError)
					{
						FileName = _parsedMsBuildError.Groups["fn"].Value.Trim(),
						Line = int.Parse(_parsedMsBuildError.Groups["L"].Value.Trim() == string.Empty ? "0" : _parsedMsBuildError.Groups["L"].Value),
						Column = int.Parse(_parsedMsBuildError.Groups["C"].Value.Trim() == string.Empty ? "0" : _parsedMsBuildError.Groups["C"].Value),
					};
			}
		}
	}
}
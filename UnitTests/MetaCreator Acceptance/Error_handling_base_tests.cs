using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;

using MetaCreator;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{
	[TestClass]
	public abstract class Acceptance_base_tests
	{
		protected TimeSpan ProcessExecutionTimeout = TimeSpan.FromSeconds(5);

		protected Assembly LoadAssembly(string fileName = null)
		{
			if (fileName == null)
			{
				var fileNameDll = SampleAssemblyName+".dll";
				var fileNameExe = SampleAssemblyName+".exe";
				if (File.Exists(fileNameDll))
				{
					fileName = fileNameDll;
				}
				else if (File.Exists(fileNameExe))
				{
					fileName = fileNameExe;
				}
			}
			fileName = SuggestFile(fileName);
			if(!File.Exists(fileName))
			{
				throw new Exception("File not found");
			}

			return Assembly.LoadFrom(fileName);
		}

		static string _originalCd;

		[TestInitialize]
		public void Error_handling_base_tests_Init()
		{
			sampleAssemblyNameIndex++;
			if (_originalCd == null)
			{
				_originalCd = Directory.GetCurrentDirectory();
			}

			var d = Path.Combine(_originalCd, Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(d);
			Directory.SetCurrentDirectory(d);

			// KillCs();

			netFxPath = Path.Combine(Environment.GetEnvironmentVariable("WinDir"), @"Microsoft.NET\Framework\v3.5\");
			msBuildPath = Path.Combine(netFxPath, "msbuild.exe");

			CreateTargets();
		}

		static int sampleAssemblyNameIndex;

		string SampleAssemblyName
		{
			get { return "Sample_" + sampleAssemblyNameIndex; }
		}

		protected static void KillCs()
		{
			foreach (var csFile in new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.cs"))
			{
				csFile.IsReadOnly = false;
				csFile.Delete();
			}
		}

		protected static void Clear()
		{
			try
			{
				foreach (var csDir in new DirectoryInfo(Directory.GetCurrentDirectory()).GetDirectories())
				{
					csDir.Delete(true);
				}
				foreach (var csFile in new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.*"))
				{
					csFile.IsReadOnly = false;
					csFile.Delete();
				}
			}catch{
			}
		
		}

		static void CreateTargets()
		{
			File.WriteAllText("sample.targets", @"
<Project ToolsVersion='3.5' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
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
		<Reference Include='System' />
		<Reference Include='System.Core' />
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
			public string AssemblyName;
			public List<string> References = new List<string>();
			public List<string> Compile = new List<string>();
		}

		public static IEnumerable<T> OrEmpty<T>(IEnumerable<T> source)
		{
			return source ?? Enumerable.Empty<T>();
		}

		protected void Build()
		{
			Build(new Params());
		}

		protected void BuildExe()
		{
			Build(new Params {IsExe = true});
		}

		protected void Build(Params options)
		{
			options.References.Add(typeof(IMetaWriter).Assembly.Location);
			var refs = string.Join("\r\n", (options.References).Select(x => string.Format(@"		<Reference Include='{0}'>
			<HintPath>{1}</HintPath>
		</Reference>", Path.GetFileNameWithoutExtension(x), SuggestFile(Path.GetFullPath(x)))));

			var compile = string.Join("\r\n", (options.Compile ?? Enumerable.Empty<string>()).Select(x => string.Format(@"		<Compile Include='{0}' />", x)));

			CreateTargets();
			File.WriteAllText("sample.targets", string.Format(File.ReadAllText("sample.targets"), Path.GetFileNameWithoutExtension(options.AssemblyName) ?? SampleAssemblyName, options.IsExe ? "Exe" : "Library", refs, compile));
			Run(msBuildPath, "/nologo /clp:errorsonly /fl /flp:Verbosity=minimal sample.targets", options.IsExpectedSuccess);
		}

		static string SuggestFile(string x)
		{
			var t = Path.GetFullPath(Path.GetFileName(x)); // for local MetaCreator.dll
			if (File.Exists(t))
			{
				return t;
			}
			if (File.Exists(x))
			{
				return x;
			}
			t = x + ".dll";
			if (File.Exists(t))
			{
				return t;
			}
			t = x + ".exe";
			if (File.Exists(t))
			{
				return t;
			}
			throw new Exception("File not found");
		}

		protected void Build(string asmName, params string[] references)
		{
			Build(new Params
			{
				IsExe = (asmName??"").EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase),
				AssemblyName = asmName,
				References = references.ToList(),
			});
		}

		protected void RunMsbuild(bool expectedSuccess, bool outputExe = false, string asmName = null, params string[] references)
		{
			Build(new Params
			{
				IsExe = outputExe,
				IsExpectedSuccess = expectedSuccess,
				AssemblyName = asmName,
				References = references.ToList(),
			});
		}

		protected void Run(string prog = null, string arg = "", bool expectedSuccess = true)
		{
			if (prog == null)
			{
				prog = SampleAssemblyName + ".exe";
			}

			var p = Process.Start(new ProcessStartInfo
				{
					FileName = prog,
					Arguments = arg,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true,
					ErrorDialog = false,
				});

			_output = p.StandardOutput.ReadToEnd();
			_error = p.StandardError.ReadToEnd();

			bool timeOut = false;
			if (!p.WaitForExit((int)ProcessExecutionTimeout.TotalMilliseconds))
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

			_output += p.StandardOutput.ReadToEnd();
			_error += p.StandardError.ReadToEnd();
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

		/// <summary>
		/// Assert that actual source string matches to expected wildcard pattern
		/// </summary>
		/// <param name="actualSource">actual source string</param>
		/// <param name="expectedPattern">wildcard *</param>
		protected void Matches(string actualSource, string expectedPattern)
		{
			StringAssert.Matches(actualSource, new Regex(Regex.Escape(expectedPattern).Replace("\\*", ".*")));
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
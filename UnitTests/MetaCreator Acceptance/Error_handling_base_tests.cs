using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance
{
	[TestClass]
	public abstract class Error_handling_base_tests
	{

		[TestInitialize]
		public void Error_handling_base_tests_Init()
		{
			netFxPath = Path.Combine(Environment.GetEnvironmentVariable("WinDir"), @"Microsoft.NET\Framework\v3.5\");
			msBuildPath = Path.Combine(netFxPath, "msbuild.exe");

			File.WriteAllText("sample.targets", @"
<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
	<PropertyGroup>
		<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>
	</PropertyGroup>
	<ItemGroup>
		<Compile Include='sample.cs' />
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

		protected void RunMsbuild(bool expectedSuccess)
		{
			Run(msBuildPath, "/nologo /clp:errorsonly sample.targets", expectedSuccess);
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
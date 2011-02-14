using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MetaCreator.Utils;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Builder of metacode for execution
	/// </summary>
	class Code1Builder
	{

		const string _skeleton = @"// METACODE
// This file is generated and compiled in-memory.
// If there are any compilation errors - file can be saved to temporary path for working with IDE.

// imports
{0}

public static class Generator
{{
	public static string Run()
	{{
		// <methodbody>
		{1}
		// </methodbody>
		return Result.ToString();
	}}

	// <classbody>
{2}
	// </classbody>

	#region utils

	public static StringBuilder Result = new StringBuilder();

	public static void Write(string msg, params object[] args)
	{{
		Result.AppendFormat(msg, args);
	}}

	public static void WriteLine(string msg, params object[] args)
	{{
		Result.AppendFormat(msg + Environment.NewLine, args);
	}}

	public static void Write(string msg)
	{{
		Result.Append(msg);
	}}

	public static void WriteLine(string msg)
	{{
		Result.AppendLine(msg);
	}}

	public static void Write(object obj)
	{{
		Result.Append(obj == null ? string.Empty : obj.ToString());
	}}

	public static void WriteLine(object obj)
	{{
		Result.AppendLine(obj == null ? string.Empty : obj.ToString());
	}}

	#endregion
}}";

		const string _generatorMethodName = "Run";
		const string _generatorClassName = "Generator";

		static readonly string[] _defaultUsings = new[] {
		                                                	"global::Microsoft.Win32",
		                                                	"global::System",
		                                                	"global::System.Collections",
		                                                	"global::System.Collections.Generic",
		                                                	"global::System.Collections.Specialized",
		                                                	"global::System.Collections.ObjectModel",
		                                                	"global::System.Configuration.Assemblies",
		                                                	"global::System.ComponentModel",
		                                                	"global::System.Diagnostics",
		                                                	"global::System.Diagnostics.CodeAnalysis",
		                                                	"global::System.Globalization",
		                                                	"global::System.IO",
		                                                	"global::System.IO.Compression",
		                                                	"global::System.IO.Ports",
		                                                	"global::System.Media",
		                                                	"global::System.Net",
		                                                	"global::System.Net.Sockets",
		                                                	"global::System.Reflection",
		                                                	"global::System.Resources",
		                                                	"global::System.Runtime.CompilerServices",
		                                                	"global::System.Runtime.InteropServices",
		                                                	"global::System.Runtime.InteropServices.ComTypes",
		                                                	"global::System.Runtime.Serialization",
		                                                	"global::System.Runtime.Serialization.Formatters.Binary",
		                                                	"global::System.Security",
		                                                	"global::System.Security.Cryptography",
		                                                	"global::System.Security.Cryptography.X509Certificates",
		                                                	"global::System.Security.Permissions",
		                                                	"global::System.Security.Policy",
		                                                	"global::System.Text",
		                                                	"global::System.Text.RegularExpressions",
		                                                	"global::System.Threading",
		                                                	"global::System.CodeDom.Compiler",
		                                                	"global::System.Linq",
		                                                };


		readonly StringBuilder _methodBody = new StringBuilder();
		readonly StringBuilder _classBody = new StringBuilder();

		void PlainTextWriter(string substring, StringBuilder sb, int line)
		{
			sb.Append("WriteLine(@\"");
			sb.Append("#line " + line + " \"\"" + _ctx.GetOriginalFileNameRelativeToIntermediatePath() + "\"\"");
			sb.AppendLine("\");");

			sb.Append("WriteLine(@\"");
			sb.Append(substring.Replace("\"", "\"\""));
			sb.AppendLine("\");");

			sb.AppendLine(@"WriteLine(""#line default"");");
		}

//		static string OriginalLinePredirrectiveAtStartOfMacroBlock(ProcessFileCtx _ctx)
//		{
			//			var banner = (_ctx.GenerateBanner) ? @"// <MetaCreator>
//			//// DO NOT MODIFY THIS GENERATED CODE
//			//// You can use /*@ GenerateBanner off */ to disable this message
			//" : null;
//			string lineRemap = null;
//			if (_ctx.ErrorRemap)
//			{
//				lineRemap = "#line default";
//			}
//			return CombineLines(lineRemap, banner);
//		}

//		static string CombineLines(params string[] lines)
//		{
//			if (lines.Length == 2)
//			{
//				var line1 = lines[0];
//				var line2 = lines[1];
//				if (line1 != null && line1[line1.Length - 1] != Environment.NewLine[Environment.NewLine.Length - 1])
//				{
//					line1 += Environment.NewLine;
//				}
//				return line1 + line2;
//			}
//			if (lines.Length < 2)
//			{
//				throw new Exception();
//			}
//			var linea = lines[0];
//			foreach (var line in lines.Skip(1))
//			{
//				linea = CombineLines(linea, line);
//			}
//			return linea;
//		}

//		static string OriginalLinePredirrectiveAtEndOfMacroBlock(ProcessFileCtx _ctx)
//		{
//			var banner = (_ctx.GenerateBanner) ? @"// </MetaCreator>" : null;
//			string lineRemap = null;
//			if (_ctx.ErrorRemap)
//			{
//				lineRemap = "#line {0} \"{1}\"".Arg(_ctx.CurrentMacrosEndLineInOriginalFile + 1, _ctx.GetOriginalFileNameRelativeToIntermediatePath());
//			}
//			return CombineLines(banner, lineRemap);
//		}

//		string GetCode(string methodBody, string classBody, string returnType, IEnumerable<string> imports)
//		{
			//							var result = Evaluate(match.Groups[1].Value, false, _ctx);
//							if (!Environment.NewLine.Contains(match.Groups["p"].Value))
//							{
//								result = Environment.NewLine + result;
//							}
//							return result;
//							return Evaluate(match.Groups[1].Value, true, _ctx);
//
//
//			return
//				string.Format(@"
//
//",
//				              string.Join(Environment.NewLine, imports.Select(x => "using " + x + ";").ToArray()),
//				              _generatorClassName,
//				              methodBody,
//				              _generatorMethodName,
//				              returnType,
//				              _generatorResultPropertyName,
//					);
//		}

		static readonly Regex _rxBlock = new Regex(@"(?s)(?<=(?'p'.)?)/\*(?'type'[@+=!])(.+?)\*/");

		ProcessFileCtx _ctx;

		public string Build(string code, ProcessFileCtx ctx)
		{
			_ctx = ctx;

			// auto import same namespaces
			_ctx.NamespaceImportsOriginal = Regex.Matches(code, @"(?m)^using\s+([^;]+)").Cast<Match>().Select(x => x.Groups[1].Value).ToArray();

			// @ dirrectives
			_ctx.FileProcessedContent = _rxBlock.Replace(code, match =>
			{
				var type = match.Groups["type"].Value[0];
				switch (type)
				{
					case '@':
						_ctx.MarkMacrosAndSaveCaptureState(match);
						Extenders.ExecuteExtender(match.Groups["1"].Value, _ctx);
						return null;
					case '=':
					case '!':
					case '+':
						// handle that later
						return match.Value;
					default:
						throw new Exception("MetaBlock type '{0}' unknown".Arg(type));
				}
			});

			var blocks = _rxBlock.Matches(_ctx.FileProcessedContent).Cast<Match>().ToArray();

			int from = 0;

			// create metacode
			foreach (var block in blocks)
			{
				_ctx.MarkMacrosAndSaveCaptureState(block);

				var type = block.Groups["type"].Value[0];
				var value = block.Groups[1].Value;

				// write previous text as plain
				PlainTextWriter(code.Substring(from, block.Index - from), _methodBody, _ctx.GetLineNumberInOriginalFileByIndex(from));

				from = block.Index + block.Length;
				switch (type)
				{
					case '!':
						_methodBody.AppendLine("#line " + _ctx.CurrentMacrosLineInOriginalFile + ' ' + '"' + _ctx.GetOriginalFileNameRelativeToIntermediatePath() + '"');
						_methodBody.AppendLine(value);
						_methodBody.AppendLine("#line default");
						break;
					case '=':
						_methodBody.AppendLine("Write(" + value + ");");
						break;
					case '+':
						_classBody.AppendLine("#line " + _ctx.CurrentMacrosLineInOriginalFile + ' ' + '"' + _ctx.GetOriginalFileNameRelativeToIntermediatePath() + '"');
						_classBody.AppendLine(value);
						_classBody.AppendLine("#line default");
						break;
					default:
						throw new Exception("Block with type " + type + " unexpected");
				}
			}

			PlainTextWriter(code.Substring(from), _methodBody, _ctx.GetLineNumberInOriginalFileByIndex(from));


			var imports = _ctx.Namespaces.OrEmpty().Concat(_defaultUsings).Distinct();
			var importsAsString = string.Join(Environment.NewLine, imports.Select(x => "using " + x + ";").ToArray());
			var metacode = _skeleton.Arg(importsAsString, _methodBody, _classBody);

			return metacode;
		}

		
	}
}
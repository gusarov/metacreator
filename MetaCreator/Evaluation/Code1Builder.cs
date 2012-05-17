using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MetaCreator.Utils;
using Microsoft.Build.Framework;
using MetaCreator.Properties;
using System.Diagnostics;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Builder of metacode for execution
	/// </summary>
	class Code1Builder
	{
		ProcessFileCtx _ctx;

		public Code1Builder()
		{
			_mapExtenders = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"StringInterpolation", StringInterpolation},
				{"ErrorRemap", ErrorRemap},
				{"reference", Reference},
				{"assembly", ReferenceT4},
				{"using", Using},
				{"import", UsingT4},
				{"GenerateBanner", GenerateBanner},
				{"CSharpVersion", CSharpVersion},
				{"Timeout", Timeout},
				{"FileExtension", FileExtension},
				{"Comment", Comment},
				{"FileInProject", FileInProject},
				{"Convert", Convert},
			};
		}

		void Convert(string value)
		{
			const string formatT4 = "<#";
			const string formatMC = "/*";
			var body = File.ReadAllText(_ctx.OriginalRelativeFileName);
			var bodyOriginal = body;
			var from = Path.GetExtension(_ctx.OriginalRelativeFileName).Equals(".tt", StringComparison.InvariantCultureIgnoreCase) ? formatT4 : formatMC;
			var to = from == formatT4 ? formatMC : formatT4;

			#region Replace

			body = body.Replace(from + "#", to + "#");
			body = body.Replace(from + "@", to + "@");
			body = body.Replace(from + "+", to + "+");
			body = body.Replace(from + "=", to + "=");
			if (from == formatT4)
			{
				body = body.Replace(from + "", to + "!");
			}
			else
			{
				body = body.Replace(from + "!", to + "");
			}

			// closing tags
			body = body.Replace(ClosingTag(from), ClosingTag(to));

			#endregion

			var backupFile = _ctx.OriginalRelativeFileName + "_" + new Random().Next() + ".bak";
			File.WriteAllText(backupFile, bodyOriginal);

			if ((File.GetAttributes(_ctx.OriginalRelativeFileName) & FileAttributes.ReadOnly) != 0)
			{
				File.SetAttributes(_ctx.OriginalRelativeFileName, File.GetAttributes(_ctx.OriginalRelativeFileName) & ~FileAttributes.ReadOnly);
			}
			File.WriteAllText(_ctx.OriginalRelativeFileName, body);
		}

		string ClosingTag(string tag)
		{
			switch (tag.ToLowerInvariant())
			{
				case "/*":
					return "*/";
				case "<#":
					return "#>";
				default:
					throw new ArgumentException("Unknown Tag");
			}
		}

		void FileInProject(string name)
		{
			_ctx.FileInProject = true;
			if (!string.IsNullOrEmpty(name))
			{
				_ctx.ReplacementFileName = name;
			}
		}

		void Comment(string value)
		{
			
		}

		void FileExtension(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("FileExtension not specified", "value");
			}
			if (string.IsNullOrEmpty(value.Trim()))
			{
				throw new ArgumentException("FileExtension not specified", "value");
			}
			// ensure start with dot
			if (value[0] != '.')
			{
				value = '.' + value;
			}
			_ctx.ReplacementExtension = value;
		}

		void Timeout(string value)
		{
			_ctx.Timeout = TimeSpan.Parse(value);
		}

		void CSharpVersion(string version)
		{
			_ctx.CSharpVersion = version;
		}

		readonly static string _skeleton = Resources._GeneratorSkeleton;

		static readonly string[] _defaultUsings = new[]
		{
			"MetaCreator",
			"MetaCreator.Extensions",
			"Microsoft.Win32",
			"System",
			"System.Collections",
			"System.Collections.Generic",
			"System.Collections.Specialized",
			"System.Collections.ObjectModel",
			"System.Configuration.Assemblies",
			"System.ComponentModel",
			"System.Diagnostics",
			"System.Diagnostics.CodeAnalysis",
			"System.Globalization",
			"System.IO",
			"System.IO.Compression",
			"System.IO.Ports",
			"System.Media",
			"System.Net",
			"System.Net.Sockets",
			"System.Reflection",
			"System.Resources",
			"System.Runtime.CompilerServices",
			"System.Runtime.InteropServices",
			"System.Runtime.InteropServices.ComTypes",
			"System.Runtime.Serialization",
			"System.Runtime.Serialization.Formatters.Binary",
			"System.Security",
			"System.Security.Cryptography",
			"System.Security.Cryptography.X509Certificates",
			"System.Security.Permissions",
			"System.Security.Policy",
			"System.Text",
			"System.Text.RegularExpressions",
			"System.Threading",
			"System.CodeDom.Compiler",
			"System.Linq",
		};

		readonly StringBuilder _methodBody = new StringBuilder();
		readonly StringBuilder _classBody = new StringBuilder();

		// write string from original file to resulting file by metacode
		void PlainTextWriter(string substring, StringBuilder sb, int line = 0)
		{
			if (line > 0 && _errorRemap)
			{
				sb.AppendLine("WriteLine();");
				sb.Append("WriteLine(@\"");
				sb.Append("#line " + line + " \"\"" + _ctx.GetOriginalFileNameRelativeToIntermediatePath() + "\"\"");
				sb.AppendLine("\");");
			}

			sb.Append("Write(@\"");
			sb.Append(substring.Replace("\"", "\"\""));
			sb.AppendLine("\");");

			if (line > 0 && _errorRemap)
			{
				sb.AppendLine("WriteLine();");
				sb.AppendLine(@"WriteLine(""#line default"");");
			}
		}

		internal class BlockParser
		{
			protected Regex Rx;

			public static BlockParser GetBlockParser(ProcessFileCtx ctx)
			{
				if (ctx.OriginalRelativeFileName.EndsWith(".tt", StringComparison.InvariantCultureIgnoreCase))
				{
					return new T4BlockParser();
				}
				return new CommentBlockParser();
			}

			public IEnumerable<Block> Parse(string data)
			{
				return Rx.Matches(data).Cast<Match>().Select(x => new Block(this, x));
			}


			internal enum BlockType
			{
				Unknown,
				GenMethodBody, // !
				GenExpressionBody, // =
				GenClassBody, // +
				Extensibility, // @
				ExtensionMethod, // #
			}

			internal class Block
			{
				readonly BlockParser _parser;
				readonly Match _match;
				public readonly int Index;
				public readonly int Length;

				public Block(BlockParser parser, Match match)
				{
					_parser = parser;
					_match = match;
					Index = match.Index;
					Length = match.Length;
				}

				public BlockType Type
				{
					get { return _parser.GetBlockType(_match); }
				}

				public string Body
				{
					get { return _parser.GetBlockBody(_match); }
				}

				public string Pre
				{
					get { return _parser.GetBlockPre(_match); }
				}

			}

			protected virtual BlockType GetBlockType(Match match)
			{
				var suc = match.Groups["type"].Success;
				var val = match.Groups["type"].Value;
				return GetBlockTypeFromChar(suc ? (val.Length > 0 ? val[0] : default(char)) : default(char));
			}

			protected virtual BlockType GetBlockTypeFromChar(char c)
			{
				switch (c)
				{
					case '#':
						return BlockType.ExtensionMethod;
					case '@':
						return BlockType.Extensibility;
					case '!':
						return BlockType.GenMethodBody;
					case '=':
						return BlockType.GenExpressionBody;
					case '+':
						return BlockType.GenClassBody;
					default:
						throw new NotSupportedException();
				}
			}

			protected virtual string GetBlockBody(Match match)
			{
				return match.Groups["body"].Value;
			}

			protected virtual string GetBlockPre(Match match)
			{
				return match.Groups["pre"].Value;
			}

			class CommentBlockParser : BlockParser
			{
				public CommentBlockParser()
				{
					Rx = new Regex(@"(?sm)(?<=(?'pre'^.*?)?)/\*(?'type'[#@+=!])(?'body'.*?)\*/");
				}
			}

			class T4BlockParser : BlockParser
			{
				public T4BlockParser()
				{
					Rx = new Regex(@"(?sm)(?<=(?'pre'^.*?)?)<#(?'type'[#@+=])?(?'body'.*?)#>");
				}

				protected override BlockType GetBlockTypeFromChar(char c)
				{
					switch (c)
					{
						case '#':
							return BlockType.ExtensionMethod;
						case '@':
							return BlockType.Extensibility;
						case '!':
						case ' ':
						case default(char):
							return BlockType.GenMethodBody;
						case '=':
							return BlockType.GenExpressionBody;
						case '+':
							return BlockType.GenClassBody;
						default:
							throw new NotSupportedException("Type = " + c + " - " + (int)c);
					}
				}
			}
		}

		string[] _namespacesFromOriginalFile;
		string _outerNamespaceFromOriginalFile;
		string _code;

		public string Build(string code, ProcessFileCtx ctx)
		{
			_ctx = ctx;
			_code = code;
			_ctx.BlockParser = BlockParser.GetBlockParser(ctx);
			code = Preprocess(ctx, code);
			var metacode = BuildMetacode(ctx, code);
			return metacode;
		}

		string Preprocess(ProcessFileCtx ctx, string code)
		{
			foreach (var block in ctx.BlockParser.Parse(code))
			{
				SaveCaptureState(block);
				var blockType = block.Type;
				switch (blockType)
				{
					case BlockParser.BlockType.Extensibility:
						ExecuteExtender(block.Body);
						break;
				}
			}
			if(_enabledStringInterpolation)
			{
				code = ProcessStringInterpolation(code);
			}
			return code;
		}

		string BuildMetacode(ProcessFileCtx ctx, string code)
		{
			_ctx = ctx;
			_code = code;

			// auto import same namespaces
			_namespacesFromOriginalFile = Regex.Matches(code, @"(?m)^using\s+([^;]+)").Cast<Match>().Select(x => x.Groups[1].Value).ToArray();
			_outerNamespaceFromOriginalFile = Regex.Match(code, @"(?m)^namespace\s+([^\r\n]+)").Groups[1].Value;

			ctx.ImportsFromOriginalFile = _namespacesFromOriginalFile;
			ctx.OuterNamespaceFromOriginalFile = _outerNamespaceFromOriginalFile;

			var blocks = ctx.BlockParser.Parse(code);

			int from = 0;
			bool isFirstBlock = true;
			// indicate that it is "something /*! asd */ " but not "/*! asdsad */"
			bool isInsertion = false;
			// create metacode
			foreach (var block in blocks)
			{
				_ctx.NumberOfMetaBlocksProcessed++;
				SaveCaptureState(block);

				var type = block.Type;
				var value = block.Body;
				var pre = block.Pre;

				ctx.BuildErrorLogger.LogDebug("Block at {0}:{4} type {1}\r\nBody = {2}\r\nPre = {3}".Arg(block.Index, block.Type, block.Body, block.Pre, block.Length));

				isInsertion = pre.Trim().Any();

				var writeLineRemap = isFirstBlock && !isInsertion;
				// write previous text as plain
				PlainTextWriter(code.Substring(from, block.Index - from), _methodBody, writeLineRemap ? GetLineNumberByIndex(code, from) : 0);

				isFirstBlock = false;

				from = block.Index + block.Length;
				switch (type)
				{
					case BlockParser.BlockType.ExtensionMethod:
						value = value.Trim();
						if (!value.StartsWith("this."))
						{
							value = "this." + value;
						}

						bool semi = !value.TrimEnd().EndsWith(";");
						bool brackets = !value.TrimEnd().TrimEnd(';').TrimEnd().EndsWith(")");

						var tryLastWordValue = value;
						var tryLastWord = CutLastWord(ref tryLastWordValue);
						// Debugger.Launch();

						if (!string.IsNullOrEmpty(tryLastWord)) // something cutted
						{
							if (!string.IsNullOrEmpty(tryLastWordValue)) // something leaved
							{
								if (!tryLastWord.Contains('>')) // not a generic part
								{
									if (!tryLastWord.Contains(')')) // not a method invokation part
									{
										if (brackets)
										{
											brackets = false;
											value = tryLastWordValue + "(\"" + tryLastWord + "\")";
										}
									}
								}
							}
						}

						if (brackets) value += "()";
						if (semi) value += ";";


						//var method = CutFirstWord(ref value);
						//if (!string.IsNullOrEmpty(value))
						//{
						//    value = '"' + value + '"';
						//}
						_methodBody.AppendLine(value);
						break;
					case BlockParser.BlockType.Extensibility:
						// extender already preprocessed
						break;
					case BlockParser.BlockType.GenMethodBody:
						// TODO detect, is it new line or just some insertion
						if (_errorRemap)
						{
							_methodBody.AppendLine("#line " + GetLineNumberByIndex(code, _currentBlockIndex) + ' ' + '"' +
								_ctx.GetOriginalFileNameRelativeToIntermediatePath() + '"');
						}
						_methodBody.AppendLine(value);
						if (_errorRemap)
						{
							_methodBody.AppendLine("#line default");
						}
						break;
					case BlockParser.BlockType.GenExpressionBody:
						_methodBody.AppendLine("Write(" + value + ");");
						break;
					case BlockParser.BlockType.GenClassBody:
						_classBody.AppendLine("#line " + GetLineNumberByIndex(code, _currentBlockIndex) + ' ' + '"' + _ctx.GetOriginalFileNameRelativeToIntermediatePath() + '"');
						_classBody.AppendLine(value);
						_classBody.AppendLine("#line default");
						break;
					default:
						throw new Exception("Block with type " + type + " unexpected");
				}
			}

			PlainTextWriter(code.Substring(from), _methodBody, isInsertion ? 0 : GetLineNumberByIndex(code, from));


			var imports = _namespacesFromOriginalFile.OrEmpty().Concat(_namespaceImportsMetaAdditional.OrEmpty());
			return CreateCode(imports, _methodBody.ToString(), _classBody.ToString());
		}

		public static string CreateCode(IEnumerable<string> imports, string methodBody, string classBody)
		{
			imports = imports.Concat(_defaultUsings).Distinct();
			var importsAsString = string.Join(Environment.NewLine, imports.Select(x => "using " + x + ";").ToArray());
			return _skeleton.Arg(importsAsString, methodBody, classBody);
		}

		public void SaveCaptureState(BlockParser.Block block)
		{
			_currentBlockIndex = block.Index;
			_currentBlockLength = block.Length;
			SaveCaptureStateRest();
		}

		public void SaveCaptureState(Match match)
		{
			_currentBlockIndex = match.Index;
			_currentBlockLength = match.Length;
			SaveCaptureStateRest();
		}

		void SaveCaptureStateRest()
		{
			_currentBlockFinishIndex = _currentBlockIndex + _currentBlockLength;
		}

		int _currentBlockIndex;
		int _currentBlockLength;
		int _currentBlockFinishIndex;

		public int GetLineNumberByIndex(string code, int i)
		{
			return code
				.Substring(0, i)
				.Split('\r').Length;
		}

		internal static readonly Regex _rxStringInterpolVerbatim = new Regex(@"@""([^""]+)""");
		internal static readonly Regex _rxStringInterpolInside = new Regex(@"{([^\d].*?)}");
		internal static readonly Regex _rxStringInterpolNoVerbatim = new Regex(@"(?<!@)""(.*?[^\\])""");

		string ProcessStringInterpolation(string code)
		{
			code = _rxStringInterpolNoVerbatim.Replace(code, match =>
			{
				var stringValue = match.Groups[1].Value;
				stringValue = _rxStringInterpolInside.Replace(stringValue, m =>
				{
					SaveCaptureState(match);
					var val = m.Groups[1].Value;
					if(string.IsNullOrEmpty(val))
					{
					   return null;
					}
					return "\"+" + val + "+\"";
				});
				stringValue = "\"" + stringValue + "\"";
				// trim "" + and + ""
				if (stringValue.StartsWith("\"\"+"))
				{
					stringValue = stringValue.Substring(3);
				}
				if (stringValue.EndsWith("+\"\""))
				{
					stringValue = stringValue.Substring(0, stringValue.Length - 3);
				}
				return stringValue;
			});

			code = _rxStringInterpolVerbatim.Replace(code, match =>
			{
				var stringValue = match.Groups[1].Value;
				stringValue = _rxStringInterpolInside.Replace(stringValue, m =>
				{
					SaveCaptureState(match);
					var val = m.Groups[1].Value;
					if (string.IsNullOrEmpty(val))
					{
					   return null;
					}
					return "\"+" + val + "+@\"";
				});
				stringValue = "@\"" + stringValue + "\"";
				return stringValue;
			});
			return code;
		}

		readonly Dictionary<string, Action<string>> _mapExtenders;

		bool _enabledStringInterpolation;


		static string CutFirstWord(ref string str)
		{
			string word;
			var i = str.IndexOf(' ');
			if (i < 0)
			{
				word = str;
				str = string.Empty;
			}
			else
			{
				word = str.Substring(0, i).Trim();
				str = str.Substring(i).Trim();
			}
			return word;
		}

		static string CutLastWord(ref string str)
		{
			string word;
			var i = str.LastIndexOf(' ');
			if (i < 0)
			{
				word = str;
				str = string.Empty;
			}
			else
			{
				word = str.Substring(i).Trim();
				str = str.Substring(0, i).Trim();
			}
			return word;
		}

		public void ExecuteExtender(string extender)
		{
			extender = extender.Trim();

			var name = CutFirstWord(ref extender);
			var args = extender;

			Action<string> value;
			if (_mapExtenders.TryGetValue(name.ToLowerInvariant(), out value))
			{
				value(args);
			}
			else
			{
				var knownExtenders = string.Join(string.Empty, _mapExtenders.Keys.Select(x => Environment.NewLine + x).ToArray());

				WriteWarning("Meta Extender '{0}' is unknown. Known extenders is:{1}", name, knownExtenders);
				//_ctx.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, _ctx.OriginalFileName,
				//GetLineNumberByIndex(_code, _currentBlockIndex), 0, 0, 0,
				//"Meta Extender '{0}' is unknown. Known extenders is:{1}".Arg(name.ToLowerInvariant(), knownExtenders), null, null));
			}
		}

		static bool ToBool(string val)
		{
			if (val == null)
			{
				return true;
			}
			switch (val.ToLowerInvariant().Trim())
			{
				case "enable":
				case "enabled":
				case "on":
				case "1":
				case "true":
				case "yes":
				case "":
					return true;
				case "disable":
				case "disabled":
				case "off":
				case "0":
				case "false":
				case "no":
					return false;
				default:
					throw new Exception("Can not convert '{0}' to bool".Arg(val));
			}
		}

		void StringInterpolation(string arg)
		{
			_enabledStringInterpolation = ToBool(arg);
		}

		void ErrorRemap(string arg)
		{
			_errorRemap = ToBool(arg);
		}

		bool _errorRemap = true;

		void ReferenceT4(string arg)
		{
			var value = ParseT4Dog("name", arg);
			if (value != null)
			{
				Reference(value);
			}
			else
			{
				_ctx.BuildErrorLogger.LogDebug("Wrong ReferenceT4 Attribute: " + arg);
			}
		}

		void UsingT4(string arg)
		{
			var value = ParseT4Dog("namespace", arg);
			if (value != null)
			{
				Using(value);
			}
			else
			{
				_ctx.BuildErrorLogger.LogDebug("Wrong UsingT4 Attribute: " + arg);
			}
		}

		/// <summary>
		/// Only one pair is supported
		/// </summary>
		string ParseT4Dog(string parameterName, string line)
		{
			var parts = line.Split('=');
			var field = parts[0];
			var value = parts[1].Trim('"');
			if (field.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase))
			{
				return value;
			}
			return null;
		}

		void Reference(string arg)
		{
			_ctx.BuildErrorLogger.LogDebug("@Reference = " + arg);
			_ctx.ReferencesMetaAdditional.Add(arg);
		}

		void GenerateBanner(string arg)
		{
			WriteWarning("GenerateBanner is deprecated. Remove that.");
			//ctx.GenerateBanner = ToBool(arg);
		}

		void WriteWarning(string message, params object[] args)
		{
			_ctx.BuildErrorLogger.LogWarningEvent(new BuildWarningEventArgs(null, null, _ctx.OriginalRelativeFileName, GetLineNumberByIndex(_code, _currentBlockIndex), 0, GetLineNumberByIndex(_code, _currentBlockFinishIndex), 0, message.Arg(args), null, "MetaCreator.dll"));
		}

		void WriteError(string message, params object[] args)
		{
			_ctx.BuildErrorLogger.LogErrorEvent(new BuildErrorEventArgs(null, null, _ctx.OriginalRelativeFileName, GetLineNumberByIndex(_code, _currentBlockIndex), 0, GetLineNumberByIndex(_code, _currentBlockFinishIndex), 0, message.Arg(args), null, "MetaCreator.dll"));
		}

		void Using(string arg)
		{
			_namespaceImportsMetaAdditional.Add(arg);
		}

		readonly List<string> _namespaceImportsMetaAdditional = new List<string>();

	}
}
﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using MetaCreator.Utils;

namespace MetaCreator.Evaluation
{
	[Serializable]
	public class EvaluationResult
	{
		public object ReturnedValue { get; set; }
		public CompilerError[] Errors { get; set; }
		public CompilerError[] Warnings { get; set; }
		public string SourceCode { get; set; }
		public string CompileError { get; set; }
		public Exception EvaluationException { get; set; }
		public string CompileTempPath { get; set; }

		[NonSerialized]
		public string NonUserCode;

		[NonSerialized]
		public Assembly Assembly;

		public bool IsSuccess
		{
			get
			{
				return !Errors.OrEmpty().Any() && CompileError == null && EvaluationException == null;
			}
		}

		public string DebugLog = string.Empty;
		public string[] References;

		public void AddToCompile(bool fileInProject, string fileName, string fileContent)
		{
			var nf = new NewFile
			{
				FileBody = fileContent,
				FileName = fileName,
				FileInProject = fileInProject,
			};
			NewFiles = NewFiles == null ? new[] { nf } : new List<NewFile>(NewFiles) { nf }.ToArray();
		}

		public NewFile[] NewFiles = new NewFile[0];

		[Serializable]
		public class NewFile
		{
			public string FileName;
			public string FileBody;
			public bool FileInProject;
		}

	}

}
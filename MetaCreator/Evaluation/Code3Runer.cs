﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using MetaCreator.AppDomainIsolation;
using MetaCreator.Utils;

namespace MetaCreator.Evaluation
{
	/// <summary>
	/// Inmemory code executor. Consider to execute this in separate app domain.
	/// </summary>
	class Code3Runer : IMetaEngine
	{
		EvaluationResult _evaluationResult;
		AnotherAppDomInputData _input;

		internal void Run(EvaluationResult evaluationResult, string className, string methodName, AnotherAppDomInputData input)
		{
			_evaluationResult = evaluationResult;
			_input = input;

			evaluationResult.EnsureExistsDebug();
			evaluationResult.Assembly.EnsureExistsDebug();

			var type = evaluationResult.Assembly.GetType(className, true);
			type.EnsureExistsDebug("Generator class not found");

			var method = type.GetMethod(methodName,
				BindingFlags.Public
				| BindingFlags.NonPublic
				| BindingFlags.Static
				| BindingFlags.Instance);
			method.EnsureExistsDebug("Generator method not found");

			if (method.GetParameters().Count() != 0 || method.GetGenericArguments().Count() != 0)
			{
				throw new Exception("Method has unexpected parameters");
			}

			object returnedValue = null;
			using (new Resolver(evaluationResult.References))
			{
				try
				{
					object instance = null;
					if (!method.IsStatic)
					{
						var ciExtra = type.GetConstructor(new[]
						{
							typeof (IMetaEngine)
						});
						if (ciExtra != null)
						{
							instance = Activator.CreateInstance(type, new object[]
							{
								this
							});
						}
						else
						{
							instance = Activator.CreateInstance(type, true);
						}
					}
					EngineState.Imports = Imports;
					EngineState.OuterNamespace = OuterNamespace;
					CreateWriter();
					returnedValue = method.Invoke(instance, null);
					DeleteWriter();
					EngineState.Imports = null;
					EngineState.OuterNamespace = null;
				}
				catch (TargetInvocationException ex)
				{
					evaluationResult.EvaluationException = ex.InnerException ?? ex;
				}
			}

			evaluationResult.ReturnedValue = returnedValue;
		}

		void IMetaEngine.AddToCompile(string fileContent)
		{
			_evaluationResult.AddToCompile(false, null, fileContent);
		}

		void IMetaEngine.AddToCompile(string fileName, string fileContent)
		{
			_evaluationResult.AddToCompile(false, fileName, fileContent);
		}

		void IMetaEngine.AddToCompile(bool fileInProject, string fileName, string fileContent)
		{
			_evaluationResult.AddToCompile(fileInProject, fileName, fileContent);
		}

		public string[] Imports
		{
			get { return _input != null ? (_input.ImportsFromOriginalFile ?? new string[0]) : new string[0]; }
		}

		void CreateWriter()
		{
			_writerDefault = new DefaultMetaWriter();
			_writer = new ArgumentConverterWriterProxy(_writerDefault, ConvertArgument);
		}

		private void DeleteWriter()
		{
			_writerDefault = null;
			_writer = null;
		}

		DefaultMetaWriter _writerDefault;
		IMetaWriter _writer;

		public IMetaWriter Writer
		{
			get
			{
				return _writer;
			}
		}

		private object ConvertArgument(object arg)
		{
			var type = arg as Type;
			if (type != null)
			{
				return type.CSharpTypeIdentifier();
			}
			return arg;
		}

		public string OuterNamespace
		{
			get { return _input != null ? (_input.OuterNamespaceFromOriginalFile ?? string.Empty) : string.Empty; }
		}
	}
}

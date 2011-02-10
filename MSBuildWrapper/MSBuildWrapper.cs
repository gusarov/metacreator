using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

[LoadInSeparateAppDomainAttribute]
public sealed class MSBuildWrapper : AppDomainIsolatedTask
{
	[Required]
	public string Task { get; set; }

	[Required]
	public string Bin { get; set; }

	public ITaskItem[] ParamNames { get; set; }
	public ITaskItem[] OutputParamNames { get; set; }
	public ITaskItem[] ArrayNames { get; set; }
	public ITaskItem[] OutputArrayNames { get; set; }

	public string Param1 { get; set; }
	public string Param2 { get; set; }
	public string Param3 { get; set; }
	public string Param4 { get; set; }
	public string Param5 { get; set; }
	public string Param6 { get; set; }
	public string Param7 { get; set; }
	public string Param8 { get; set; }
	public string Param9 { get; set; }

	public ITaskItem[] Array1 { get; set; }
	public ITaskItem[] Array2 { get; set; }
	public ITaskItem[] Array3 { get; set; }
	public ITaskItem[] Array4 { get; set; }
	public ITaskItem[] Array5 { get; set; }
	public ITaskItem[] Array6 { get; set; }
	public ITaskItem[] Array7 { get; set; }
	public ITaskItem[] Array8 { get; set; }
	public ITaskItem[] Array9 { get; set; }

	[Output]
	public ITaskItem[] OutputArray1 { get; set; }

	[Output]
	public ITaskItem[] OutputArray2 { get; set; }

	[Output]
	public ITaskItem[] OutputArray3 { get; set; }

	[Output]
	public ITaskItem[] OutputArray4 { get; set; }

	[Output]
	public ITaskItem[] OutputArray5 { get; set; }

	[Output]
	public ITaskItem[] OutputArray6 { get; set; }

	[Output]
	public ITaskItem[] OutputArray7 { get; set; }

	[Output]
	public ITaskItem[] OutputArray8 { get; set; }

	[Output]
	public ITaskItem[] OutputArray9 { get; set; }

	[Output]
	public string OutputParam1 { get; set; }
	[Output]
	public string OutputParam2 { get; set; }
	[Output]
	public string OutputParam3 { get; set; }
	[Output]
	public string OutputParam4 { get; set; }
	[Output]
	public string OutputParam5 { get; set; }
	[Output]
	public string OutputParam6 { get; set; }
	[Output]
	public string OutputParam7 { get; set; }
	[Output]
	public string OutputParam8 { get; set; }
	[Output]
	public string OutputParam9 { get; set; }

	new void Log(string msg)
	{
		base.Log.LogMessage(MessageImportance.High, "* MSBuildTaskWrapper: " + msg);
	}

	public override bool Execute()
	{
		Log("Execute " + Task);

		Type taskType;
		try
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			taskType = Type.GetType(Task);
		}
		finally
		{
			AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
		}

		if (taskType == null)
		{
			throw new Exception("Task Type '" + Task + "' does not found");
		}

		var task = (AppDomainIsolatedTask) Activator.CreateInstance(taskType);

		if (taskType == null)
		{
			throw new Exception("Task Type '" + taskType + "' does not instanciated");
		}

		task.BuildEngine = BuildEngine;
		task.HostObject = HostObject;
		//task.LogMessage = msg => base.Log.LogMessage(MessageImportance.High, "* Task " + msg);
		//task.Log

		if (ParamNames != null)
		{
			for (var i = 0; i < ParamNames.Length; i++)
			{
				var paramName = ParamNames[i];

				var spi = GetType().GetProperty("Param" + (i + 1));
				if(spi==null)
				{
					throw new Exception("Source property " + "Param" + (i + 1)+" does not found");
				}
				var value = spi.GetValue(this, null);
				var tpi = taskType.GetProperty(paramName.ItemSpec);
				if (tpi == null)
				{
					throw new Exception("Target property " + paramName.ItemSpec + " does not found");
				}
				tpi.SetValue(task, value, null);
			}
		}

		if (ArrayNames != null)
		{
			for (var i = 0; i < ArrayNames.Length; i++)
			{
				var arrayName = ArrayNames[i];
				var spi = GetType().GetProperty("Array" + (i + 1));
				if (spi == null)
				{
					throw new Exception("Source array " + "Array" + (i + 1) + " does not found");
				}
				var value = spi.GetValue(this, null);
				var tpi = taskType.GetProperty(arrayName.ItemSpec);
				if (tpi == null)
				{
					throw new Exception("Target property " + arrayName.ItemSpec + " does not found");
				}
				tpi.SetValue(task, value, null);
			}
		}

		var result = task.Execute();

		if (OutputArrayNames != null)
		{
			for (var i = 0; i < OutputArrayNames.Length; i++)
			{
				var arrayName = OutputArrayNames[i];
				var value = taskType.GetProperty(arrayName.ItemSpec).GetValue(task, null);
				GetType().GetProperty("OutputArray" + (i + 1)).SetValue(this, value, null);
			}
		}

		if (OutputParamNames != null)
		{
			for (var i = 0; i < OutputParamNames.Length; i++)
			{
				var paramName = OutputParamNames[i];
				var value = taskType.GetProperty(paramName.ItemSpec).GetValue(task, null);
				GetType().GetProperty("OutputParam" + (i + 1)).SetValue(this, value, null);
			}
		}

		return result;
	}

	Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	{
		var i = Task.IndexOf(',');
		if (i == -1)
		{
			throw new Exception("Specify task name with assembly, like 'MyNamespace.MyTask, MyAssembly.dll'");
		}
		var file = Path.Combine(Bin, Task.Substring(i).Trim().Trim(',').Trim());
		return Assembly.LoadFrom(file);
	}
}
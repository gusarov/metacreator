// METACODE
// This file is generated and compiled in-memory.
// If there are any compilation errors - file can be saved to temporary path for working with IDE.

// imports
using SomeSuper;
using global::Microsoft.Win32;
using global::Microsoft.Win32.SafeHandles;
using global::System;
using global::System.Collections;
using global::System.Collections.Generic;
using global::System.Collections.Specialized;
using global::System.Collections.ObjectModel;
using global::System.Configuration.Assemblies;
using global::System.ComponentModel;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Globalization;
using global::System.Host;
using global::System.IO;
using global::System.IO.Compression;
using global::System.IO.Ports;
using global::System.Media;
using global::System.Net;
using global::System.Sockets;
using global::System.PInvoke;
using global::System.PInvoke.Performance;
using global::System.Reflection;
using global::System.Resources;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.InteropServices.ComTypes;
using global::System.Runtime.Serialization;
using global::System.Runtime.Serialization.Formatters.Binary;
using global::System.Security;
using global::System.Security.Cryptography;
using global::System.Security.Cryptography.X509Certificates;
using global::System.Security.Permissions;
using global::System.Security.Policy;
using global::System.Security.Util;
using global::System.Text;
using global::System.Text.RegularExpressions;
using global::System.Threading;
using global::System.CodeDom.Compiler;
using global::System.Linq;
using global::Microsoft.CSharp;

public static class Generator
{
	public static string Run()
	{
		// methodbody
		Write(@"
using SomeSuper;

public class Class1
{
	public static void Main()
	{
		var message = ""hello world"";
		");
 Write("System.Console.WriteLine(message);"); 
Write(@"
	}
}
");

		return Result.ToString();
	}

	// classbody


	#region utils

	public static StringBuilder Result = new StringBuilder();

	public static void Write(string msg, params object[] args)
	{
		Result.AppendFormat(msg, args);
	}

	public static void WriteLine(string msg, params object[] args)
	{
		Result.AppendFormat(msg + Environment.NewLine, args);
	}

	public static void Write(string msg)
	{
		Result.Append(msg);
	}

	public static void WriteLine(string msg)
	{
		Result.AppendLine(msg);
	}

	public static void Write(object obj)
	{
		Result.Append(obj == null ? string.Empty : obj.ToString());
	}

	public static void WriteLine(object obj)
	{
		Result.AppendLine(obj == null ? string.Empty : obj.ToString());
	}

	#endregion
}
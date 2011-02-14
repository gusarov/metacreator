using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MetaCreator;
using MetaCreator.AppDomainIsolation;
using MetaCreator.Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class ExecuteMetaCreatorTest
	{
		[TestMethod]
		public void ProcessFile_create_correct_build_error_line_number()
		{
			var code = @"
class Sample {
	public static bool IsProcessed {
		get {
/*! 
#error This is error, that actually in the line #6 of original file
*/
			return false;
		}
	}
}
";
			SimulateBuild(code);
			Assert.IsTrue(buildFailed);

			Assert.AreEqual(1, logger.Errors.Count);
			Assert.AreEqual(0, logger.Warnings.Count);
			Assert.AreEqual(6, logger.Errors[0].LineNumber);
			Assert.IsTrue(logger.Errors[0].File.Contains("1.tmp"));
		}

		FakeErrorLogger logger;
		bool buildFailed;
		ExecuteMetaCreatorCore sut;
		ProcessFileCtx ctx;

		void SimulateBuild(string code)
		{
			logger = new FakeErrorLogger();
			sut = new ExecuteMetaCreatorCore
			{
				BuildErrorLogger = logger,
				ProjDir = Path.GetDirectoryName(GetType().Assembly.Location),
				IntermediateOutputPathRelative = "obj\\Debug",
				IntermediateOutputPathFull = Path.GetFullPath("obj\\Debug"),
				Sources = new ITaskItem[] {new TaskItem("1.tmp")},
			};
			sut.Initialize();
			buildFailed = false;
			try
			{
				sut.ProcessFile(ctx = new ProcessFileCtx
				{
					FileOriginalContent = code,
					BuildErrorLogger = logger,
					OriginalFileName = "1.tmp",
					ProjDir = sut.ProjDir,
					AppDomFactory = AnotherAppDomFactory.AppDomainLiveScope(),
				});
			}
			catch (FailBuildingException ex)
			{
				Console.WriteLine(ex.ToString());
				buildFailed = true;
			}
		}

		[TestMethod]
		public void ProcessFile_string_interpolation()
		{
			var code = @"
/*@ StringInterpolation */
class q
{
	void c()
	{
		static int a=5;
		static string b=""a={a}"";
	}
}";

			SimulateBuild(code);
			Assert.IsFalse(buildFailed);

			var act = ctx.FileProcessedContent.Trim();

			switch (act.Trim())
			{
				case @"class q
{
	void c()
	{
		static int a=5;
		static string b=""a=""+a;
	}
}":
					break;
				case @"class q
{
	void c()
	{
		static int a=5;
		static string b=""a=""+a+"""";
	}
}":
					Console.WriteLine(act);
					Assert.Inconclusive("+");
					break;
				default:
					//File.WriteAllText(Path.GetTempFileName(), exp);
					//File.WriteAllText(Path.GetTempFileName(), act);
					Assert.Fail(act);
					break;
			}
		}

		[TestMethod]
		public void ProcessFile_string_interpolation_in_med()
		{
			var sut = new ExecuteMetaCreatorCore();
			var ctx = GetSut();
			ctx.FileOriginalContent = @"
/*@ StringInterpolation */
class q
{
	void c()
	{
		static string a=""asd"";
		static string b=""a={a}_"";
	}
}
";
			sut.ProcessFile(ctx);
			Assert.AreEqual(@"class q
{
	void c()
	{
		static string a=""asd"";
		static string b=""a=""+a+""_"";
	}
}", ctx.FileProcessedContent.Trim());
		}

		[TestMethod]
		public void Should_not_corrupt_empty_strings()
		{
			var sut = new ExecuteMetaCreatorCore();
			var ctx = GetSut();
			ctx.FileOriginalContent = @"
/*@ StringInterpolation */
class q
{
	void c()
	{
		static string a="""";
		static string b=@"""";
	}
}
";
			sut.ProcessFile(ctx);
			Assert.AreEqual(@"class q
{
	void c()
	{
		static string a="""";
		static string b=@"""";
	}
}", ctx.FileProcessedContent.Trim());
		}

		[TestMethod]
		public void ProcessFile_string_interpolation_verbatim()
		{
			var sut = new ExecuteMetaCreatorCore();
			var ctx = GetSut();
			ctx.FileOriginalContent = @"
/*@ StringInterpolation */
class q
{
	void c()
	{
		static string a=""asd"";
		static string b=@""a={a}_"";
	}
}
";
			sut.ProcessFile(ctx);
			Assert.AreEqual(@"class q
{
	void c()
	{
		static string a=""asd"";
		static string b=@""a=""+a+@""_"";
	}
}", ctx.FileProcessedContent.Trim());
		}

		[TestMethod, Ignore]
		public void Should_use_string_builder_if_there_are_more_than_7_items()
		{
			var sut = new ExecuteMetaCreatorCore();
			var ctx = GetSut();
			ctx.FileOriginalContent = @"
/*@ StringInterpolation */
string a = ""asd1"";
string b = ""asd2"";
return ""a={a}, b={b}, a={a}, b={b}, a={a}, b={b}, a={a}, b={b}"";
";
			sut.ProcessFile(ctx);
			Assert.AreEqual(@"
string a = ""asd1"";
string b = ""asd2"";
return string.Format(""a={0}, b={1}, a={0}, b={1}, a={0}, b={1}, a={0}, b={1}"", a, b);
", ctx.FileProcessedContent.Trim());
			Assert.AreEqual(@"
string a = ""asd1"";
string b = ""asd2"";
return new StringBuilder().Append(""a="").Append(a).Append("", b="").Append(b).Append("", a="").Append(a).Append("", b="").Append(b).Append("", a="").Append(a).Append("", b="").Append(b).ToString();
", ctx.FileProcessedContent.Trim());
			Assert.Inconclusive();
			//Assert.AreEqual(Evaluator.EvaluateMethodBody(ctx.FileOriginalContent), Evaluator.EvaluateMethodBody(ctx.FileProcessedContent));
		}

		[TestMethod, Ignore]
		public void Should_show_who_is_Faster()
		{
			string a = "asd1";
			string b = "asd2";
			var sw1 = Stopwatch.StartNew();
			string q1=null,q2=null;
			for (int i = 0; i < int.MaxValue / 500; i++)
			{
				// q1 = string.Format("a={0}, b={1}, a={0}, b={1}, a={0}, b={1}, a={0}, b={1}", a, b);
				// q1 = string.Format("a={0}, b={1}, a={0}, b={1}", a, b);
				// q1 = "a=" + a + ", b=" + b + ", a=" + a + ", b=" + b;
				// q1 = "a=" + a + ", b=" + b + ", a=" + a + ", b=" + b + ", a=" + a + ", b=" + b + ", a=" + a + ", b=" + b;
				q1 = "a=" + a + ", b=" + b + ", a=" + a + ", b=" + b 
					+ ", a=" + a + ", b=" + b + ", a=" + a + ", b=" + b 
					+ ", a=" + a + ", b=" + b + ", a=" + a + ", b=" + b
					+ ", a=" + a + ", b=" + b + ", a=" + a + ", b=" + b
					+ ", a=" + a + ", b=" + b + ", a=" + a + ", b=" + b;
			}
			sw1.Stop();
			var sw2 = Stopwatch.StartNew();
			for (int i = 0; i < int.MaxValue / 500; i++)
			{
				//q2 = new StringBuilder().Append("a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b).ToString();
				q2 = new StringBuilder().Append("a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b)
					.Append(", a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b)
					.Append(", a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b)
					.Append(", a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b)
					.Append(", a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b).ToString();
				//q2 = new StringBuilder().Append("a=").Append(a).Append(", b=").Append(b).Append(", a=").Append(a).Append(", b=").Append(b).ToString();
			}
			sw2.Stop();
			Assert.AreEqual(q1,q2);
			Assert.Inconclusive(sw1.Elapsed + " <=> " + sw2.Elapsed);
		}

		[TestMethod]
		public void Should_detect_no_verbatim_strings_in_code()
		{
			var rx = ExecuteMetaCreatorCore._rxStringInterpolNoVerbatim;
			ValidateString(rx, true, @"someThing = ""asd"";", "\"asd\"","asd");
			ValidateString(rx, false, @"someThing = ""asd;", null, "asd");
			ValidateString(rx, false, @"someThing = @""asd"";", null, "asd");
		}

		[TestMethod]
		public void Should_detect_verbatim_strings_in_code()
		{
			var rx = ExecuteMetaCreatorCore._rxStringInterpolVerbatim;
			ValidateString(rx, false, @"someThing = ""asd"";", null, "asd");
			ValidateString(rx, false, @"someThing = ""asd;", null, "asd");
			ValidateString(rx, true, @"someThing = @""asd"";", "@\"asd\"", "asd");
			ValidateString(rx, false, @"someThing = @""asd;", null, "asd");
		}

		static void ValidateString(Regex rx, bool result, string input, string match, string body)
		{
			var m = rx.Match(input);
			Assert.AreEqual(result, m.Success);
			if (m.Success)
			{
				Assert.AreEqual(match, m.Value, "input = " + input);
				Assert.AreEqual(body, m.Groups[1].Value);
			}
		}


//      [TestMethod]
//      public void Should_detect_original_namespace_imports()
//      {
//         var tmp = File.WriteAllText("q.cs",@");
//			var sut = new ExecuteMetaCreatorCore
//				{
//					
//				};
//			sut.Execute();
//			DebugAssert.Inconclusive();
//		}
	}
}



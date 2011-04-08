using System.IO;
using MetaCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class EvaluatorTest
	{
		[TestMethod, Ignore]
		public void EvaluateGeneratorCoreTest_Warning()
		{
			var result = Evaluator.EvaluateGeneratorCore(@"

#warning test13
#warning test13
#warning test13
#warning test13

return;
return;
return;

");
			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("", result.ResultBody);
			try
			{
				Assert.IsNotNull(result.Warnings, "That is very strange - CSharpCodeProvider does not produce warning list, if there are only warnings without errors");
			}
			catch
			{
				Assert.Inconclusive("CSharpCodeProvider error");
			}
			Assert.AreEqual(1, result.Warnings.Length);
			StringAssert.Contains(result.Warnings[0].ErrorText, "test13");
		}

		[TestMethod]
		public void EvaluateGeneratorCoreTestError()
		{
			var result = Evaluator.EvaluateGeneratorCore(@"

#error test13

");
			Assert.IsFalse(result.IsSuccess);
			Assert.IsNotNull(result.Errors);
			Console.WriteLine(result.SourceCode);
			Assert.AreEqual(1, result.Errors.Length, string.Join(Environment.NewLine, result.Errors.Select(x=>x.ToString()).ToArray()));
			StringAssert.Contains(result.Errors[0].ErrorText, "test13");
		}

		[TestMethod]
		public void EvaluateGeneratorCoreTestErrorAndWarn()
		{
			var result = Evaluator.EvaluateGeneratorCore(@"

#error test13
#warning  test12

");
			Assert.IsFalse(result.IsSuccess);
			Assert.IsNotNull(result.Errors);
			Assert.IsNotNull(result.Warnings);
			Assert.AreEqual(1, result.Errors.Length);
			Assert.AreEqual(1, result.Warnings.Length);
			StringAssert.Contains(result.Errors[0].ErrorText, "test13");
			StringAssert.Contains(result.Warnings[0].ErrorText, "test12");
		}


		[TestMethod]
		public void EvaluateGeneratorCoreTestException()
		{
			var result = Evaluator.EvaluateGeneratorCore(@"throw null;");

			Assert.IsFalse(result.IsSuccess);
			Assert.IsTrue(result.EvaluationException != null);
			Assert.IsTrue(result.EvaluationException is NullReferenceException);
			Assert.IsTrue(result.EvaluationExceptionAtLine > 1);

			var line = result.SourceCode.IndexOf("// <UserCode>");
			Assert.IsTrue(line > 0);
			var userCode = result.SourceCode.Substring(0, line).Split('\r').Length;

			File.WriteAllText(Path.GetTempFileName(), result.SourceCode);

			Assert.IsTrue(result.EvaluationExceptionAtLine > 1);
			//DebugAssert.AreEqual(46, result.EvaluationExceptionAtLine);
			Assert.AreEqual(userCode + 1, result.EvaluationExceptionAtLine);
		}

	}
}
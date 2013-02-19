using System;
using System.Collections.Generic;
using System.Linq;
using MetaCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class EvaluationTests
	{
		[TestMethod]
		public void Should_evaluate_expression()
		{
			Assert.AreEqual(4, Evaluator.EvaluateExpression<int>("2+2"));
		}

		[TestMethod]
		public void Should_evaluate_file()
		{
			Assert.AreEqual(4, Evaluator.EvaluateFile<int>(@"
class Generator
{
	public int Run()
	{
		return 2+2;
	}
}
"));
		}

		[TestMethod]
		public void Should_evaluate_method_body()
		{
			Assert.AreEqual(4, Evaluator.EvaluateMethodBody<int>(@"return 2+2;"));
		}
	}
}

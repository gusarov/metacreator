using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using MetaCreator.Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class ExamineSerialization
	{
		private readonly BinaryFormatter _formatter = new BinaryFormatter();

		T DeSerialize<T>(T item)
		{
			var ms = new MemoryStream();
			_formatter.Serialize(ms, item);
			ms.Position = 0;
			item = (T)_formatter.Deserialize(ms);
			return item;
		}

		[TestMethod]
		public void Should_serialize_execution_results()
		{
			var res = new EvaluationResult()
			{
				EvaluationException = new Exception("test"),
			};

			var des = DeSerialize(res);
			Assert.AreEqual("test", des.EvaluationException.Message);
		}

		[TestMethod]
		public void Should_serialize_exception()
		{
			var res = new FailBuildingException("test_build")
			{
				Result = new EvaluationResult
				{
					EvaluationException = new Exception("test_eval"),
				},
				IgnoreThisFile = true,
			};

			var des = DeSerialize(res);
			Assert.AreEqual("test_build", des.Message);
			Assert.IsTrue(des.IgnoreThisFile);
			Assert.IsNotNull(des.Result);
			Assert.IsNotNull(des.Result.EvaluationException);
			Assert.AreEqual("test_eval", des.Result.EvaluationException.Message);
		}

	}
}

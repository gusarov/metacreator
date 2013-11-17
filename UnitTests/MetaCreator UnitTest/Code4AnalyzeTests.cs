using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaCreator;
using MetaCreator.Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class Code4AnalyzeTests
	{
		readonly MetaCreator.Evaluation.Code4Analyze _sut = new MetaCreator.Evaluation.Code4Analyze();

		[TestMethod]
		public void Should_display_inner_exception_information()
		{
			var ex = new Exception("level 1", new Exception("level 2", new Exception("level 3")));
			var str = ExceptionAnalyzer.ExceptionDetails(ex);
			Assert.AreEqual(@"level 1
Inner: level 2
Inner: level 3", str);
		}
	}
}

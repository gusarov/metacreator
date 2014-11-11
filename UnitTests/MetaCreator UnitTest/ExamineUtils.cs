using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaCreator.Utils;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class ExamineUtils
	{
		[TestMethod]
		public void Should_convert_to_warning()
		{
			var error = new BuildErrorEventArgs("", "", "", 0, 0, 0, 0, "Test {{asd", "", "");
			Assert.AreEqual("Test {{asd", error.ConvertToWarning().Message);
		}
	}
}

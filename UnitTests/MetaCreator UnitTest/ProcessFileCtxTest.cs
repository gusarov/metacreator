using MetaCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class ProcessFileCtxTest
	{
		[TestMethod]
		public void GetOriginalFileNameRelativeToIntermediatePathTest()
		{
			var target = new ProcessFileCtx()
			{
				OriginalFileName = "MyFolder\\Class1.cs",
				IntermediateOutputPathRelative = "obj\\Debug",
				IntermediateOutputPathFull = "C:\\Files\\Solution1\\Project1\\obj\\Debug",
				ReplacementFileName = "C:\\Files\\Solution1\\Project1\\obj\\Debug\\MyFolder\\Class1.g.cs",
				ProjDir = "C:\\Files\\Solution1\\Project1"
			};
			Assert.AreEqual("C:\\Files\\Solution1\\Project1\\MyFolder\\Class1.cs", target.GetOriginalFileNameRelativeToIntermediatePath());
		}

		[TestMethod]
		public void GetOriginalFileNameRelativeToIntermediatePathTest3()
		{
			var target = new ProcessFileCtx()
			{
				OriginalFileName = "MyFolder\\Class1.cs",
				IntermediateOutputPathRelative = "bin\\debug",
				IntermediateOutputPathFull = "C:\\Files\\SolutionInterm\\bin\\debug",
				ReplacementFileName = "C:\\Files\\SolutionInterm\\bin\\debug\\MyFolder\\Class1.g.cs",
				ProjDir = "C:\\Files\\Solution1\\Project1"
			};
			Assert.AreEqual("C:\\Files\\Solution1\\Project1\\MyFolder\\Class1.cs", target.GetOriginalFileNameRelativeToIntermediatePath());
		}
	}
}
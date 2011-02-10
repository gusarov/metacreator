using MetaCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class ExtendersTest
	{
		[TestMethod]
		public void Execute_extender_should_log_warning_about_unknown_extender()
		{
			var logger = new FakeErrorLogger();
			var ctx = new ProcessFileCtx { BuildErrorLogger = logger };
			Extenders.ExecuteExtender("fake13", null, ctx);
			Assert.AreEqual(0, logger.Errors.Count);
			Assert.AreEqual(1, logger.Warnings.Count);
			Assert.IsTrue(logger.Warnings[0].Message.Contains("fake13"));
		}

		[TestMethod]
		public void Execute_extender_should_process_string_interpolation()
		{
			var ctx = new ProcessFileCtx();
			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
			Extenders.ExecuteExtender("stringinterpolation", null, ctx);
			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
		}

		[TestMethod]
		public void Execute_extender_should_process_string_interpolation_enable()
		{
			var ctx = new ProcessFileCtx();
			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
			Extenders.ExecuteExtender("stringinterpolation", "enable", ctx);
			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
		}

		[TestMethod]
		public void Execute_extender_should_process_string_interpolation_disable()
		{
			var ctx = new ProcessFileCtx();
			ctx.EnabledStringInterpolation = true;
			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
			Extenders.ExecuteExtender("stringinterpolation", "disable", ctx);
			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
		}
	}
}
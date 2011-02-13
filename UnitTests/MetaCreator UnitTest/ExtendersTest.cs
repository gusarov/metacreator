using MetaCreator;
using MetaCreator.Evaluation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest
{
	[TestClass]
	public class ExtendersTest
	{
		[TestInitialize]
		public void TestInit()
		{
			_logger = new FakeErrorLogger();
			_ctx = new ProcessFileCtx { BuildErrorLogger = _logger };
		}

		FakeErrorLogger _logger;
		ProcessFileCtx _ctx;


		[TestMethod]
		public void Execute_extender_should_log_warning_about_unknown_extender()
		{
			Extenders.ExecuteExtender("fake13", _ctx);
			Assert.AreEqual(0, _logger.Errors.Count);
			Assert.AreEqual(1, _logger.Warnings.Count);
			Assert.IsTrue(_logger.Warnings[0].Message.Contains("fake13"));
		}

		[TestMethod]
		public void Execute_extender_should_process_string_interpolation()
		{
			var ctx = new ProcessFileCtx();
			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
			Extenders.ExecuteExtender("stringinterpolation",  ctx);
			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
		}

		[TestMethod]
		public void Execute_extender_should_process_string_interpolation_enable()
		{
			var ctx = new ProcessFileCtx();
			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
			Extenders.ExecuteExtender("stringinterpolation enable", ctx);
			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
		}

		[TestMethod]
		public void Execute_extender_should_process_string_interpolation_disable()
		{
			var ctx = new ProcessFileCtx();
			ctx.EnabledStringInterpolation = true;
			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
			Extenders.ExecuteExtender("stringinterpolation disable", ctx);
			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
		}
	}
}
using System.Reflection;
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
			_sut = new MetaCreator.Evaluation.Code1Builder();
			_logger = new FakeErrorLogger();
			_ctx = new ProcessFileCtx { BuildErrorLogger = _logger };
			_sut.GetType().GetField("_ctx", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(_sut, _ctx);
		}

		FakeErrorLogger _logger;
		ProcessFileCtx _ctx;
		MetaCreator.Evaluation.Code1Builder _sut;

		[TestMethod]
		public void Execute_extender_should_log_warning_about_unknown_extender()
		{
			_sut.Build("/*@ fake13 */", _ctx);
			Assert.AreEqual(0, _logger.Errors.Count);
			Assert.AreEqual(1, _logger.Warnings.Count);
			Assert.IsTrue(_logger.Warnings[0].Message.Contains("fake13"));
		}
//
//		[TestMethod]
//		public void Execute_extender_should_process_string_interpolation()
//		{
//			_sut.ExecuteExtender("stringinterpolation");
//			Assert.AreEqual(true, _sut.GetType().GetField("_enabledStringInterpolation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_sut));
//		}
//
//		[TestMethod]
//		public void Execute_extender_should_process_string_interpolation_enable()
//		{
//			var ctx = new ProcessFileCtx();
//			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
//			Extenders.ExecuteExtender("stringinterpolation enable", ctx);
//			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
//		}
//
//		[TestMethod]
//		public void Execute_extender_should_process_string_interpolation_disable()
//		{
//			var ctx = new ProcessFileCtx();
//			ctx.EnabledStringInterpolation = true;
//			Assert.AreEqual(true, ctx.EnabledStringInterpolation);
//			Extenders.ExecuteExtender("stringinterpolation disable", ctx);
//			Assert.AreEqual(false, ctx.EnabledStringInterpolation);
//		}
	}
}
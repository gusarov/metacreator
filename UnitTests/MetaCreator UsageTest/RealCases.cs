using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UsageTest
{
	[TestClass]
	public class RealCases
	{
		[TestMethod]
		public void Should_add_notification_implementation()
		{
			Assert.Inconclusive("+");
			var obj = new Sample();
			var notify = obj as INotifyPropertyChanged;
			Assert.IsNotNull(notify);
			var ok = 0;
			notify.PropertyChanged += delegate { ok++; };
			Assert.AreEqual(0, ok);
			obj.MySample = "a";
			Assert.AreEqual(1, ok);
		}

	}
}

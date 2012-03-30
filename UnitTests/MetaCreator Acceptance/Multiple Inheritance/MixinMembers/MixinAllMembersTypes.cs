using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using MetaCreator_Acceptance.Properties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance.Multiple_Inheritance.NewFolder1
{
	[TestClass]
	public class MixinAllMembersTypes : Acceptance_base_tests
	{
		[TestMethod]
		public void Should_mixin_events()
		{
			File.WriteAllText("common.cs", Resources._sample_mixin_events_base);
			Build("Common");
			KillCs();

			File.WriteAllText("sample.cs", Resources._sample_mixin_events_exec);
			Build("Sample", "Common");

			var test = (INotifyPropertyChanged)Activator.CreateInstance(LoadAssembly("Sample").GetType("Test"));
			bool ok = false;
			test.PropertyChanged += delegate { ok = true; };
			test.GetType().GetProperty("TestProperty").SetValue(test, 5, null);
			Assert.AreEqual(true, ok);
		}
	}
}

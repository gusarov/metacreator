using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MetaCreator_Acceptance.Properties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_Acceptance.Multiple_Inheritance.By_interface_implementation_redirect
{
	[TestClass]
	public class __AtMinhMembers : Acceptance_base_tests
	{
		[TestMethod]
		public void Should_do_something()
		{
			// build common assembly: IBeh & BehImpl
			File.WriteAllText("common.cs", Resources._SampleMinhCommon2);
			Build("CommonAsm");
			KillCs();

			// build sample assembly: Derived : Base // +mixin BehImpl
			File.WriteAllText("sample.cs", Resources._SampleMinh2);
			Build("SampleAsm", "CommonAsm");

			// load assembly
			var asmCommon = LoadAssembly("CommonAsm.dll");
			Assert.IsNotNull(asmCommon);
			var asmSample = LoadAssembly("SampleAsm.dll");
			Assert.IsNotNull(asmSample);
			var baseClass = asmSample.GetType("Base");
			Assert.IsNotNull(baseClass);
			var behImpl = asmCommon.GetType("BehImpl");
			Assert.IsNotNull(behImpl);
			var derivedClass = asmSample.GetType("Derived");
			Assert.IsNotNull(derivedClass);

			var der = Activator.CreateInstance(derivedClass);
			Assert.AreEqual("Base_arg1", baseClass.GetMethod("BaseMethod").Invoke(der, new object[] { "arg1" }));
			Assert.AreEqual("Derived_arg1", derivedClass.GetMethod("DerivedMethod").Invoke(der, new object[] { "arg1" }));
			Assert.AreEqual("BehImpl_arg1", derivedClass.GetMethod("BehMethod").Invoke(der, new object[] { "arg1" }));
		}
	}
}

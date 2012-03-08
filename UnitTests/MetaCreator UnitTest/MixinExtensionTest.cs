using MetaCreator.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MetaCreator_UnitTest
{
	public class SampleGeneric<T1, T2>
	{
		
	}


	[TestClass]
	public class MixinExtensionTest
	{
		MetaCreator_UnitTest.MixinExtensionTest q;
		SampleGeneric<string, SampleGeneric<long, short>> qq;

		[TestMethod]
		public void Should_return_type_name_with_regards_to_csharp_syntax()
		{
			Assert.AreEqual("MetaCreator_UnitTest.MixinExtensionTest", MixinExtension.CSharpTypeIdentifier(typeof(MixinExtensionTest)));

			Assert.AreEqual("System.String", MixinExtension.CSharpTypeIdentifier(typeof(System.String)));
			Assert.AreEqual("System.Int32", MixinExtension.CSharpTypeIdentifier(typeof(int)));

		}

		[TestMethod]
		public void Should_return_for_generic_class()
		{
			Assert.AreEqual("MetaCreator_UnitTest.SampleGeneric<System.String, MetaCreator_UnitTest.SampleGeneric<System.Int64, System.Int16>>", MixinExtension.CSharpTypeIdentifier(typeof(SampleGeneric<System.String, SampleGeneric<long, short>>)));

		}
	}
}

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

			Assert.AreEqual("string", MixinExtension.CSharpTypeIdentifier(typeof(System.String)));
			Assert.AreEqual("int", MixinExtension.CSharpTypeIdentifier(typeof(int)));

		}

		[TestMethod]
		public void Should_return_for_generic_class()
		{
			Assert.AreEqual("MetaCreator_UnitTest.SampleGeneric<string, MetaCreator_UnitTest.SampleGeneric<long, short>>", MixinExtension.CSharpTypeIdentifier(typeof(SampleGeneric<System.String, SampleGeneric<long, short>>)));

		}

		[TestMethod]
		public void Should_return_for_generic_class_without_known_namespaces()
		{
			Assert.AreEqual("MetaCreator_UnitTest.SampleGeneric<string, MetaCreator_UnitTest.SampleGeneric<long, short>>", MixinExtension.CSharpTypeIdentifier(typeof(SampleGeneric<System.String, SampleGeneric<long, short>>)));

		}
	}
}

using System.Collections.Generic;

using MetaCreator.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MetaCreator_UnitTest
{
	public class SampleGeneric<T1, T2>
	{
		
	}


	[TestClass]
	public class CSharpTypeIdentifierTest
	{
		//SampleGeneric<string, SampleGeneric<long, short>> qq;

		[TestMethod]
		public void Should_return_type_name_with_regards_to_csharp_syntax()
		{
			Assert.AreEqual("MetaCreator_UnitTest.CSharpTypeIdentifierTest", typeof(CSharpTypeIdentifierTest).CSharpTypeIdentifier());

			Assert.AreEqual("string", typeof(System.String).CSharpTypeIdentifier());
			Assert.AreEqual("int", typeof(int).CSharpTypeIdentifier());

		}

		[TestMethod]
		public void Should_return_for_generic_class()
		{
			Assert.AreEqual("MetaCreator_UnitTest.SampleGeneric<string, MetaCreator_UnitTest.SampleGeneric<long, short>>", typeof(SampleGeneric<System.String, SampleGeneric<long, short>>).CSharpTypeIdentifier());

		}

		[TestMethod]
		public void Should_return_for_generic_class_without_known_namespaces()
		{
			Assert.AreEqual("MetaCreator_UnitTest.SampleGeneric<string, MetaCreator_UnitTest.SampleGeneric<long, short>>", typeof(SampleGeneric<System.String, SampleGeneric<long, short>>).CSharpTypeIdentifier());

		}

		[TestMethod]
		public void Should_provide_generic_type_definition()
		{
			Assert.AreEqual("IDictionary<TKey, TValue>", typeof(IDictionary<,>).CSharpTypeIdentifier(null, "System.Collections.Generic"));
			Assert.AreEqual("IList<T>", typeof(IList<>).CSharpTypeIdentifier(null, "System.Collections.Generic"));

			var cfg = new SharpGenerator.TypeIdentifierConfig
			{
				UseNamedTypeParameters = false,
				Imports = new[]
				{
					"System.Collections.Generic",
				},
			};

			Assert.AreEqual("IDictionary<, >", typeof(IDictionary<,>).CSharpTypeIdentifier(cfg));
			Assert.AreEqual("IList<>", typeof(IList<>).CSharpTypeIdentifier(cfg));
		}

		[TestMethod]
		public void Should_generate_type_name_correctly_for_kvp_type_def()
		{
			Assert.AreEqual("KeyValuePair<, >", typeof(KeyValuePair<,>).CSharpTypeIdentifier(new SharpGenerator.TypeIdentifierConfig
			{
				Imports = new[]{
					"System.Collections.Generic",
				},
				UseNamedTypeParameters = false,
			}));
		}

		[TestMethod]
		public void Should_generate_type_name_correctly_for_kvp()
		{
			Assert.AreEqual("KeyValuePair<int, int>[]", typeof(KeyValuePair<int, int>[]).CSharpTypeIdentifier("MySpace", "System.Collections.Generic"));
		}
	}
}

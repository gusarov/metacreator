using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MetaCreator.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaCreator_UnitTest.Reintrospection
{
	[TestClass]
	public class TypeNameTest
	{
		[TestMethod]
		public void Should_return_namespace_with_imports()
		{
			Assert.AreEqual("System.", SharpGenerator.GetNamespace("System", "My"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("System", "My", "System"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("System", "My", "System", "System.IO"));
			Assert.AreEqual("System.", SharpGenerator.GetNamespace("System", "My", "MySpace", "System.IO"));
			Assert.AreEqual("System.", SharpGenerator.GetNamespace("System", "My", "MySpace", "MySpace2"));

			Assert.AreEqual("System.Xaml.", SharpGenerator.GetNamespace("System.Xaml", "My", "MySpace.Xaml", "MySpace2"));
			Assert.AreEqual("System.Xaml.", SharpGenerator.GetNamespace("System.Xaml", "My", "System2", "MySpace2"));
			Assert.AreEqual("System.Xaml.", SharpGenerator.GetNamespace("System.Xaml", "My", "System.Xml", "MySpace2"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("System.Xaml", "My", "System.Xaml", "MySpace2"));
			Assert.AreEqual("System.Xaml.", SharpGenerator.GetNamespace("System.Xaml", "My", "System", "MySpace2"));
		}

		[TestMethod]
		public void Should_return_namespace_considering_current_space()
		{
			Assert.AreEqual("a.b.c.d.", SharpGenerator.GetNamespace("a.b.c.d", "My"));
			Assert.AreEqual("a.b.c.d.", SharpGenerator.GetNamespace("a.b.c.d", "My", "a"));
			Assert.AreEqual("a.b.c.d.", SharpGenerator.GetNamespace("a.b.c.d", "My", "a.b"));
			Assert.AreEqual("a.b.c.d.", SharpGenerator.GetNamespace("a.b.c.d", "My", "a.b.c"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("a.b.c.d", "My", "a.b.c.d"));

			Assert.AreEqual("b.c.d.", SharpGenerator.GetNamespace("a.b.c.d", "a"));
			Assert.AreEqual("c.d.", SharpGenerator.GetNamespace("a.b.c.d", "a.b"));
			Assert.AreEqual("d.", SharpGenerator.GetNamespace("a.b.c.d", "a.b.c"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("a.b.c.d", "a.b.c.d"));
		}

		[TestMethod]
		public void Should_case1()
		{
			var q = new[]
			{
				"Plasma",
				"PlasmaTests.Sample",
				"PlasmaTests.Precompiler",
				"System",
				"",
			};
			Assert.AreEqual("Sample.Proxy.", SharpGenerator.GetNamespace("PlasmaTests.Sample.Proxy", "PlasmaTests", q));
			Assert.AreEqual("Sample.Proxy.", SharpGenerator.GetNamespace("PlasmaTests.Sample.Proxy", "PlasmaTests.Precompiler", q));
		}
	}
}

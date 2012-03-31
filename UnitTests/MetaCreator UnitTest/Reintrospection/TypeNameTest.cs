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
			Assert.AreEqual("System.", SharpGenerator.GetNamespace("System"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("System", "System"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("System", "System", "System.IO"));
			Assert.AreEqual("System.", SharpGenerator.GetNamespace("System", "MySpace", "System.IO"));
			Assert.AreEqual("System.", SharpGenerator.GetNamespace("System", "MySpace", "MySpace2"));

			Assert.AreEqual("System.Xaml.", SharpGenerator.GetNamespace("System.Xaml", "MySpace.Xaml", "MySpace2"));
			Assert.AreEqual("System.Xaml.", SharpGenerator.GetNamespace("System.Xaml", "System2", "MySpace2"));
			Assert.AreEqual("System.Xaml.", SharpGenerator.GetNamespace("System.Xaml", "System.Xml", "MySpace2"));
			Assert.AreEqual("", SharpGenerator.GetNamespace("System.Xaml", "System.Xaml", "MySpace2"));
			Assert.AreEqual("Xaml.", SharpGenerator.GetNamespace("System.Xaml", "System", "MySpace2"));
		}
	}
}

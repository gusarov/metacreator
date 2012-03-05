using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MetaCreator_Acceptance.Properties;

namespace MetaCreator_Acceptance.Create_additional_files
{
	[TestClass]
	public class Additional_files : Acceptance_base_tests
	{
		[TestMethod]
		public void Should_5_add_one_file_to_root()
		{
			File.WriteAllText("test.cs", Resources._sample_additional_files);
			Build();

			var test = LoadAssembly().GetType("Test");
			Assert.IsNotNull(test.GetMethod("Method1"));
			Assert.IsNotNull(test.GetMethod("Method2"));

			Assert.IsTrue(File.Exists("obj\\debug\\test.g.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\test_add_1.g.cs"));
		}

		[TestMethod]
		public void Should_10_add_one_file()
		{
			Directory.CreateDirectory("sub");
			File.WriteAllText("sub\\test.cs", Resources._sample_additional_files);
			Build(new Params { Compile = { "sub\\*.cs" } });

			var test = LoadAssembly().GetType("Test");
			Assert.IsNotNull(test.GetMethod("Method1"));
			Assert.IsNotNull(test.GetMethod("Method2"));

			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test.g.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test_add_1.g.cs"));
		}

		[TestMethod]
		public void Should_20_add_two_files()
		{
			Directory.CreateDirectory("sub");
			File.WriteAllText("sub\\test.cs", Resources._sample_additional_files2);
			Build(new Params { Compile = { "sub\\*.cs" } });

			var test = LoadAssembly().GetType("Test");
			Assert.IsNotNull(test.GetMethod("Method1"));
			Assert.IsNotNull(test.GetMethod("Method2"));
			Assert.IsNotNull(test.GetMethod("Method3"));

			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test.g.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test_add_1.g.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test_add_2.g.cs"));
		}

		[TestMethod]
		public void Should_30_add_named_file()
		{
			Directory.CreateDirectory("sub");
			File.WriteAllText("sub\\test.cs", Resources._sample_additional_files3);
			Build(new Params { Compile = { "sub\\*.cs" } });

			var test = LoadAssembly().GetType("Test");
			Assert.IsNotNull(test.GetMethod("Method1"));
			Assert.IsNotNull(test.GetMethod("Method2"));
			Assert.IsNotNull(test.GetMethod("MethodNamed"));
			Assert.IsNotNull(test.GetMethod("Method3"));

			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test.g.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test_add_1.g.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\my_file_name.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test_add_2.g.cs"));
		}

		[TestMethod]
		public void Should_40_add_named_file_included_project()
		{
			Directory.CreateDirectory("sub");
			File.WriteAllText("sub\\test.cs", Resources._sample_additional_files4);
			Build(new Params { Compile = { "sub\\test.cs", "my_file_name.cs" } }); // included in project

			var test = LoadAssembly().GetType("Test");
			Assert.IsNotNull(test.GetMethod("Method1"));
			Assert.IsNotNull(test.GetMethod("Method2"));
			Assert.IsNotNull(test.GetMethod("MethodNamed"));
			Assert.IsNotNull(test.GetMethod("Method3"));

			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test.g.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test_add_1.g.cs"));
			Assert.IsTrue(File.Exists("my_file_name.cs"));
			Assert.IsTrue(File.Exists("obj\\debug\\sub\\test_add_2.g.cs"));
		}
	}
}

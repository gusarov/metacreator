﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MetaCreator_Acceptance.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MetaCreator_Acceptance.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using MetaCreator;
        ///
        ///public static class MyMetaExtensions
        ///{
        ///	public static void Test(){}
        ///
        ///	public static void MakeMyMagic(this IMetaWriter writer)
        ///	{
        ///		string msg = &quot;!!&quot;;
        ///		writer.WriteLine(@&quot;System.Console.WriteLine(&quot;&quot;&quot; + msg + @&quot;&quot;&quot;);&quot;);
        ///	}
        ///}.
        /// </summary>
        internal static string _Meta_blocks_as_single_common {
            get {
                return ResourceManager.GetString("_Meta_blocks_as_single_common", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public partial class Test
        ///{
        ///	public void Method1(){}
        ///
        ///	/*!
        ///
        ///	Engine.AddToCompile(@&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void Method2(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	*/
        ///}.
        /// </summary>
        internal static string _sample_additional_files {
            get {
                return ResourceManager.GetString("_sample_additional_files", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public partial class Test
        ///{
        ///	public void Method1(){}
        ///
        ///	/*!
        ///
        ///	Engine.AddToCompile(@&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void Method2(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	Engine.AddToCompile(@&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void Method3(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	*/
        ///}.
        /// </summary>
        internal static string _sample_additional_files2 {
            get {
                return ResourceManager.GetString("_sample_additional_files2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public partial class Test
        ///{
        ///	public void Method1(){}
        ///
        ///	/*!
        ///
        ///	Engine.AddToCompile(@&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void Method2(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	Engine.AddToCompile(&quot;my_file_name.cs&quot;, @&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void MethodNamed(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	Engine.AddToCompile(@&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void Method3(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	*/
        ///}.
        /// </summary>
        internal static string _sample_additional_files3 {
            get {
                return ResourceManager.GetString("_sample_additional_files3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public partial class Test
        ///{
        ///	public void Method1(){}
        ///
        ///	/*!
        ///
        ///	Engine.AddToCompile(@&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void Method2(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	Engine.AddToCompile(true, &quot;my_file_name.cs&quot;, @&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void MethodNamed(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	Engine.AddToCompile(@&quot;
        ///
        ///public partial class Test
        ///{
        ///	public void Method3(){}
        ///}
        ///
        ///&quot;);
        ///
        ///	*/
        ///}.
        /// </summary>
        internal static string _sample_additional_files4 {
            get {
                return ResourceManager.GetString("_sample_additional_files4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///using System.Linq;
        ///using System.Text;
        ///using Microsoft.VisualStudio.TestTools.UnitTesting;
        ///using System.IO;
        ///using MetaCreator;
        ///
        ///public static class SampleExtensions
        ///{
        ///	public static void ExtensionSample1(IWriter writer)
        ///	{
        ///		writer.WriteLine(&quot;return 777&quot;);
        ///	}
        ///
        ///	public static void ExtensionSample2(IWriter writer, string arg1)
        ///	{
        ///		writer.WriteLine(&quot;return \&quot;&quot; + arg1 + &quot;\&quot;&quot;);
        ///	}
        ///
        ///	public static void ExtensionSample3(IWriter writer, string arg1 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _sample_extensions_common {
            get {
                return ResourceManager.GetString("_sample_extensions_common", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///using System.Linq;
        ///using System.Text;
        ///using Microsoft.VisualStudio.TestTools.UnitTesting;
        ///using System.IO;
        ///using MetaCreator;
        ///
        ///public static class Test
        ///{
        ///	public static object TestExtensionSample1()
        ///	{
        ///		/*# ExtensionSample1 */
        ///	}
        ///
        ///	public static object TestExtensionSample2()
        ///	{
        ///		/*# ExtensionSample2  */
        ///	}
        ///
        ///	public static object TestExtensionSample3_ok()
        ///	{
        ///		/*# ExtensionSample3 ok */
        ///	}
        ///
        ///	public static object TestExtensionSample3_k [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _sample_extensions_test {
            get {
                return ResourceManager.GetString("_sample_extensions_test", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///using System;
        ///using System.ComponentModel;
        ///
        ///public class Observable : INotifyPropertyChanged
        ///{
        ///	public event PropertyChangedEventHandler PropertyChanged;
        ///
        ///	protected void OnPropertyChanged(string name)
        ///	{
        ///		OnPropertyChanged(new PropertyChangedEventArgs(name));
        ///	}
        ///
        ///	protected void OnPropertyChanged(PropertyChangedEventArgs e)
        ///	{
        ///		var handler = PropertyChanged;
        ///		if (handler != null)
        ///		{
        ///			handler(this, e);
        ///		}
        ///	}
        ///
        ///	int _testProperty2;
        ///
        ///	public int TestProperty2
        ///	{
        ///		get { return [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _sample_mixin_events_base {
            get {
                return ResourceManager.GetString("_sample_mixin_events_base", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.ComponentModel;
        ///
        ///public class Base
        ///{
        ///}
        ///
        ///public class Test : Base /*= &quot;: INotifyPropertyChanged &quot; */
        ///{
        ///	/*# Mixin&lt;Observable&gt; */
        ///
        ///	int _testProperty;
        ///
        ///	public int TestProperty
        ///	{
        ///		get { return _testProperty; }
        ///		set
        ///		{
        ///			_testProperty = value;
        ///			OnPropertyChanged(&quot;TestProperty&quot;);
        ///		}
        ///	}
        ///
        ///}
        ///.
        /// </summary>
        internal static string _sample_mixin_events_exec {
            get {
                return ResourceManager.GetString("_sample_mixin_events_exec", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using n;
        ///
        ///namespace n
        ///{
        ///	public class T&lt;T1, T2&gt;
        ///	{
        ///		
        ///	}
        ///}.
        /// </summary>
        internal static string _sampleIntroscopeCommon {
            get {
                return ResourceManager.GetString("_sampleIntroscopeCommon", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using n;
        ///
        ///public static class Test
        ///{
        ///	public static string TestA
        ///	{
        ///		get { return &quot;/*= SharpGenerator.CSharpTypeIdentifier(typeof(T&lt;T&lt;int, System.Version&gt;, System.Text.RegularExpressions.Regex&gt;)) */&quot;; }
        ///	}
        ///
        ///	public static string TestB
        ///	{
        ///		get { return &quot;/*= SharpGenerator.CSharpTypeIdentifier(typeof(T&lt;T&lt;int, System.Version&gt;, System.Text.RegularExpressions.Regex&gt;), Engine.Imports) */&quot;; }
        ///	}
        ///
        ///	public static string TestC
        ///	{
        ///		get { return &quot;/*= SharpGenerator.CSharpTypeIdentifier [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _sampleIntroscopeTests {
            get {
                return ResourceManager.GetString("_sampleIntroscopeTests", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using n;
        ///
        ///public static class Test
        ///{
        ///	public static string TestA
        ///	{
        ///		get { return &quot;/*= SharpGenerator.CSharpTypeIdentifier(typeof(T&lt;T&lt;int, System.Version&gt;, System.Text.RegularExpressions.Regex&gt;)) */&quot;; }
        ///	}
        ///
        ///	public static string TestB
        ///	{
        ///		get { return &quot;/*= SharpGenerator.CSharpTypeIdentifier(typeof(T&lt;T&lt;int, System.Version&gt;, System.Text.RegularExpressions.Regex&gt;), Engine.Imports) */&quot;; }
        ///	}
        ///
        ///	public static string TestC
        ///	{
        ///		get { return &quot;/*= SharpGenerator.CSharpTypeIdentifier [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _sampleIntroscopeTests1 {
            get {
                return ResourceManager.GetString("_sampleIntroscopeTests1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public class Base
        ///{
        ///	public string BaseMethod(string arg)
        ///	{
        ///		return &quot;Base_&quot; + arg;
        ///	}
        ///}
        ///
        ///public class Derived : Base /*= &quot;, IBeh&quot; */
        ///{
        ///	/*# Mixin&lt;IBeh, BehImpl&gt; */
        ///
        ///	public string DerivedMethod(string arg)
        ///	{
        ///		return &quot;Derived_&quot; + arg;
        ///	}
        ///}
        ///
        ///.
        /// </summary>
        internal static string _SampleMinh {
            get {
                return ResourceManager.GetString("_SampleMinh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public class Base
        ///{
        ///	public string BaseMethod(string arg)
        ///	{
        ///		return &quot;Base_&quot; + arg;
        ///	}
        ///}
        ///
        ///public class Derived : Base
        ///{
        ///	/*# Mixin&lt;BehImpl&gt; */
        ///
        ///	public string DerivedMethod(string arg)
        ///	{
        ///		return &quot;Derived_&quot; + arg;
        ///	}
        ///}
        ///
        ///.
        /// </summary>
        internal static string _SampleMinh2 {
            get {
                return ResourceManager.GetString("_SampleMinh2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public interface IBeh
        ///{
        ///	string BehMethod(string arg);
        ///}
        ///
        ///public class BehImpl : IBeh
        ///{
        ///	public string BehMethod(string arg)
        ///	{
        ///		return &quot;BehImpl_&quot; + arg;
        ///	}
        ///}.
        /// </summary>
        internal static string _SampleMinhCommon {
            get {
                return ResourceManager.GetString("_SampleMinhCommon", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public class BehImpl
        ///{
        ///	public string BehMethod(string arg)
        ///	{
        ///		return &quot;BehImpl_&quot; + arg;
        ///	}
        ///}.
        /// </summary>
        internal static string _SampleMinhCommon2 {
            get {
                return ResourceManager.GetString("_SampleMinhCommon2", resourceCulture);
            }
        }
    }
}

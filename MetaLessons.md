# Lesson 1 #

You can write metacode on C# inside multiline comments block:

```
/*!
WriteLine("var q = 5;");
*/
```

First symbol inside comment block indicates the type of block

# Lesson 2 #

There are 4 types of block that is very close to T4.

  1. ! - generator method body
  1. = - expression
  1. + - generator class body
  1. @ - special switchers and commands

# Lesson 3 #

In the "!" block you type a body of the method, that will be executed to create a code. The most dummy way to generate code is using `WriteLine` method:
```
Write(string);
WriteLine(string);
Write(string, params object[]);
WriteLine(string, params object[]);
WriteLine();
```

e.g.
```
/*!
Write("// this is my metacode");
*/
```

Try to play around with `/*! GetType().GetMethods() */` ;)

See also [Generator Class Reference](GeneratorClassReference.md)

# Lesson 4 #

You can generate code with expression syntax instead:
```
Console.WriteLine(/*= new Random().Next() */);
```

# Lesson 5 #

For some more exotic cases, you can use a set of extenders:

  1. `/*@ reference ..\..\ThirdPartyCompileTimeMetaGenerator.dll */` - reference that exists only for meta code. The referenced assembly will not be copied to production output. Note, that inside your metacode you can use any references of your production assembly without any special markup.
  1. `/*@ using System.Web */` - import namespace for metacode
  1. `/*@ StringInterpolation on */`
  1. `/*@ ErrorRemap off */` - MetaCreator tries to remap compile errors and exceptions from intermediate file 'obj\Debug\sample.g.cs' to original file 'sample.cs'. With this extender you can disable any remap, and every error or exception will reference an intermediate file. This can simplify a debugging of difficult cases.

Binary switchers for extenders:
  * TRUE: enable, enabled, on, 1, true, yes, (empty string)
  * FALSE: disable, disabled, off, 0, false, no

# Lesson 6 #

Every metablock actually is a parts of generator method... so, you can use variables, created inside another block...
```
/*!
var properties = typeof(SomeClass).GetProperties();
foreach (var property in properties)
{
*/

/*! if (property.PropertyType.IsValueType) WriteLine("// it is value type") */
public string /*= property.Name */ { get; set; }

/*!}*/
```
Also, you can notice, that usual code between `*/` and `/*!` is just a metablock `WriteLine("with code between");`
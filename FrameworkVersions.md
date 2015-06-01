# Introduction #

Next configurations are supported

# Details #

|Tools|Target|Metacode C# Version|
|:----|:-----|:------------------|
|3.5 (VS2008)|2.0..3.5|3.5                |
|3.5 (VS2008)|Compact Framework|3.5                |
|4.0 (VS2010)|2.0..4.0|3.5..4.0           |

By default Metacode C# Version equals to the target framework. You can override it by

`/*@ csharpversion v3.5 */`
or
`/*@ csharpversion v4.0 */`

Note that for target 2.0 and 3.0 you always should do that. Otherwise:

```
Error	3	The "ExecuteMetaCreator" task failed unexpectedly.
System.InvalidOperationException: Compiler executable file csc.exe cannot be found.
   at System.CodeDom.Compiler.RedistVersionInfo.GetCompilerPath(IDictionary`2 provOptions, String compilerExecutable)
   at Microsoft.CSharp.CSharpCodeGenerator.FromFileBatch(CompilerParameters options, String[] fileNames)
```
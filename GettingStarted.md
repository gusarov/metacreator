MetaCreator can be installed in the same way, as PostSharp or [LightPersist](http://code.google.com/p/lightpersist/) because this projects use MSBuild targets to integrate into building process.

  1. Personal system-wide. Useful if you create project alone. Otherwise, every team member and builder should install this. Simply run [MetaCreator.msi](MSI.md) and proceed an installation.
  1. Per-project. As a third-party reference. [Open your \*.csproj](OpenCsproj.md), and add as the last line something like this:
```
  <Import Project="..\ThirdParty\MetaCreator.targets" />
```
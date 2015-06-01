## Jul 22 ##
Every assembly, referenced by your project are automatically referenced for meta code.

![http://clip2net.com/clip/m27927/1279825080-clip-6kb.png](http://clip2net.com/clip/m27927/1279825080-clip-6kb.png)

Every 'using' imports before 'namespace' are automatically imported for your metacode.

![http://clip2net.com/clip/m27927/1279825247-clip-5kb.png](http://clip2net.com/clip/m27927/1279825247-clip-5kb.png)

You can reference assembly for meta code manually. This assembly will not be copied to production output.

![http://clip2net.com/clip/m27927/1279825365-clip-14kb.png](http://clip2net.com/clip/m27927/1279825365-clip-14kb.png)

## Jul 17 ##

MSI Installator is created!

Actions:
  * Copy to %ProgramFiles%\MetaCreator
  * Install to GAC
  * NGEN MetaCreator.dll
  * Patch %ProgramFiles%\MSBuild\v3.5\Custom.After.Microsoft.Common.targets
  * Registry: HKLM\Software\Microsoft\VisualStudio\9.0\MSBuild\SafeImports)

## Jun 27 ##

For now, in build error, you can see correct message about errors for meta code.
There are references for original source file, recalculated line number and column number, to reflect this place in original source file.

![http://clip2net.com/clip/m27927/1277626779-clip-29kb.png](http://clip2net.com/clip/m27927/1277626779-clip-29kb.png)
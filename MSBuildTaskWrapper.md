MSBuildTaskWrapper it is the same project, as in [LightPersist](http://code.google.com/p/lightpersist/). Both VisualStudio and SharpDevelop locks dll with msbuild task, that described in `*.targets` file. `MetaCreator.targets` references `MetaCreator.dll` and if we'll call `MetaCreator.dll` dirrectly, than this project builds only once. Because after first building, IDE locks the target dll, and fails trying to replace this file on next build or rebuild. But we should introduce changes in MetaCreator.dll during development here.

The problem solved in a 2 part:
  1. `MetaCreator.targets` references abstract generic MSBuildTaskWrapper, kind of proxy MSBuild task, for transferring input and output.
  1. `MetaCreator.targets` create `MSBuildTaskWrapper.copy.dll` and use that dll instead of original build output. This allows you tu run Rebuild of MetaCreator solution several times.

Of cource, in production there are no MSBuildTaskWrapper. `MetaCreator.targets` decide what assembly to use by solution name. In [LightPersist](http://code.google.com/p/lightpersist/) I had created 2 files, `LightPersist.targets` & `LightPersist_dev.targets` for that purpose.
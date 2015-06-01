`MetaCreator.msi` performs next steps

  * 1. Install into `%programfiles%`
  * 2. `GAC`
  * 3. `NGEN`
  * 4. Register `MetaCreator.targets` as safe for Visual Studio in Registry (HKLM)
  * 5. Extend `Custom.Before.Microsoft.Common.targets` - msbuild plugin injection point
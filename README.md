# Robin
Open source emulation front end
To compile 
-Requires VS 2017 at least and .net 4.5
-Change output directory to something valid
-Install ado.net provider. Sqlite does not have an ado.net provider for VS17 yet. This guy does https://github.com/ErikEJ/SqlCeToolbox/wiki/EF6-workflow-with-SQLite-DDEX-provider.
-Install sqlite in the development machine GAC. The package that provides design-time components for VS is sqlite-netFx46-setup-bundle-x86-2015-1.0.105.2.exe at https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki all of the other packages will not let you view and edit the DB schema graphically. The alternative is to edit thes in xml.
-Install NuGet packages (usually the latest version) System.Data.SQlite System.Data.SQlite.Core System.Data.SQlite.EF6 System.Data.SQlite.Linq EntityFrameWork

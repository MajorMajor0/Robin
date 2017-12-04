# Robin
Open source emulation front end

To compile

	-Requires VS 2017 at least and .net 4.5

	-Change output directory to something valid

	-Install ado.net provider. Sqlite does not have an ado.net provider for VS17 yet.
	 This guy does https://github.com/ErikEJ/SqlCeToolbox/wiki/EF6-workflow-with-SQLite-DDEX-provider.

	-Install sqlite in the development machine GAC. The package that provides design-time components for
	 VS is at https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki. Read the descriptions
	 and get the correct package to include design time components. All of the other packages will not let
	 you view and edit the DB schema graphically. The alternative is to edit these in xml.

	-Restore NuGet packages

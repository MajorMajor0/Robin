# Robin
Robin is a open source emulation front end. There are better front ends--but probably not better databases. Robin includes a database that pulls together MAME, Datomatic/No Intro, OpenDB, GiantBomb, LaunchBox and GamesDB. This can be updated pretty easily in the GUI, and Robin will pull artwork from any or all of these sites as it is available. Robin prefers to pull from sites that distinguish between regions, since artwork by region can be different.

Robin will sort through your ROMS and pull out only the good ones, leaving behind the dupes and hacks. It then will present them to you as games, grouping the nearly identical versions of ROMs so that you can see through the mountains of crap.

About 26k ROMS boil down to 13k games. Of these, 11.9k are non-crap, playable video games.

The database includes flags for
	Crap
	Adult
	Unlicensed
	MESS mechanical
	USer rating (your rating)
	Beaten (by you)
	Playable
	
Most of the crap and unplayable have already been marked. But the GUI has tools to keep marking them and save to the database.

Development has more or less stopped, because teh work is too much for one person and was just too time consuming.But the database is pretty valuable and I wish somebody would make use of it. Please let me know in the issues if you do find a use.

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

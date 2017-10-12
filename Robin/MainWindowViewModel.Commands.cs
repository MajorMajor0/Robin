using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	partial class MainWindowViewModel
	{
		public Command DropCollectionCommand { get; set; }

		private void DropCollection()
		{

		}


		public Command SaveDataBaseCommand { get; set; }

		private void SaveDataBase()
		{
			R.Data.Save(true);
		}


		public Command AboutCommand { get; set; }

		private void About()
		{
			AboutBox aboutBox = new AboutBox();
		}

		public Command HelpCommand { get; set; }

		private void Help()
		{
			// TODO Launch help website
			Reporter.Report("Help is on the way.");
		}


		public Command PlayCommand { get; set; }

		private void Play()
		{
			SelectedDB?.Play();
		}

		private bool PlayCanExecute()
		{
			return SelectedDB != null && SelectedDB.Included;
		}

		//public Command GetSelectedArtCommand { get; set; }

		//private void GetSelectedArt1()
		//{
		//    //GetSelectedArt();
		//}

		//private bool GetSelectedArtCanExecute()
		//{
		//    return SelectedDB != null;
		//}


		public Command ReporterWindowCommand { get; set; }

		private void ReporterWindow()
		{
			ReporterWindow reporterWindow = new ReporterWindow();
		}


		public Command DatabaseWindowCommand { get; set; }

		private void DatabaseWindow()
		{
			DatabaseWindow databaseWindow = new DatabaseWindow();
		}


		public Command ClearFiltersCommand { get; set; }

		private void ClearFilters()
		{
			(MainBigSelection as AutoFilterCollection<Release>)?.ClearFilters();
			(MainBigSelection as AutoFilterCollection<Game>)?.ClearFilters();
			(MainBigSelection as AutoFilterCollection<Platform>)?.ClearFilters();
			(MainBigSelection as AutoFilterCollection<Emulator>)?.ClearFilters();
		}

		private bool ClearFiltersCanExecute()
		{
#if DEBUG
			Debug.WriteLine("CFCE");
#endif
			return MainBigSelection != null;
		}


		public Command MarkAsCrapCommand { get; set; }

		public Command MarkNotCrapCommand { get; set; }

		private void MarkAsCrap()
		{
			MarkCrap(true);
		}

		private bool MarkAsCrapCanExecute()
		{
			return SelectedDB != null && !SelectedDB.IsCrap;
		}

		private void MarkNotCrap()
		{
			MarkCrap(false);
		}

		private bool MarkNotCrapCanExecute()
		{
			return SelectedDB != null && SelectedDB.IsCrap;
		}

		private void MarkCrap(bool value)

		{
			IList idbList = SelectedDBs;

			foreach (IDBobject idbObject in idbList)
			{
				idbObject.IsCrap = value;
			}
		}


		public Command MarkPreferredCommand { get; set; }

		public Command MarkNotPreferredCommand { get; set; }

		private void MarkPreferred()
		{
			SelectedDB.Preferred = true;
		}

		private bool MarkPreferredCanExecute()
		{
			return SelectedDB != null && !(SelectedDB is Emulator) && !SelectedDB.Preferred;
		}

		private void MarkNotPreferred()
		{
			SelectedDB.Preferred = false;
		}

		private bool MarkNotPreferredCanExecute()
		{
			return SelectedDB != null && !(SelectedDB is Emulator) && SelectedDB.Preferred;
		}


		public Command MarkSelectedGameReleasePreferredCommand { get; set; }

		private void MarkSelectedGameReleasePreferred()
		{
			SelectedGameRelease.Preferred = true;
		}

		private bool MarkSelectedGameReleasePreferredCanExecute()
		{
			return SelectedGameRelease != null && !SelectedGameRelease.Preferred;
		}


		public Command MarkSelectedPlatformEmulatorPreferredCommand { get; set; }

		private void MarkSelectedPlatformEmulatorPreferred()
		{
			SelectedPlatformEmulator.MarkPreferred((SelectedDB as Platform));
		}

		private bool MarkSelectedPlatformEmulatorPreferredCanExecute()
		{
			return SelectedDB is Platform && SelectedPlatformEmulator != null && (SelectedDB as Platform).PreferredEmulator_ID != SelectedPlatformEmulator.ID;
		}


		public Command MarkSelectedEmulatorPlatformPreferredCommand { get; set; }

		private void MarkSelectedEmulatorPlatformPreferred()
		{
			SelectedEmulatorPlatform.MarkPreferred(SelectedDB as Emulator);
		}

		private bool MarkSelectedEmulatorPlatformPreferredCanExecute()
		{
			return SelectedDB is Emulator && SelectedEmulatorPlatform != null && (SelectedDB as Emulator).ID != SelectedEmulatorPlatform.PreferredEmulator_ID;
		}


		public Command MarkAsGameCommand { get; set; }

		public Command MarkNotGameCommand { get; set; }

		private void MarkAsGame()
		{
			MarkGame(true);
		}

		private bool MarkAsGameCanExecute()
		{
			return
				((SelectedDB is Game && !(SelectedDB as Game).IsGame)
				||
				(SelectedDB is Release && !(SelectedDB as Release).IsGame));
		}

		private bool MarkNotGameCanExecute()
		{
			return ((SelectedDB is Game && (SelectedDB as Game).IsGame)
				   ||
				   (SelectedDB is Release && (SelectedDB as Release).IsGame));
		}

		private void MarkNotGame()
		{
			MarkGame(false);
		}

		private void MarkGame(bool value)
		{
			IList idbList = SelectedDBs;
			if (SelectedDB.GetType().BaseType == typeof(Game))
			{
				foreach (Game game in idbList)
				{
					game.IsGame = value;
				}
			}

			if (SelectedDB.GetType().BaseType == typeof(Release))
			{
				foreach (Release release in idbList)
				{
					release.Game.IsGame = value;
				}
			}
		}


		public Command MarkAsBeatenCommand { get; set; }

		public Command MarkNotBeatenCommand { get; set; }

		private void MarkAsBeaten()
		{
			MarkBeaten(true);
		}

		private bool MarkAsBeatenCanExecute()
		{
			return SelectedDB != null && (((SelectedDB is Game) && !(SelectedDB as Game).IsBeaten) || (SelectedDB is Release) && !(SelectedDB as Release).IsBeaten);
		}

		private void MarkBeaten(bool value)
		{
			IList idbList = SelectedDBs;
			if (SelectedDB.GetType().BaseType == typeof(Game))
			{
				foreach (Game game in idbList)
				{
					game.IsBeaten = value;
				}
			}

			if (SelectedDB.GetType().BaseType == typeof(Release))
			{
				foreach (Release release in idbList)
				{
					release.Game.IsBeaten = value;
				}
			}
		}

		private bool MarkNotBeatenCanExecute()
		{
			return ((SelectedDB is Game) && (SelectedDB as Game).IsBeaten)
				   ||
				   (SelectedDB is Release) && (SelectedDB as Release).IsBeaten;
		}

		private void MarkNotBeaten()
		{
			MarkBeaten(false);
		}


		public Command GetAllArtCommand { get; set; }

		private void GetAllArt()
		{
			GetArt(false);
		}

		private bool GetAllArtCanExecute()
		{
			return MainBigSelection != null;
		}


		public Command GetSelectedArtCommand { get; set; }

		private void GetSelectedArt()
		{
			GetArt(true);
		}

		private bool GetSelectedArtCanExecute()
		{
			return SelectedDB != null;
		}


		private async void GetArt(bool selected)
		{
			int misCount = 0;
			int totalCount = 0;
			await Task.Run(() =>
			{
				Reporter.Report("Opening databases...");

				if (MainBigSelection == PlatformCollection)
				{
					R.Data.GDBPlatforms.Load();
				}
				else
				{
					R.Data.GDBReleases.Load();
					R.Data.GBReleases.Load();
					R.Data.OVGReleases.Load();
					R.Data.LBReleases.Include(x => x.LBImages).Load();
				}

				Reporter.Report("Scraping art files...");

				Directory.CreateDirectory(FileLocation.Art.BoxFront);
				Directory.CreateDirectory(FileLocation.Art.BoxBack);
				Directory.CreateDirectory(FileLocation.Art.Screen);
				Directory.CreateDirectory(FileLocation.Art.Banner);
				Directory.CreateDirectory(FileLocation.Art.Logo);
				Directory.CreateDirectory(FileLocation.Art.Console);

				List<IDBobject> list = new List<IDBobject>();

				if (selected)
				{
					// Must cache selected items in case user changes during scrape
					foreach (IDBobject idbObject in SelectedDBs)
					{
						list.Add(idbObject);
					}
				}
				else
				{
					dynamic MainBigThing = MainBigSelection;
					list.AddRange(MainBigThing.FilteredCollection);
				}

				int tryCount = 0;
				do
				{
					foreach (IDBobject idbObject in list)
					{
						if (idbObject.ScrapeArt(0) == -1)
						{
							misCount -= 1;
						}
						else
						{
							totalCount += 1;
						}
					}
				} while (misCount < 0 && ++tryCount < 5);

			});

			Reporter.Report("Added art for " + totalCount + " releases.");
		}


		public Command GetAllDataCommand { get; set; }

		private async void GetAllData()
		{
			// TODO: make this good for any idbobject and add get selected data command
			Stopwatch Watch1 = new Stopwatch();
			Watch1.Start();
			Stopwatch Watch2 = new Stopwatch();
			Watch2.Start();
			int j = 0;

			await Task.Run(() =>
			{
				Reporter.Report("Opening local cache...");

				R.Data.GBReleases.Load();
				R.Data.GDBReleases.Load();
				R.Data.OVGReleases.Load();
				Reporter.ReportInline(Watch1.Elapsed.ToString("ss") + " s");
				Watch1.Restart();

				int count = R.Data.Releases.Count();
				foreach (Release release in R.Data.Releases)
				{
					if (j++ % (count / 10) == 0)
					{
						Reporter.Report("Copying " + j + @" / " + count + " " + Watch1.Elapsed.ToString(@"m\:ss") + " elapsed.");
						Watch1.Restart();
					}
					release.CopyData();
				}
			});
			Reporter.Report("Finished. Copied data to " + j + " releases." + Watch2.Elapsed.ToString(@"m\:ss"));
			R.Data.Save(true);
		}

		private bool GetAllDataCanExecute()
		{
			return MainBigSelection != null && (MainBigSelection == ReleaseCollection || MainBigSelection == GameCollection);
		}


		public Command AddCollectionCommand { get; set; }

		private void AddCollection()
		{
			Collection collection = new Collection();
			collection.Title = "New friggin collection";
			collection.Type = "Game";
			CollectionList.Add(collection);
		}

		private bool AddCollectionCanExecute()
		{
			return MainBigSelection == CollectionList;
		}


		public Command RemoveFromCollectionCommand { get; set; }

		private void RemoveFromCollection()
		{
			int N = SelectedDBs.Count;
			for (int i = N - 1; i >= 0; i--)
			{
				IDBobject idbObject = SelectedDBs[i] as IDBobject;
				(MainBigSelection as Collection).Remove(idbObject);
			}
		}

		private bool RemoveFromCollectionCanExecute()
		{
			return MainBigSelection is Collection && SelectedDBs?.Count > 0;
		}


		public Command RemoveCollectionCommand { get; set; }

		private void RemoveCollection()
		{
			CollectionList.Remove(MainBigSelection as Collection);
		}

		private bool RemoveCollectionCanExecute()
		{
			return MainBigSelection is Collection;
		}


		private void InitializeCommands()
		{
			AddCollectionCommand = new Command(AddCollection, AddCollectionCanExecute, "Add collection", "Add a new custom collection to the list.");
			RemoveCollectionCommand = new Command(RemoveCollection, "Delete", "Remove this collection permanently.");

			DatabaseWindowCommand = new Command(DatabaseWindow, "Database Window", "Open the database window to manage databases.");
			ReporterWindowCommand = new Command(ReporterWindow, "Reporter Window", "Open the reporter window to view logs.");

			SaveDataBaseCommand = new Command(SaveDataBase, "Save database", "Save all changes from the current session to the database.");
			AboutCommand = new Command(About, "About", "Open the about box.");
			HelpCommand = new Command(About, "Help", "Navigate to the help website.");

			PlayCommand = new Command(Play, PlayCanExecute, "Play this", "Launch the selected game or release.");
			ClearFiltersCommand = new Command(ClearFilters, ClearFiltersCanExecute, "Clear", "Clear all filters.");

			MarkAsCrapCommand = new Command(MarkAsCrap, MarkAsCrapCanExecute, "Mark as crap", "Mark selected items as crap.");
			MarkNotCrapCommand = new Command(MarkNotCrap, MarkNotCrapCanExecute, "Mark not crap", "Mark selected items as not crap.");

			MarkPreferredCommand = new Command(MarkPreferred, MarkPreferredCanExecute, "Mark preferred", "Mark selected item as preferred. Only one item can be marked at a time");
			MarkNotPreferredCommand = new Command(MarkNotPreferred, MarkNotPreferredCanExecute, "Mark not preferred", "Mark selected item as not preferred. Only one item can be marked at a time");

			MarkSelectedGameReleasePreferredCommand = new Command(MarkSelectedGameReleasePreferred, MarkSelectedGameReleasePreferredCanExecute, "Mark preferred", "Mark selected release as preferred. Only one item can be marked at a time. The preferred release wil be launched by default");

			MarkSelectedPlatformEmulatorPreferredCommand = new Command(MarkSelectedPlatformEmulatorPreferred, MarkSelectedPlatformEmulatorPreferredCanExecute, "Mark preferred", "Mark selected emulator as preferred. Only one item can be marked at a time. The preferred emulator will be used to play games on this platform by default");

			MarkSelectedEmulatorPlatformPreferredCommand = new Command(MarkSelectedEmulatorPlatformPreferred, MarkSelectedEmulatorPlatformPreferredCanExecute, "Mark preferred", "Mark this  emulator as preferred for the selected platform. An emulator can be preferred for multiple platforms. If this emulator is preferred, it will be be used to play games on this platform by default");

			MarkAsGameCommand = new Command(MarkAsGame, MarkAsGameCanExecute, "Mark as game", "Mark selected items as games, i.e. not calculators or test cartridges");
			MarkNotGameCommand = new Command(MarkNotGame, MarkNotGameCanExecute, "Mark as not a game", "Mark selected items as not games, i.e. calculators or test cartridges.");

			MarkAsBeatenCommand = new Command(MarkAsBeaten, MarkAsBeatenCanExecute, "Mark as beaten", "Mark selected item as beaten.");
			MarkNotBeatenCommand = new Command(MarkNotBeaten, MarkNotBeatenCanExecute, "Mark not beaten", "Mark selected item as not beaten");

			GetAllArtCommand = new Command(GetAllArt, GetAllArtCanExecute, "Get all art", "Downloads art for all displayed items.");
			GetSelectedArtCommand = new Command(GetSelectedArt, GetSelectedArtCanExecute, "Get art for slected items", "Downloads art for items currently selected.");

			GetAllDataCommand = new Command(GetAllData, GetAllDataCanExecute, "Get all data", "Downloads metadata for all displayed items.");

			RemoveFromCollectionCommand = new Command(RemoveFromCollection, RemoveFromCollectionCanExecute, "Remove from collection", "Removes the current item from the collection permanently.");

		}
	}
}

//private async void GetReleaseArt(bool selected)
//{
//	int misCount = 0;
//	int totalCount = 0;
//	await Task.Run(() =>
//	{
//		Reporter.Report("Opening databases...");
//		R.Data.GDBReleases.Load();
//		R.Data.GBReleases.Load();
//		R.Data.OVGReleases.Load();
//		R.Data.LBReleases.Include(x => x.LBImages).Load();

//		Reporter.Report("Scraping art files...");

//		Directory.CreateDirectory(FileLocation.Art.BoxFront);
//		Directory.CreateDirectory(FileLocation.Art.BoxBack);
//		Directory.CreateDirectory(FileLocation.Art.Screen);
//		Directory.CreateDirectory(FileLocation.Art.Banner);
//		Directory.CreateDirectory(FileLocation.Art.Logo);

//		List<Release> releaseList = new List<Release>();

//		if (selected)
//		{
//			// Must cache selected items in case user changes during scrape
//			foreach (Release release in SelectedDBs)
//			{
//				releaseList.Add(release);
//			}
//		}
//		else
//		{
//			releaseList = R.Data.Releases.Local.ToList();
//		}

//		int tryCount = 0;
//		do
//		{
//			foreach (Release release in releaseList)
//			{
//				if (release.ScrapeArt(0) == -1)
//				{
//					misCount -= 1;
//				}
//				else
//				{
//					totalCount += 1;
//				}
//			}
//		} while (misCount < 0 && ++tryCount < 5);

//	});

//	Reporter.Report("Added art for " + totalCount + " releases.");
//}

//private async void GetPlatformArt(bool selected)
//{
//	using (GamesDB gamesDB = new GamesDB())
//	{
//		await Task.Run(() =>
//		{
//			List<Platform> list = new List<Platform>();
//			if (selected)
//			{
//				foreach (Platform platform in SelectedDBs)
//				{
//					list.Add(platform);
//				}
//			}
//			else
//			{
//				list = R.Data.Platforms.Local.ToList();
//			}

//			foreach (Platform platform in list)
//			{
//				Reporter.Tic("Getting " + platform.Title + " art...");
//				platform.ScrapeArt(LocalDB.GamesDB);
//				Reporter.Toc();
//			}
//		});
//	}
//	R.Data.Save(true);
//}
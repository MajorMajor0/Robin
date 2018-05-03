/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Robin
{
	partial class MainWindowViewModel
	{
		public Command SaveDataBaseCommand { get; set; }

		void SaveDataBase()
		{
			R.Data.Save(true);
			Mame.M.Data.Save(true);
		}


		public Command AboutCommand { get; set; }

		void About()
		{
			AboutBox aboutBox = new AboutBox();
		}

		public Command HelpCommand { get; set; }

		void Help()
		{
			Process.Start(@"http://robinemu.org/help/");
		}


		public Command PlayCommand { get; set; }

		void Play()
		{
			SelectedDB?.Play();
		}

		bool PlayCanExecute()
		{
			return SelectedDB != null && SelectedDB.Included;
		}


		public Command ReporterWindowCommand { get; set; }

		void ReporterWindow()
		{
			new ReporterWindow();
		}


		public Command DatabaseWindowCommand { get; set; }

		void DatabaseWindow()
		{
			new DatabaseWindow();
		}

		public Command ArtWindowCommand { get; set; }

		void ArtWindow()
		{
			new ArtWindow(SelectedDB as Release);
		}

		bool ArtWindowCanExecute()
		{
			return SelectedDB is Release;
		}

		public Command StatisticsWindowCommand { get; set; }

		void StatisticsWindow()
		{
			new StatisticsWindow();
		}


		public Command OptionsWindowCommand { get; set; }

		void OptionsWindow()
		{
			SaveSettings();
			OptionsWindow optionsWindow = new OptionsWindow();
			optionsWindow.Closed += new EventHandler(OptionsWindowClosed);
		}


		public Command ClearFiltersCommand { get; set; }

		void ClearFilters()
		{
			(MainBigSelection as AutoFilterCollection)?.ClearFilters();
		}

		bool ClearFiltersCanExecute()
		{
			return MainBigSelection != null;
		}


		public Command MarkAsCrapCommand { get; set; }

		public Command MarkNotCrapCommand { get; set; }

		void MarkAsCrap()
		{
			MarkCrap(true);
		}

		bool MarkAsCrapCanExecute()
		{
			return SelectedDB != null && !SelectedDB.IsCrap;
		}

		void MarkNotCrap()
		{
			MarkCrap(false);
		}

		bool MarkNotCrapCanExecute()
		{
			return SelectedDB != null && SelectedDB.IsCrap;
		}

		void MarkCrap(bool value)

		{
			IList idbList = SelectedDBs;

			foreach (IDBobject idbObject in idbList)
			{
				idbObject.IsCrap = value;
			}
		}


		public Command MarkPreferredCommand { get; set; }

		public Command MarkNotPreferredCommand { get; set; }

		void MarkPreferred()
		{
			SelectedDB.Preferred = true;
		}

		bool MarkPreferredCanExecute()
		{
			return SelectedDB != null && !(SelectedDB is Emulator) && !(SelectedDB is Game) && !SelectedDB.Preferred;
		}

		void MarkNotPreferred()
		{
			SelectedDB.Preferred = false;
		}

		bool MarkNotPreferredCanExecute()
		{
			return SelectedDB != null && !(SelectedDB is Emulator) && SelectedDB.Preferred;
		}


		public Command MarkSelectedGameReleasePreferredCommand { get; set; }

		void MarkSelectedGameReleasePreferred()
		{
			SelectedGameRelease.Preferred = true;
		}

		bool MarkSelectedGameReleasePreferredCanExecute()
		{
			return SelectedGameRelease != null && !SelectedGameRelease.Preferred;
		}


		public Command MarkSelectedPlatformEmulatorPreferredCommand { get; set; }

		void MarkSelectedPlatformEmulatorPreferred()
		{
			SelectedPlatformEmulator.MarkPreferred((SelectedDB as Platform));
		}

		bool MarkSelectedPlatformEmulatorPreferredCanExecute()
		{
			return SelectedDB is Platform && SelectedPlatformEmulator != null &&
				   (SelectedDB as Platform).PreferredEmulator_ID != SelectedPlatformEmulator.ID;
		}


		public Command MarkSelectedEmulatorPlatformPreferredCommand { get; set; }

		void MarkSelectedEmulatorPlatformPreferred()
		{
			SelectedEmulatorPlatform.MarkPreferred(SelectedDB as Emulator);
		}

		bool MarkSelectedEmulatorPlatformPreferredCanExecute()
		{
			return SelectedDB is Emulator && SelectedEmulatorPlatform != null &&
				   (SelectedDB as Emulator).ID != SelectedEmulatorPlatform.PreferredEmulator_ID;
		}


		public Command MarkAsGameCommand { get; set; }

		public Command MarkNotGameCommand { get; set; }

		void MarkAsGame()
		{
			MarkGame(true);
		}

		bool MarkAsGameCanExecute()
		{
			return
			((SelectedDB is Game && !(SelectedDB as Game).IsGame)
			 ||
			 (SelectedDB is Release && !(SelectedDB as Release).IsGame));
		}

		bool MarkNotGameCanExecute()
		{
			return ((SelectedDB is Game && (SelectedDB as Game).IsGame)
					||
					(SelectedDB is Release && (SelectedDB as Release).IsGame));
		}

		void MarkNotGame()
		{
			MarkGame(false);
		}

		void MarkGame(bool value)
		{
			IList idbList = SelectedDBs;

			if (SelectedDB is Game)
			{
				foreach (Game game in idbList)
				{
					game.IsGame = value;
				}
			}

			if (SelectedDB is Release)
			{
				foreach (Release release in idbList)
				{
					release.Game.IsGame = value;
				}
			}
		}


		public Command MarkAsAdultCommand { get; set; }

		public Command MarkNotAdultCommand { get; set; }

		void MarkAsAdult()
		{
			MarkAdult(true);
		}

		bool MarkAsAdultCanExecute()
		{
			return
				((SelectedDB is Game && !(SelectedDB as Game).IsAdult)
				||
				(SelectedDB is Release && !(SelectedDB as Release).IsAdult));
		}

		bool MarkNotAdultCanExecute()
		{
			return ((SelectedDB is Game && (SelectedDB as Game).IsAdult)
				   ||
				   (SelectedDB is Release && (SelectedDB as Release).IsAdult));
		}

		void MarkNotAdult()
		{
			MarkAdult(false);
		}

		void MarkAdult(bool value)
		{
			IList idbList = SelectedDBs;
			if (SelectedDB is Game)
			{
				foreach (Game game in idbList)
				{
					game.IsAdult = value;
				}
			}

			if (SelectedDB is Release)
			{
				foreach (Release release in idbList)
				{
					release.Game.IsAdult = value;
				}
			}
		}


		public Command MarkAsMessCommand { get; set; }

		public Command MarkNotMessCommand { get; set; }

		void MarkAsMess()
		{
			MarkMess(true);
		}

		bool MarkAsMessCanExecute()
		{
			return
				((SelectedDB is Game && !(SelectedDB as Game).IsMess)
				||
				(SelectedDB is Release && !(SelectedDB as Release).IsMess));
		}

		bool MarkNotMessCanExecute()
		{
			return ((SelectedDB is Game && (SelectedDB as Game).IsMess)
				   ||
				   (SelectedDB is Release && (SelectedDB as Release).IsMess));
		}

		void MarkNotMess()
		{
			MarkMess(false);
		}

		void MarkMess(bool value)
		{
			IList idbList = SelectedDBs;
			if (SelectedDB is Game)
			{
				foreach (Game game in idbList)
				{
					game.IsMess = value;
				}
			}

			if (SelectedDB is Release)
			{
				foreach (Release release in idbList)
				{
					release.Game.IsMess = value;
				}
			}
		}


		public Command MarkAsBeatenCommand { get; set; }

		public Command MarkNotBeatenCommand { get; set; }

		void MarkAsBeaten()
		{
			MarkBeaten(true);
		}

		bool MarkAsBeatenCanExecute()
		{
			return SelectedDB != null && (((SelectedDB is Game) && !(SelectedDB as Game).IsBeaten) || (SelectedDB is Release) && !(SelectedDB as Release).IsBeaten);
		}

		void MarkBeaten(bool value)
		{
			IList idbList = SelectedDBs;
			if (SelectedDB is Game)
			{
				foreach (Game game in idbList)
				{
					game.IsBeaten = value;
				}
			}

			if (SelectedDB is Release)
			{
				foreach (Release release in idbList)
				{
					release.Game.IsBeaten = value;
				}
			}
		}

		bool MarkNotBeatenCanExecute()
		{
			return ((SelectedDB is Game) && (SelectedDB as Game).IsBeaten)
				   ||
				   (SelectedDB is Release) && (SelectedDB as Release).IsBeaten;
		}

		void MarkNotBeaten()
		{
			MarkBeaten(false);
		}


		public Command GetAllArtCommand { get; set; }

		void GetAllArt()
		{
			GetArt(false);
		}

		bool GetAllArtCanExecute()
		{
			return MainBigSelection != null;
		}


		public Command GetSelectedArtCommand { get; set; }

		void GetSelectedArt()
		{
			GetArt(true);
		}

		bool GetSelectedArtCanExecute()
		{
			return SelectedDB != null;
		}


		void GetArt(bool selected)
		{
			TaskInProgress = true;
			tokenSource = new CancellationTokenSource();
			FileLocation.CreateDirectories();

			if (MainBigSelection == PlatformCollection)
			{
				GetPlatformArt(selected);
			}
			else
			{
				GetReleaseArt(selected);
			}
		}

		async void GetReleaseArt(bool selected)
		{
			try
			{
				// Cache the art count prior to scraping to find out how many we get
				int boxFrontCount = Directory.GetFiles(FileLocation.Art.BoxFront).Count();
				int boxBackCount = Directory.GetFiles(FileLocation.Art.BoxBack).Count();
				int bannerCount = Directory.GetFiles(FileLocation.Art.Banner).Count();
				int screenCount = Directory.GetFiles(FileLocation.Art.Screen).Count();
				int logoCount = Directory.GetFiles(FileLocation.Art.Logo).Count();

				await Task.Run(() =>
				{
					Reporter.Report("Opening databases...");

					R.Data.GDBReleases.Load();
					R.Data.GBReleases.Load();
					R.Data.OVGReleases.Load();
					R.Data.LBReleases.Include(x => x.LBImages).Load();

					Reporter.Report("Scraping art files...");

					List<IDBobject> list = new List<IDBobject>();

					// Cache selected items in case the user changes them during scrape
					// If items are selected, cache them
					if (selected)
					{
						foreach (IDBobject idbObject in SelectedDBs)
						{
							list.Add(idbObject);
						}
					}

					// If no items are selected, cache the entire collection
					else
					{
						dynamic MainBigThing = MainBigSelection;
						list.AddRange(MainBigThing.FilteredCollection);
					}

					// Scrape art in the order of most usefull
					if (Properties.Settings.Default.ScrapeReleaseBoxFront)
					{
						GetArtSub(list, ArtType.BoxFront, 0);
					}

					if (Properties.Settings.Default.ScrapeReleaseLogo)
					{
						GetArtSub(list, ArtType.Logo, 0);
					}

					if (Properties.Settings.Default.ScrapeReleaseScreen)
					{
						GetArtSub(list, ArtType.Screen, 0);
					}

					if (Properties.Settings.Default.ScrapeReleaseBanner)
					{
						GetArtSub(list, ArtType.Banner, 0);
					}

					if (Properties.Settings.Default.ScrapeReleaseBoxBack)
					{
						GetArtSub(list, ArtType.BoxBack, 0);
					}
				});

				boxFrontCount = Directory.GetFiles(FileLocation.Art.BoxFront).Count() - boxFrontCount;
				boxBackCount = Directory.GetFiles(FileLocation.Art.BoxBack).Count() - boxBackCount;
				bannerCount = Directory.GetFiles(FileLocation.Art.Banner).Count() - bannerCount;
				screenCount = Directory.GetFiles(FileLocation.Art.Screen).Count() - screenCount;
				logoCount = Directory.GetFiles(FileLocation.Art.Logo).Count() - logoCount;

				Reporter.Report($"Added {boxFrontCount} box front art images.");
				Reporter.Report($"Added {boxBackCount} box back art images.");
				Reporter.Report($"Added {bannerCount} banner images.");
				Reporter.Report($"Added {logoCount} clear logos.");
				Reporter.Report($"Added {screenCount} screenshots.");
			}

			catch { }

			finally
			{
				TaskInProgress = false;
			}
		}

		async void GetPlatformArt(bool selected)
		{
			try
			{
				// Cache the number of art files so we can see how many we got
				int platformArtCount = Directory.GetFiles(FileLocation.Art.Console).Count();

				await Task.Run(() =>
				{
					Reporter.Report("Opening databases...");

					R.Data.GDBPlatforms.Load();

					Reporter.Report("Scraping art files...");

					List<IDBobject> list = new List<IDBobject>();

					// Cache selected items in case the user changes them during scrape
					// If items are selected, cache them
					if (selected)
					{
						foreach (IDBobject idbObject in SelectedDBs)
						{
							list.Add(idbObject);
						}
					}

					// If no items are selected, cache the entire collection
					else
					{
						dynamic MainBigThing = MainBigSelection;
						list.AddRange(MainBigThing.FilteredCollection);
					}

					// Scrape art in the order of most usefull
					if (Properties.Settings.Default.ScrapePlatformConsole)
					{
						GetArtSub(list, ArtType.Console, 0);

					}

					if (Properties.Settings.Default.ScrapePlatformController)
					{
						GetArtSub(list, ArtType.Controller, 0);
					}

					if (Properties.Settings.Default.ScrapePlatformBoxFront)
					{
						GetArtSub(list, ArtType.BoxFront, 0);
					}

					if (Properties.Settings.Default.ScrapePlatformBanner)
					{
						GetArtSub(list, ArtType.Banner, 0);
					}

					if (Properties.Settings.Default.ScrapePlatformBoxBack)
					{
						GetArtSub(list, ArtType.BoxBack, 0);
					}
				});
			}

			catch { }

			finally
			{
				TaskInProgress = false;
			}
		}

		/// <summary>
		/// Sub function to download art based on options figured elsewhere
		/// </summary>
		/// <param name="db"></param>
		/// <param name="artType"></param>
		/// <returns></returns>
		int GetArtSub(List<IDBobject> list, ArtType artType, LocalDB localDB)
		{
			int misCount;
			int tryCount = 0;
			do
			{
				misCount = 0;
				foreach (IDBobject idbObject in list)
				{
					if (tokenSource.Token.IsCancellationRequested)
					{
						Reporter.Report("Downloading art cancelled.");
						tryCount = 6;
						break;
					}
					misCount += idbObject.ScrapeArt(artType, localDB);
				}
			} while (misCount < 0 && ++tryCount < 5);
			return misCount;
		}

		public Command GetAllDataCommand { get; set; }

		async void GetAllData()
		{
			// TODO: make this good for any idbobject and add get selected data command
			Stopwatch Watch = Stopwatch.StartNew();
			Stopwatch Watch1 = Stopwatch.StartNew();

			int j = 0;

			await Task.Run(() =>
			{
				Reporter.Report("Opening local cache...");

				R.Data.GBReleases.Load();
				R.Data.GDBReleases.Load();
				R.Data.OVGReleases.Load();
				R.Data.LBReleases.Include(x => x.LBGame).Load();
				Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
				Watch.Restart();

				int count = R.Data.Releases.Local.Count();
				foreach (Release release in R.Data.Releases.Local)
				{
					if (j++ % (count / 10) == 0)
					{
						Reporter.Report("Copying " + j + @" / " + count + " " + Watch.Elapsed.ToString(@"m\:ss") + " elapsed.");
						Watch.Restart();
					}
					release.CopyData();
				}
			});

			Datomatic datomatic = new Datomatic();
			datomatic.ReportUpdates(true);
			R.Data.Save(false);
		}

		bool GetAllDataCanExecute()
		{
			return MainBigSelection != null && (MainBigSelection == ReleaseCollection || MainBigSelection == GameCollection);
		}


		public Command AddCollectionCommand { get; set; }

		void AddCollection()
		{
			Collection collection = new Collection
			{
				Title = "New friggin collection",
				Type = "Game"
			};
			CollectionList.Add(collection);
		}

		bool AddCollectionCanExecute()
		{
			return MainBigSelection == CollectionList;
		}


		public Command RemoveFromCollectionCommand { get; set; }

		void RemoveFromCollection()
		{
			int N = SelectedDBs.Count;
			for (int i = N - 1; i >= 0; i--)
			{
				IDBobject idbObject = SelectedDBs[i] as IDBobject;
				(MainBigSelection as Collection).Remove(idbObject);
			}
		}

		bool RemoveFromCollectionCanExecute()
		{
			return MainBigSelection is Collection && SelectedDBs?.Count > 0;
		}


		public Command RemoveCollectionCommand { get; set; }

		void RemoveCollection()
		{
			CollectionList.Remove(MainBigSelection as Collection);
		}

		bool RemoveCollectionCanExecute()
		{
			return MainBigSelection is Collection;
		}


		public Command CancelTaskCommand { get; set; }

		void CancelTask()
		{
			tokenSource.Cancel();
		}

		bool CancelTaskCanExecute()
		{
			return TaskInProgress;
		}


		public Command AuditRomsCommand { get; set; }

		void AuditRoms()
		{
			new AuditWindow();
		}

		void InitializeCommands()
		{
			AddCollectionCommand = new Command(AddCollection, AddCollectionCanExecute, "Add collection", "Add a new custom collection to the list.");
			RemoveCollectionCommand = new Command(RemoveCollection, "Delete", "Remove this collection permanently.");

			DatabaseWindowCommand = new Command(DatabaseWindow, "Database Window", "Open the database window to manage databases.");
			ReporterWindowCommand = new Command(ReporterWindow, "Reporter Window", "Open the reporter window to view logs.");

			StatisticsWindowCommand = new Command(StatisticsWindow, "Statistics Window", "View statistics.");

			ArtWindowCommand = new Command(ArtWindow, ArtWindowCanExecute, "Art Window", "Open the window to select or improve available artwork.");

			OptionsWindowCommand = new Command(OptionsWindow, "Options", "Choose options for the main view.");

			SaveDataBaseCommand = new Command(SaveDataBase, "Save database", "Save all changes from the current session to the database.");
			AboutCommand = new Command(About, "About", "Open the about box.");
			HelpCommand = new Command(Help, "Help", "Navigate to the help website.");

			PlayCommand = new Command(Play, PlayCanExecute, "Play this", "Launch the selected game or release.");
			ClearFiltersCommand = new Command(ClearFilters, ClearFiltersCanExecute, "X", "Clear filters and display all objects.");

			MarkAsCrapCommand = new Command(MarkAsCrap, MarkAsCrapCanExecute, "Crap", "Mark selected items as crap.");
			MarkNotCrapCommand = new Command(MarkNotCrap, MarkNotCrapCanExecute, "Not crap", "Mark selected items as not crap.");

			MarkPreferredCommand = new Command(MarkPreferred, MarkPreferredCanExecute, "Preferred", "Mark selected item as preferred. Only one item can be marked at a time");
			MarkNotPreferredCommand = new Command(MarkNotPreferred, MarkNotPreferredCanExecute, "Not preferred", "Mark selected item as not preferred. Only one item can be marked at a time");

			MarkAsGameCommand = new Command(MarkAsGame, MarkAsGameCanExecute, "Game", "Mark selected items as games, i.e. not calculators or test cartridges");
			MarkNotGameCommand = new Command(MarkNotGame, MarkNotGameCanExecute, "Not a game", "Mark selected items as not games, i.e. calculators or test cartridges.");

			MarkAsBeatenCommand = new Command(MarkAsBeaten, MarkAsBeatenCanExecute, "Beaten", "Mark selected item as beaten.");
			MarkNotBeatenCommand = new Command(MarkNotBeaten, MarkNotBeatenCanExecute, "Not beaten", "Mark selected item as not beaten");

			MarkAsAdultCommand = new Command(MarkAsAdult, MarkAsAdultCanExecute, "Adult", "Mark selected item as adult.");
			MarkNotAdultCommand = new Command(MarkNotAdult, MarkNotAdultCanExecute, "Not adult", "Mark selected item as not adult");

			MarkAsMessCommand = new Command(MarkAsMess, MarkAsMessCanExecute, "Mess", "Mark selected item as an MESS machine.");
			MarkNotMessCommand = new Command(MarkNotMess, MarkNotMessCanExecute, "Not Mess", "Mark selected item as not an MESS machine.");

			MarkSelectedGameReleasePreferredCommand = new Command(MarkSelectedGameReleasePreferred, MarkSelectedGameReleasePreferredCanExecute, "Mark preferred", "Mark selected release as preferred. Only one item can be marked at a time. The preferred release will be launched by default");

			MarkSelectedPlatformEmulatorPreferredCommand = new Command(MarkSelectedPlatformEmulatorPreferred, MarkSelectedPlatformEmulatorPreferredCanExecute, "Mark preferred", "Mark selected emulator as preferred. Only one item can be marked at a time. The preferred emulator will be used to play games on this platform by default");

			MarkSelectedEmulatorPlatformPreferredCommand = new Command(MarkSelectedEmulatorPlatformPreferred, MarkSelectedEmulatorPlatformPreferredCanExecute, "Mark preferred", "Mark this  emulator as preferred for the selected platform. An emulator can be preferred for multiple platforms. If this emulator is preferred, it will be be used to play games on this platform by default");

			GetAllArtCommand = new Command(GetAllArt, GetAllArtCanExecute, "Get all art", "Download art for all displayed items.");
			GetSelectedArtCommand = new Command(GetSelectedArt, GetSelectedArtCanExecute, "Get art for selected items", "Downloads art for items currently selected.");

			GetAllDataCommand = new Command(GetAllData, GetAllDataCanExecute, "Get all data", "Downloads metadata for all displayed items.");

			RemoveFromCollectionCommand = new Command(RemoveFromCollection, RemoveFromCollectionCanExecute, "Remove from collection", "Removes the current item from the collection permanently.");

			CancelTaskCommand = new Command(CancelTask, CancelTaskCanExecute, "Cancel", "Cancel the current task");

			AuditRomsCommand = new Command(AuditRoms, "Audit Roms", "Verify ROM files are as expected.");
		}
	}
}
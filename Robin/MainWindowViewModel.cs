using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Robin
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		public RobinDataEntities Rdata { get; set; }

		AutoFilterCollection<Game> gameCollection;
		public AutoFilterCollection<Game> GameCollection
		{
			get { return gameCollection; }
			set
			{
				if (value != gameCollection)
				{
					gameCollection = value;
					OnPropertyChanged("GameCollection");
				}
			}
		}

		public AutoFilterCollection<Release> releaseCollection;
		public AutoFilterCollection<Release> ReleaseCollection
		{
			get { return releaseCollection; }
			set
			{
				if (value != releaseCollection)
				{
					releaseCollection = value;
					OnPropertyChanged("ReleaseCollection");
				}
			}
		}

		AutoFilterCollection<Platform> platformCollection;
		public AutoFilterCollection<Platform> PlatformCollection
		{
			get { return platformCollection; }
			set
			{
				if (value != platformCollection)
				{
					platformCollection = value;
					OnPropertyChanged("PlatformCollection");
				}
			}
		}

		AutoFilterCollection<Emulator> emulatorCollection;
		public AutoFilterCollection<Emulator> EmulatorCollection
		{
			get { return emulatorCollection; }
			set
			{
				if (value != emulatorCollection)
				{
					emulatorCollection = value;
					OnPropertyChanged("EmulatorCollection");
				}
			}
		}

		ObservableCollection<Collection> collectionList;
		public ObservableCollection<Collection> CollectionList
		{
			get { return collectionList; }
			set
			{
				if (value != collectionList)
				{
					collectionList = value;
					OnPropertyChanged("CollectionList");
				}
			}
		}

		IList selectedDBs { get; set; }
		public IList SelectedDBs
		{
			get { return selectedDBs; }
			set
			{
				if (value != selectedDBs)
				{
					selectedDBs = value;
					OnPropertyChanged("selectedDBs");
				}
			}
		}

		IDBobject selectedDB;
		public IDBobject SelectedDB
		{
			get { return selectedDB; }
			set
			{
				if (value != selectedDB)
				{
					selectedDB = value;
					OnPropertyChanged("selectedDB");
				}
			}
		}

		public Collection SelectedCollection { get; set; }

		public MainWindowViewModel()
		{
			Rdata = new RobinDataEntities(this);


			foreach (Game game in Rdata.Games)
			{
				game.Releases = game.Releases;
			}

			platformCollection = new AutoFilterCollection<Platform>(Rdata.Platforms.Local.ToList());
			gameCollection = new AutoFilterCollection<Game>(Rdata.Games.Local.Where(x => x.IsGame).ToList());
			releaseCollection = new AutoFilterCollection<Release>(Rdata.Releases.Local.Where(x => x.IsGame).ToList());
			emulatorCollection = new AutoFilterCollection<Emulator>(Rdata.Emulators.Local.ToList());
			collectionList = Rdata.Collections.Local;
		}

		public async void GetSelectedPlatformArt()
		{
			await Task.Run(() =>
			{
				Stopwatch Watch = new Stopwatch();
				Watch.Start();

				Reporter.Report("Opening Games DB cache...");

				Rdata.GDBPlatforms.Load();
				Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");

				List<Platform> list = new List<Platform>();
				foreach (Platform platform in SelectedDBs)
				{
					list.Add(platform);
				}

				foreach (Platform platform in list)
				{
					Reporter.Report("Caching " + platform.Title + "...");
					Watch.Restart();
					GamesDB.CachePlatformData(platform.GDBPlatform);
					Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
					Watch.Restart();
					Reporter.Report("Getting " + platform.Title + " art...");

					platform.ScrapeArt();
				}
			});
			Save();
		}

		public async void GetAllPlatformArt()
		{
			await Task.Run(() =>
			{
				Stopwatch Watch = new Stopwatch();
				Watch.Start();

				Reporter.Report("Opening GiantBombCache...");

				Rdata.GDBPlatforms.Load();
				Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");

				foreach (Platform platform in Rdata.Platforms)
				{
					Reporter.Report("Caching " + platform.Title + "...");
					Watch.Restart();
					GamesDB.CachePlatformData(platform.GDBPlatform);
					Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");

					Reporter.Report("Getting " + platform.Title + " art...");
					platform.ScrapeArt();
				}
			});
			Save();
		}

		public async void GetAllReleaseData()
		{
			Stopwatch Watch1 = new Stopwatch();
			Watch1.Start();
			Stopwatch Watch2 = new Stopwatch();
			Watch2.Start();
			int j = 0;

			await Task.Run(() =>
			{
				Reporter.Report("Opening local cache...");

				Rdata.GBReleases.Load();
				Rdata.GDBReleases.Load();
				Rdata.OVGReleases.Load();
				Reporter.ReportInline(Watch1.Elapsed.ToString("ss") + " s");
				Watch1.Restart();

				int count = Rdata.Releases.Count();
				foreach (Release release in Rdata.Releases)
				{
					j++;
					if (j % (count / 10) == 0)
					{
						Reporter.Report("Copying " + j + @" / " + count + " " + Watch1.Elapsed.ToString(@"m\:ss") + " elapsed.");
						Watch1.Restart();
					}
					release.CopyData();
				}
			});
			Reporter.Report("Finished. Copied data to " + j + " releases." + Watch2.Elapsed.ToString(@"m\:ss"));
			Save();
		}

		public async void GetAllReleaseArt()
		{
			int misCount = 0;
			await Task.Run(() =>
			{

				Reporter.Report("Opening databases...");
				Rdata.GDBReleases.Load();
				Rdata.GBReleases.Load();
				Rdata.OVGReleases.Load();
				Rdata.LBGames.Include(x => x.LBImages).Load();

				Reporter.Report("Scraping art files...");

				Directory.CreateDirectory(FileLocation.Art.BoxFront);
				Directory.CreateDirectory(FileLocation.Art.BoxBack);
				Directory.CreateDirectory(FileLocation.Art.Screen);
				Directory.CreateDirectory(FileLocation.Art.Banner);
				Directory.CreateDirectory(FileLocation.Art.Logo);
				int tryCount = 0;
				do
				{
					foreach (Release release in Rdata.Releases)
					{
						misCount += release.ScrapeArt(0);
					}
				} while (misCount < 0 && ++tryCount < 5);

			});
		}

		public async void GetSelectedReleaseArt()
		{
			int scrapedFiles = 0;
			await Task.Run(() =>
			{
				Reporter.Report("Opening databases...");
				Rdata.GDBReleases.Load();
				Rdata.GBReleases.Load();
				Rdata.OVGReleases.Load();
				Rdata.LBGames.Include(x => x.LBImages).Load();

				Reporter.Report("Scraping art files...");

				Directory.CreateDirectory(FileLocation.Art.BoxFront);
				Directory.CreateDirectory(FileLocation.Art.BoxBack);
				Directory.CreateDirectory(FileLocation.Art.Screen);
				Directory.CreateDirectory(FileLocation.Art.Banner);
				Directory.CreateDirectory(FileLocation.Art.Logo);

			// Must cache selected items in case user changes during scrape
			List<Release> list = new List<Release>();
				foreach (Release release in SelectedDBs)
				{
					list.Add(release);
				}

				foreach (Release release in list)
				{
					scrapedFiles += release.ScrapeArt(0);
				}
			});
			Reporter.Report("Added " + scrapedFiles + " art files.");
		}

		public void Save()
		{
			int i = Rdata.Save();
			Reporter.Report(i + " objects written to database.");
		}

		public void MarkCrap(bool value)
		{
			IList idbList = SelectedDBs;
			foreach (IDBobject idbObject in idbList)
			{
				idbObject.IsCrap = value;
			}
		}

		public void MarkBeaten(bool value)
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

		public void MarkGame(bool value)
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

		public void MarkPreferred(bool value)
		{
			if (SelectedDBs.Count == 1)
			{
				Release release = SelectedDB as Release;
				if (release != null)
				{
					release.Preferred = value;
				}
			}
			else
			{
				Reporter.Report("Can only mark one release as preferred.");
			}
		}

		public void AddCollection()
		{
			Collection collection = new Collection();
			collection.Title = "New friggin collection";
			collection.Type = "Game";
			CollectionList.Add(collection);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}

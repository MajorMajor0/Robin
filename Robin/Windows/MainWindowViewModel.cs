/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General internal License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General internal License for more details. 
 * 
 * You should have received a copy of the GNU General internal License
 * along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace Robin
{
	public partial class MainWindowViewModel
	{
		public ObservableCollection<object> MainBigList { get; }

		AutoFilterGames gameCollection;
		public AutoFilterGames GameCollection
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

		public AutoFilterReleases releaseCollection;
		public AutoFilterReleases ReleaseCollection
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

		AutoFilterPlatforms platformCollection;
		public AutoFilterPlatforms PlatformCollection
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

		AutoFilterEmulators emulatorCollection;
		public AutoFilterEmulators EmulatorCollection
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

		public CollectionList CollectionList { get; set; }

		IList selectedDBs;
		public IList SelectedDBs
		{
			get { return selectedDBs; }
			set
			{
				if (value != selectedDBs)
				{
					selectedDBs = value;
					OnPropertyChanged("SelectedDBs");
				}
			}
		}

		IDbObject selectedDB;
		public IDbObject SelectedDB
		{
			get { return selectedDB; }
			set
			{
				if (value != selectedDB)
				{
					selectedDB = value;
					OnPropertyChanged("SelectedDB");
				}
			}
		}

		CancellationTokenSource tokenSource;

		bool taskInProgress;
		public bool TaskInProgress
		{
			get
			{
				return taskInProgress;
			}
			set
			{
				if (value != taskInProgress)
				{
					taskInProgress = value;
					OnPropertyChanged("TaskInProgress");
				}
			}
		}

		public object MainBigSelection { get; set; }

		public Release SelectedGameRelease { get; set; }

		public Emulator SelectedPlatformEmulator { get; set; }

		public Platform SelectedEmulatorPlatform { get; set; }

		public bool DisplayNonGames => Properties.Settings.Default.DisplayNonGames;

		public bool DisplayAdult => Properties.Settings.Default.DisplayAdult;

		public bool DisplayCrap => Properties.Settings.Default.DisplayCrap;

		public bool DisplayNotIncluded => Properties.Settings.Default.DisplayNotIncluded;

		public MainWindowViewModel()
		{
			Stopwatch watch = Stopwatch.StartNew();
			R.Trash = true;
			Reporter.Report($"Trash: {watch.Elapsed.TotalSeconds:f1}");
			watch.Restart();

			MainBigList = new ObservableCollection<object>
			{
				(releaseCollection = new AutoFilterReleases(R.Data.Releases.Local, "Releases")),
				(gameCollection = new AutoFilterGames(R.Data.Games.Local, "Games")),
				(platformCollection = new AutoFilterPlatforms(R.Data.Platforms.Local, "Platforms")),
				(emulatorCollection = new AutoFilterEmulators(R.Data.Emulators.Local, "Emulators")),
				(CollectionList = new CollectionList("Collections"))
			};
			Reporter.Report($"Filters created: {watch.Elapsed.TotalSeconds:f1}");

			InitializeCommands();
		}

		public void SaveSettings()
		{
			foreach (AutoFilterCollection AFC in MainBigList.Where(x => x is AutoFilterCollection))
			{
				AFC.SaveSettings();
			}

			RecentFileList.Save();
			Properties.Settings.Default.Save();
		}

		void OptionsWindowClosed(object sender, EventArgs e)
		{
			releaseCollection = new AutoFilterReleases(R.Data.Releases.ToList(), "Releases");
			MainBigList[0] = releaseCollection;
			gameCollection = new AutoFilterGames(R.Data.Games.ToList(), "Games");
			MainBigList[1] = gameCollection;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}

	public class CollectionList
	{
		public string Title { get; set; }

		public Collection popular => new(R.Data.Games.Local.Where(x => x.PlayCount > 0).OrderByDescending(x => x.PlayCount)) { Title = "Popular" };

		public IEnumerable<Collection> List => R.Data.Collections.Local.Concat(new[] { popular });

		public CollectionList(string title)
		{
			Title = title;
		}

		public void Add(Collection collection)
		{
			R.Data.Collections.Add(collection);
		}
		public void Remove(Collection collection)
		{
			R.Data.Collections.Remove(collection);
		}
	}
}

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
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Robin
{
	partial class MainWindowViewModel
	{
		public ObservableCollection<object> MainBigList { get; }

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

		public CollectionList CollectionList { get; set; }

		IList selectedDBs { get; set; }
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

		IDBobject selectedDB;
		public IDBobject SelectedDB
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

		public object MainBigSelection { get; set; }

		public Release SelectedGameRelease { get; set; }

		public Emulator SelectedPlatformEmulator { get; set; }

		public Platform SelectedEmulatorPlatform { get; set; }


		public MainWindowViewModel()
		{
			R.Data = new RobinDataEntities(this);
			ReleaseCollection = new AutoFilterCollection<Release>(R.Data.Releases.Local.Where(x => x.IsGame).ToList(), "Releases");
			GameCollection = new AutoFilterCollection<Game>(R.Data.Games.Local.Where(x => x.IsGame).ToList(), "Games");
			PlatformCollection = new AutoFilterCollection<Platform>(R.Data.Platforms.Local.ToList(), "Platforms");
			EmulatorCollection = new AutoFilterCollection<Emulator>(R.Data.Emulators.Local.ToList(), "Emulators");

			CollectionList = new CollectionList("Collections");

			MainBigList = new ObservableCollection<object>();
			MainBigList.Add(ReleaseCollection);
			MainBigList.Add(GameCollection);
			MainBigList.Add(PlatformCollection);
			MainBigList.Add(EmulatorCollection);
			MainBigList.Add(CollectionList);

			InitializeCommands();
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

		public ObservableCollection<Collection> List => R.Data.Collections.Local;

		public CollectionList(string title)
		{
			Title = title;
		}

		public void Add(Collection collection)
		{
			List.Add(collection);
		}
		public void Remove(Collection collection)
		{
			List.Remove(collection);
		}
	}
}

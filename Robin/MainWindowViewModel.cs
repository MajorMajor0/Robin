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

		//private string SelectedDBNote()
		//{
		//	string plusText;
		//	string firstReason;
		//	string secondReason;
		//	string and = "and";

		//	if (SelectedDB.Included)
		//	{
		//		return selectedDB.Title + " is ready to play.";
		//	}

		//	switch (SelectedDB.GetType().BaseType.Name)
		//	{

		//		case "Release":
		//			if ((selectedDB as Release).Platform.HasEmulator)
		//				firstReason = "None of it's platform emulators have been installed ";
		//			break;
		//		case "Game":

		//			break;
		//		case "Platform":

		//			break;
		//		case "Emulator":

		//			break;
		//	}

		//	return SelectedDB.Title + " is not playable beacause ";

		//}


		public MainWindowViewModel()
		{
			R.Data = new RobinDataEntities(this);

			MainBigList = new ObservableCollection<object>();
			MainBigList.Add(ReleaseCollection = new AutoFilterReleases(R.Data.Releases.Local.Where(x => x.IsGame).ToList(), "Releases"));
			MainBigList.Add(GameCollection = new AutoFilterGames(R.Data.Games.Local.Where(x => x.IsGame).ToList(), "Games"));
			MainBigList.Add(PlatformCollection = new AutoFilterPlatforms(R.Data.Platforms.Local.ToList(), "Platforms"));
			MainBigList.Add(EmulatorCollection = new AutoFilterEmulators(R.Data.Emulators.Local.ToList(), "Emulators"));
			MainBigList.Add(CollectionList = new CollectionList("Collections"));

			InitializeCommands();
		}

		public void SaveSettings()
		{
			foreach (AutoFilterCollection AFC in MainBigList.Where(x => x is AutoFilterCollection))
			{
				AFC.SaveSettings();
			}
			Properties.Settings.Default.Save();
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

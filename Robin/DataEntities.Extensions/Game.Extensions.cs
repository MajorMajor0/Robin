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
 * along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Robin
{
	public partial class Game : IDBobject
	{
		//Stopwatch Watch = new Stopwatch();

		 string _title;
		public string Title
		{
			get
			{
				if (_title == null)
				{
					_title = Releases[0].Title;
				}
				return _title;
			}
		}


		public string Year => Releases[0].Year;

		List<string> genreList;
		public List<string> GenreList
		{
			get
			{
				if (genreList == null)
				{
					genreList = (Genre ?? "Unknown").Split(',').Select(x => x.Trim()).ToList();
				}
				return genreList;
			}
		}

		public Platform Platform => Releases[0].Platform;

		public string PlatformTitle => Platform.Title;

		public long Platform_ID => Releases[0].Platform_ID;

		string regions;
		public string Regions
		{
			get
			{
				if (regions == null)
				{
					regions = string.Join(", ", Releases.Select(x => x.Region.Title).Distinct());
				}
				return regions;
			}
		}

		List<string> regionsList;
		public List<string> RegionsList
		{
			get
			{
				if (regionsList == null)
				{
					regionsList = Releases.Select(x => x.Region.Title).Distinct().ToList();
				}
				return regionsList;
			}
		}

		public DateTime? Date => Releases[0].Date;


		public long PlayCount => Releases.Sum(x => x.PlayCount);

		public bool Included => Releases.Any(x => x.Included);

		public bool HasEmulator => Platform.Emulators.Any(x => x.Included);

		public bool HasRelease => Releases.Any(x => x.Included);

		public string WhyCantIPlay
		{
			get
			{
				if (Included)
				{
					return Title + " is ready to play.";
				}
				string and = HasRelease || HasEmulator ? "" : " and ";
				string emulatorTrouble = HasEmulator ? "" : "no emulator appears to be installed for " + Platform.Title + ".";
				string releaseTrouble = HasRelease ? "" : "no rom files appear to be available";
				return Title + " can't launch because " + releaseTrouble + and + emulatorTrouble + ".";
			}
		}

		public bool HasArt => Releases.Any(x => x.HasArt);


		public int BorderThickness { get; set; } = 1;

		public string MainDisplay
		{
			get
			{
#if DEBUG
				Stopwatch Watch = Stopwatch.StartNew();
#endif
				if (Platform_ID == CONSTANTS.ARCADE_PLATFORM_ID)
				{
					if (File.Exists(LogoPath)) { BorderThickness = 0; OnPropertyChanged("BorderThickness"); return LogoPath; }
					if (File.Exists(MarqueePath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return MarqueePath; }
					if (File.Exists(BoxFrontThumbPath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return BoxFrontThumbPath; }
				}

				else
				{
					if (File.Exists(BoxFrontThumbPath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return BoxFrontThumbPath; }
					if (File.Exists(LogoPath)) { BorderThickness = 0; OnPropertyChanged("BorderThickness"); return LogoPath; }
					if (File.Exists(MarqueePath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return MarqueePath; }
				}
#if DEBUG
				Debug.WriteLine("MainDisplay: " + Title + " " + Watch.ElapsedMilliseconds);
#endif
				BorderThickness = 0;
				OnPropertyChanged("BorderThickness");
				return Releases[0].Platform.ControllerPath;
			}
		}

		 string _boxFrontPath;
		public string BoxFrontPath
		{
			get
			{
				if (_boxFrontPath == null)
				{
					foreach (Release release in Releases)
					{
						if (File.Exists(release.BoxFrontPath))
						{
							_boxFrontPath = release.BoxFrontPath;
							break;
						}
					}
				}

				return _boxFrontPath;
			}
		}

		 string _boxFrontThumbPath;
		public string BoxFrontThumbPath
		{
			get
			{
				if (_boxFrontThumbPath == null)
				{
					foreach (Release release in Releases)
					{
						if (File.Exists(release.BoxFrontThumbPath))
						{
							_boxFrontThumbPath = release.BoxFrontThumbPath;
							break;
						}
					}
				}
				return _boxFrontThumbPath;
			}
		}

		public string BoxBackPath
		{
			// TODO this should probably go through all realeases looking for a file
			get { return Releases[0].BoxBackPath; }
		}

		public string BannerPath
		{
			// TODO this should probably go through all realeases looking for a file
			get { return Releases[0].BannerPath; }
		}

		public string ScreenPath
		{
			// TODO this should probably go through all realeases looking for a file
			get { return Releases[0].ScreenPath; }
		}

		public string LogoPath
		{
			// TODO this should probably go through all realeases looking for a file
			get { return Releases[0].LogoPath; }
		}

		public string MarqueePath
		{
			// TODO this should probably go through all realeases looking for a file
			get { return Releases[0].MarqueePath; }
		}

		 Release _preferredRelease;
		public Release PreferredRelease
		{
			get
			{
				int i = 0;
				while (_preferredRelease == null && i < 6)
				{
					_preferredRelease = Releases.FirstOrDefault(x => x.Region.Priority == i);
					i++;
				}
				if (_preferredRelease == null)
				{
					_preferredRelease = Releases[0];
				}
				return _preferredRelease;
			}
		}

		public bool Preferred
		{
			set
			{
				if (value == true)
				{
					Rating = 5;
				}
			}

			get
			{
				return Rating == 5;
			}
		}

		public void Play()
		{
			Play(PreferredRelease);
		}

		public void Play(Release release)
		{
			if (release == null)
			{
				release = PreferredRelease;
			}

			release.Play(null);
		}

		public int ScrapeArt(LocalDB localDB)
		{
			int returner = 0;
			foreach (Release release in Releases)
			{
				returner += release.ScrapeArt(localDB);
			}
			return returner;
		}

		public void OnPropertyChanged(string prop, string pubchoose)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}




	}
}


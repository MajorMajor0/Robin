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
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

namespace Robin;

public partial class Game
{
	[NotMapped]
	public string Title => Releases[0].Title;

	[NotMapped]
	public string Year => Releases[0].Year;

	[NotMapped]
	List<string> genreList;

	[NotMapped]
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

	[NotMapped]
	public Platform Platform => Releases[0].Platform;

	[NotMapped]
	public string PlatformTitle => Platform.Title;

	[NotMapped]
	public long Platform_ID => Releases[0].Platform_ID;

	string regions;
	[NotMapped]
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

	[NotMapped]
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

	[NotMapped]
	public DateTime? Date => Releases[0].Date;

	[NotMapped]
	public long PlayCount => Releases.Sum(x => x.PlayCount);

	[NotMapped]
	public bool Included => Releases.Any(x => x.Included);

	[NotMapped]
	public bool HasEmulator => Platform.Emulators.Any(x => x.Included);

	[NotMapped]
	public bool HasRelease => Releases.Any(x => x.Included);

	[NotMapped]
	public bool MatchedToSomething => Releases.Any(x => x.MatchedToSomething);

	[NotMapped]
	public bool HasArt => Catalog.Art.Contains(BoxFrontThumbPath)
		|| Catalog.Art.Contains(LogoPath)
		|| Catalog.Art.Contains(MarqueePath);

	[NotMapped]
	public string WhyCantIPlay
	{
		get
		{
			if (Included)
			{
				return Title + " is ready to play.";
			}
			string and = HasRelease || HasEmulator ? "" : " and ";
			string emulatorTrouble = HasEmulator ? "" : "no emulator appears to be installed for " + Platform.Title;
			string releaseTrouble = HasRelease ? "" : "no rom files appear to be available";
			return Title + " can't launch because " + releaseTrouble + and + emulatorTrouble + ".";
		}
	}

	int borderThickness = 1;

	[NotMapped]
	public int BorderThickness
	{
		get
		{
			return borderThickness;
		}

		set
		{
			if (borderThickness != value)
			{
				borderThickness = value;
				OnPropertyChanged("BorderThickness");
			}
		}
	}

	[NotMapped]
	public string MainDisplay
	{
		get
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			switch (AppSettings.DisplayChoice)
			{
				case AppSettings.DisplayOption.Default:
					if (Platform_ID == CONSTANTS.Platform_ID.Arcade)
					{
						if (Catalog.Art.Contains(LogoPath))
						{
							BorderThickness = 0;
							return LogoPath;
						}
						if (Catalog.Art.Contains(MarqueePath))
						{
							BorderThickness = 1;
							return MarqueePath;
						}
						if (Catalog.Art.Contains(BoxFrontThumbPath))
						{
							BorderThickness = 1;
							return BoxFrontThumbPath;
						}
					}

					else
					{
						if (Catalog.Art.Contains(BoxFrontThumbPath))
						{
							BorderThickness = 1;
							return BoxFrontThumbPath;
						}
						if (Catalog.Art.Contains(LogoPath))
						{
							BorderThickness = 0;
							return LogoPath;
						}
						if (Catalog.Art.Contains(MarqueePath))
						{
							BorderThickness = 1;
							return MarqueePath;
						}
					}
					break;
				case AppSettings.DisplayOption.BoxFront:
					if (Catalog.Art.Contains(BoxFrontThumbPath))
					{
						BorderThickness = 1;
						return BoxFrontThumbPath;
					}
					break;
				case AppSettings.DisplayOption.BoxBack:
					if (Catalog.Art.Contains(BoxBackPath))
					{
						BorderThickness = 1;
						return BoxBackPath;
					}
					break;
				case AppSettings.DisplayOption.Screen:
					if (Catalog.Art.Contains(ScreenPath))
					{
						BorderThickness = 1;
						return ScreenPath;
					}
					break;
				case AppSettings.DisplayOption.Banner:
					if (Catalog.Art.Contains(BannerPath))
					{
						BorderThickness = 0;
						return BannerPath;
					}
					break;
				default:
					Debug.Assert(false, "No valid display option");
					break;
			}
#if DEBUG
			//Debug.WriteLine("MainDisplay: " + Title + " " + Watch.ElapsedMilliseconds);
#endif
			BorderThickness = 0;

			return Platform.ControllerPath;
		}
	}

	string boxFrontPath;
	[NotMapped]
	public string BoxFrontPath
	{
		get
		{
			if (boxFrontPath == null)
			{
				foreach (Release release in Releases)
				{
					if (Catalog.Art.Contains(release.BoxFrontPath))
					{
						boxFrontPath = release.BoxFrontPath;
						break;
					}
				}
			}

			return boxFrontPath;
		}
	}

	string boxFrontThumbPath;
	[NotMapped]
	public string BoxFrontThumbPath
	{
		get
		{
			if (boxFrontThumbPath == null)
			{
				foreach (Release release in Releases)
				{
					if (Catalog.Art.Contains(release.BoxFrontThumbPath))
					{
						boxFrontThumbPath = release.BoxFrontThumbPath;
						break;
					}
				}
			}
			return boxFrontThumbPath;
		}
	}

	[NotMapped]
	public string BoxBackPath
	{
		// TODO this should probably go through all realeases looking for a file
		get { return Releases[0].BoxBackPath; }
	}

	[NotMapped]
	public string BannerPath
	{
		// TODO this should probably go through all realeases looking for a file
		get { return Releases[0].BannerPath; }
	}

	[NotMapped]
	public string ScreenPath
	{
		// TODO this should probably go through all realeases looking for a file
		get { return Releases[0].ScreenPath; }
	}

	[NotMapped]
	public string LogoPath
	{
		// TODO this should probably go through all realeases looking for a file
		get { return Releases[0].LogoPath; }
	}

	[NotMapped]
	public string MarqueePath
	{
		// TODO this should probably go through all realeases looking for a file
		get { return Releases[0].MarqueePath; }
	}

	private Release preferredRelease;

	[NotMapped]
	public Release PreferredRelease
	{
		get
		{
			int i = 0;
			while (preferredRelease == null && i < 6)
			{
				preferredRelease = Releases.FirstOrDefault(x => x.Region.Priority == i);
				i++;
			}
			if (preferredRelease == null)
			{
				preferredRelease = Releases[0];
			}
			return preferredRelease;
		}
	}

	[NotMapped]
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

	public int ScrapeArt(ArtType artType, LocalDB localDB)
	{
		int returner = 0;
		foreach (Release release in Releases)
		{
			returner += release.ScrapeArt(artType, localDB);
		}
		return returner;
	}

}



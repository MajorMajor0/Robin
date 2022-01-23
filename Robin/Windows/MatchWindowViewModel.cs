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
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Robin;
public class MatchWindowViewModel : INotifyPropertyChanged
{
	Release release;
	public Release Release
	{
		get
		{
			return release;
		}

		set
		{
			release = value;
			OnPropertyChanged("Release");
			OnPropertyChanged("Index");
		}
	}

	private Platform platform => Release.Platform;

	public List<Release> UnmatchedReleases => platform.Releases.Where(x => !x.HasArt && x.IsGame).ToList();

	int index = 0;
	public int Index
	{
		get { return index; }

		set
		{
			if (value != index)
			{
				index = value;
				OnPropertyChanged("Index");
			}
		}
	}


	string searchTerm;
	public string SearchTerm
	{
		get
		{
			return searchTerm;
		}

		set
		{
			if (searchTerm != value)
			{
				searchTerm = value;

				if (searchTerm.Length != 1)
				{
					OnPropertyChanged("IDBReleases");
				}
				OnPropertyChanged("SearchTerm");
			}
		}
	}

	public IDbRelease SelectedIDBRelease { get; set; }

	public IEnumerable<IDbRelease> IDBReleases => GBReleases.Concat(GDBReleases).Concat(LBReleases);

	public IEnumerable<IDbRelease> GBReleases
	{
		get
		{
			if (platform.GBPlatform != null && !string.IsNullOrEmpty(searchTerm))
			{
				return platform.GBPlatform.GBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
			}
			return Enumerable.Empty<IDbRelease>();
		}
	}

	public IEnumerable<IDbRelease> GDBReleases
	{
		get
		{
			if (platform.GDBPlatform != null && !string.IsNullOrEmpty(searchTerm))
			{
				return platform.GDBPlatform.GDBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
			}
			return Enumerable.Empty<IDbRelease>();
		}
	}

	public IEnumerable<IDbRelease> LBReleases
	{
		get
		{
			if (platform.LBPlatform != null && !string.IsNullOrEmpty(searchTerm))
			{
				return platform.LBPlatform.LBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
			}
			return Enumerable.Empty<IDbRelease>();
		}
	}

	//public IEnumerable<IDBRelease> OVGReleases
	//{
	//	get
	//	{
	//		if (platform.OVGPlatform != null && !string.IsNullOrEmpty(searchTerm))
	//		{
	//			return platform.OVGPlatform.OVGReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
	//		}
	//		return Enumerable.Empty<IDBRelease>();
	//	}
	//}

	public long? ID_GB => Release.ID_GB;

	public long? ID_GDB => Release.ID_GDB;

	public long? ID_LB => Release.ID_LB;

	public long? ID_OVG => Release.ID_OVG;

	public MatchWindowViewModel(Release release)
	{
		this.Release = release;
		_ = new Launchbox();
		_ = new GiantBomb();
		_ = new GamesDB();
		MatchCommand = new Command(Match, MatchCanExecute, "Match", "Match the selected release to " + release.Title + ".");
		ShowBoxCommand = new Command(ShowBox, "Show box art", "Show box front art for this selection.");
		NextCommand = new Command(Next, NextCanExecute, @">", "Move to the next unmatched release in this platform.");
		PreviousCommand = new Command(Previous, PreviousCanExecute, @"<", "Move to the previous unmatched release in this platform.");
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}


	public Command MatchCommand { get; set; }

	void Match()
	{
		LocalDB db = SelectedIDBRelease.LocalDB;
		switch (db)
		{
			case LocalDB.GamesDB:
				Release.ID_GDB = SelectedIDBRelease?.ID;
				break;
			case LocalDB.GiantBomb:
				Release.ID_GB = SelectedIDBRelease?.ID;
				break;
			case LocalDB.OpenVGDB:
				Release.ID_OVG = SelectedIDBRelease?.ID;
				break;
			case LocalDB.LaunchBox:
				Release.ID_LB = SelectedIDBRelease?.ID;
				break;
			default:
				break;
		}
		Reporter.Report(Release.Title + " matched to " + db.Description() + " release " + SelectedIDBRelease?.ID + ", " + SelectedIDBRelease?.Title + ".");
		OnPropertyChanged("IDBReleases");
		OnPropertyChanged("UnmatchedReleases");
	}

	bool MatchCanExecute()
	{
		return SelectedIDBRelease != null;
	}

	public Command ShowBoxCommand { get; set; }

	async void ShowBox()
	{
		await Task.Run(() =>
		{
			SelectedIDBRelease.ScrapeBoxFront();
		});
	}

	public Command NextCommand { get; set; }

	bool NextCanExecute()
	{
		return UnmatchedReleases.Any();
	}

	void Next()
	{
		if (Index < UnmatchedReleases.Count - 1)
		{
			Index++;
		}

		else
		{
			Index = 0;
		}

		Release = UnmatchedReleases[Index];
		SearchTerm = string.Empty;
	}

	void Previous()
	{
		if (Index > 0)
		{
			Index--;
		}

		else
		{
			Index = UnmatchedReleases.Count - 1;
		}

		Release = UnmatchedReleases[Index];
		SearchTerm = string.Empty;
	}

	bool PreviousCanExecute()
	{
		return UnmatchedReleases.Any();
	}

	public Command PreviousCommand { get; set; }
}

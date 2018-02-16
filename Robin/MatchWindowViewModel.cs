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

using System.Diagnostics;
using System;
using System.Collections;
using Robin.Properties;
using System.Collections.ObjectModel;

namespace Robin
{
	public class MatchWindowViewModel : INotifyPropertyChanged
	{
		Release release;

		Platform platform => release.Platform;

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
					Search();
				}
			}
		}

		public string Title => release.Title;

		public string SHA1 => release.Rom.SHA1;

		public string Region => release.RegionTitle;

		public bool GBFocused { get; set; }
		public GBRelease SelectedGBRelease { get; set; }
		public IEnumerable<GBRelease> GBReleases
		{
			get
			{
				if (platform.GBPlatform != null && searchTerm != null)
				{
					return platform.GBPlatform.GBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public bool GDBFocused { get; set; }
		public GDBRelease SelectedGDBRelease { get; set; }
		public IEnumerable<GDBRelease> GDBReleases
		{
			get
			{
				if (platform.GDBPlatform != null && searchTerm != null)
				{
					return platform.GDBPlatform.GDBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public bool LBFocused { get; set; }
		public LBRelease SelectedLBRelease { get; set; }
		public IEnumerable<LBRelease> LBReleases
		{
			get
			{
				if (platform.LBPlatform != null && searchTerm != null)
				{
					return platform.LBPlatform.LBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public bool OVGFocused { get; set; }
		public OVGRelease SelectedOVGRelease { get; set; }
		public IEnumerable<OVGRelease> OVGReleases
		{
			get
			{
				if (platform.OVGPlatform != null && searchTerm != null)
				{
					return platform.OVGPlatform.OVGReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public long? ID_GB => release.ID_GB;

		public long? ID_GDB => release.ID_GDB;

		public long? ID_LB => release.ID_LB;

		public long? ID_OVG => release.ID_OVG;

		public MatchWindowViewModel(Release release)
		{
			this.release = release;
			MatchGBCommand = new Command(MatchGB, MatchGBCanExecute, "Match GB", "Match the selected GiantBomb release to " + release.Title + ".");
			MatchGDBCommand = new Command(MatchGDB, MatchGDBCanExecute, "Match GDB", "Match the selected Games DB release to " + release.Title + ".");
			MatchLBCommand = new Command(MatchLB, MatchLBCanExecute, "Match LB", "Match the selected Launchbox release to " + release.Title + ".");
			MatchOVGCommand = new Command(MatchOVG, MatchOVGCanExecute, "Match OVG", "Match the selected Open VDB release to " + release.Title + ".");
			ShowBoxCommand = new Command(ShowBox, "Show box art", "SHow box front art for this selection.");
		}

		public void Search()
		{
			if (searchTerm.Length > 1)
			{
				OnPropertyChanged("GBReleases");
				OnPropertyChanged("GDBReleases");
				OnPropertyChanged("LBReleases");
				OnPropertyChanged("OVGReleases");
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}


		public Command MatchGBCommand { get; set; }

		void MatchGB()
		{
			release.ID_GB = SelectedGBRelease?.ID;
			Reporter.Report(release.Title + " matched to GB release" + SelectedGBRelease?.ID + ", " + SelectedGBRelease?.Title + ".");
		}

		bool MatchGBCanExecute()
		{
			return SelectedGBRelease != null && GBFocused;
		}

		public Command MatchGDBCommand { get; set; }

		void MatchGDB()
		{
			release.ID_GDB = SelectedGDBRelease?.ID;
			Reporter.Report(release.Title + " matched to GDB release" + SelectedGDBRelease?.ID + ", " + SelectedGDBRelease?.Title + ".");
		}

		bool MatchGDBCanExecute()
		{
			return SelectedGDBRelease != null && GDBFocused;
		}

		public Command MatchLBCommand { get; set; }

		void MatchLB()
		{
			release.ID_LB = SelectedLBRelease?.ID;
			Reporter.Report(release.Title + " matched to LB release" + SelectedLBRelease?.ID + ", " + SelectedLBRelease?.Title + ".");
		}

		bool MatchLBCanExecute()
		{
			return SelectedLBRelease != null && LBFocused;
		}

		public Command MatchOVGCommand { get; set; }

		void MatchOVG()
		{
			release.ID_OVG = SelectedOVGRelease?.ID;
			Reporter.Report(release.Title + " matched to OVG release" + SelectedOVGRelease?.ID + ", " + SelectedOVGRelease?.Title + ".");
		}
		bool MatchOVGCanExecute()
		{
			return SelectedOVGRelease != null && OVGFocused;
		}

		public Command ShowBoxCommand { get; set; }

		async void ShowBox()
		{
			//await Task.Run(() => { release.ScrapeBoxFront(); });
			await Task.Run(() => { });
		}

	}
}

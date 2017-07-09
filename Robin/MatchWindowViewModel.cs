using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Robin
{
	class MatchWindowViewModel : INotifyPropertyChanged
	{
		Release release;

		Platform platform;

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

		public string Title { get { return release.Title; } }

		public string SHA1 { get { return release.Rom.SHA1; } }

		public string Region { get { return release.RegionTitle; } }

		public IEnumerable<GBRelease> GBReleases
		{
			get
			{
				if (platform.GBPlatform != null && searchTerm != null)
				{
					return platform.GBPlatform.GBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm, RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public IEnumerable<GDBRelease> GDBReleases
		{
			get
			{
				if (platform.GDBPlatform != null && searchTerm != null)
				{
					return platform.GDBPlatform.GDBReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm, RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public IEnumerable<LBGame> LBReleases
		{
			get
			{
				if (platform.LBPlatform != null && searchTerm != null)
				{
					return platform.LBPlatform.LBGames.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm, RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public IEnumerable<OVGRelease> OVGReleases
		{
			get
			{
				if (platform.OVGPlatform != null && searchTerm != null)
				{
					return platform.OVGPlatform.OVGReleases.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm, RegexOptions.IgnoreCase));
				}
				return null;
			}
		}

		public long? ID_GB { get { return release.ID_GB; } }

		public long? ID_GDB { get { return release.ID_GDB; } }

		public long? ID_LB { get { return release.ID_LB; } }

		public long? ID_OVG { get { return release.ID_OVG; } }


		public MatchWindowViewModel(Release _release)
		{
			release = _release;
			platform = _release.Platform;
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

		public void Match(IDBRelease idbRelease)
		{
			switch (idbRelease.GetType().BaseType.Name)
			{
				case "GBRelease":
					release.ID_GB = idbRelease.ID;
					break;
				case "GDBRelease":
					release.ID_GDB = idbRelease.ID;
					break;
				case "LBGame":
					release.ID_LB = idbRelease.ID;
					break;
				case "OVGRelease":
					release.ID_OVG = idbRelease.ID;
					break;
				default:
					break;
			}

		}

		public async void ShowBox(IDBRelease idbRelease)
		{
			await Task.Run(() => { idbRelease.ScrapeBoxFront(); });
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}

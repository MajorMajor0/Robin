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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	partial class DatabaseWindowViewModel
	{

		public Command CompareCommand { get; set; }

		async void CompareAsync()
		{
			if (SelectedPlatforms != null && SelectedPlatforms.Count > 0)
			{
				List<IDBPlatform> list = new List<IDBPlatform>();
				foreach (IDBPlatform platform in SelectedPlatforms)
				{
					list.Add(platform);
				}

				if (SelectedIDB.DB == LocalDB.OpenVGDB)
				{
					foreach (IDBPlatform platform in list)
					{
						Reporter.Report("Comparing " + platform.Releases.Count + " Robin " + platform.Title + " games to OVG.");

						await Task.Run(() => { CompareToOVGDB(platform); });
					}
				}

				else
				{
					foreach (IDBPlatform idbPlatform in list)
					{
						Compares comparator = new Compares(SelectedIDB.DB, idbPlatform);
						comparator.Title += "-" + ComparisonResults.Count;

						Reporter.Report("Comparing " + idbPlatform.Releases.Count + " Robin " + idbPlatform.Title + " games to " + Enum.GetName(typeof(LocalDB), SelectedIDB.DB) + ".");

						await Task.Run(() =>
						{
							comparator.CompareToDB(Threshold, (SelectedIDB.HasRegions));
						});

						ComparisonResults.Add(comparator);
					}
					Reporter.Report("Finished");
				}
			}
		}

		bool CompareCanExecute()
		{
			return SelectedIDB != null && !(SelectedIDB is Datomatic) && SelectedPlatform != null;
		}

		void CompareToOVGDB(IDBPlatform idbPlatform)
		{
			// There are no results in OVGDB for arcade, so skip it
			if (idbPlatform.Title.Contains("Arcade"))
			{
				Reporter.Report("Skiping platform \"Arcade\"");
				return;
			}

			Platform RPlatform = R.Data.Platforms.FirstOrDefault(x => x.ID == idbPlatform.ID);

			OVGRelease ovgrelease;
			foreach (Release release in RPlatform.Releases)
			{
				ovgrelease = R.Data.OVGReleases.FirstOrDefault(x => x.SHA1 == release.Rom.SHA1 && x.Region_ID == release.Region_ID && (x.BoxFrontURL != null || x.BoxBackURL != null));
				if (ovgrelease != null)
				{
					release.ID_OVG = ovgrelease.ID;
				}
			}
		}


		public Command AcceptCommand { get; set; }

		public void Accept()
		{
			Compares Comparator = SelectedComparisonResult;
			int N_comp = Comparator.List.Count;
			int i_r;
			int i_db;

			for (int i_comp = N_comp - 1; i_comp >= 0; i_comp--)
			{
				i_r = Comparator.List[i_comp].RIndex;

				switch (Comparator.Database)
				{
					case LocalDB.GamesDB:
						if (Comparator.List[i_comp].AcceptMatch)
						{
							// Get the index of the game stored in the current compare
							i_db = Comparator.List[i_comp].DBIndex;

							// Assign the rom in the current compare to the game in the current compare
							Comparator.RReleases[i_r].ID_GDB = Comparator.DBreleases[i_db].ID;
							Comparator.List.RemoveAt(i_comp);
						}
						break;
					case LocalDB.GiantBomb:
						if (Comparator.List[i_comp].AcceptMatch)
						{
							// Get the index of the game stored in the current compare
							i_db = Comparator.List[i_comp].DBIndex;

							// Assign the rom in the current compare to the game in the current compare
							Comparator.RReleases[i_r].ID_GB = Comparator.DBreleases[i_db].ID;
							Comparator.List.RemoveAt(i_comp);
						}
						break;
					case LocalDB.LaunchBox:
						if (Comparator.List[i_comp].AcceptMatch)
						{
							// Get the index of the game stored in the current compare
							i_db = Comparator.List[i_comp].DBIndex;

							// Assign the rom in the current compare to the game in the current compare
							Comparator.RReleases[i_r].ID_LB = Comparator.DBreleases[i_db].ID;
							Comparator.List.RemoveAt(i_comp);
						}
						break;
				}
			}
		}

		bool AcceptCanExecute()
		{
			return SelectedComparisonResult != null;
		}


		public Command ArtWindowCommand { get; set; }

		void ArtWindow()
		{
			ArtWindow artWindow = new ArtWindow(SelectedRelease as Release);
		}

		bool ArtWindowCanExecute()
		{
			return SelectedIDB is Datomatic && SelectedRelease != null;
		}


		public Command MatchWindowCommand { get; set; }

		void MatchWindow()
		{
			MatchWindow matchWindow = new MatchWindow(SelectedRelease as Release);
		}

		bool MatchWindowCanExecute()
		{
			return SelectedIDB is Datomatic && SelectedRelease != null;
		}


		public Command CacheReleasesCommand { get; set; }

		async void CacheReleases()
		{
			// Cache platforms to cache in case selection changes during operation
			List<IDBPlatform> IDBPlatforms = new List<IDBPlatform>();

			foreach (IDBPlatform idbPlatform in SelectedPlatforms)
			{
				IDBPlatforms.Add(idbPlatform);
			}

			Reporter.Report("Caching " + IDBPlatforms.Count + " " + SelectedIDB.Title + " platforms.");

			await Task.Run(() =>
			{
				foreach (IDBPlatform idbPlatform in IDBPlatforms)
				{
					SelectedIDB.CachePlatformData(idbPlatform.RPlatform);
					SelectedIDB.CachePlatformReleases(idbPlatform.RPlatform);			
					idbPlatform.CacheDate = DateTime.Now;
				}

				//ReportUpdates() calls detect changes, so not necessary in save
			});

			SelectedIDB.ReportUpdates(true);
			if (SelectedIDB != IDBs[0])
			{
				IDBs[0].ReportUpdates(false);
			}

			//R.Data.Save(false);
		}

		bool CacheReleasesCanExecute()
		{
			return SelectedPlatform != null;
		}


		public Command WriteDBCommand { get; set; }

		void WriteDB()
		{
			R.Data.Save(true);
		}


		void IntializeCommands()
		{
			CompareCommand = new Command(CompareAsync, CompareCanExecute, "Compare", "Compare title of releases in selected platforms to Robin database.");
			AcceptCommand = new Command(Accept, AcceptCanExecute, "Accept Changes", "Push all matches to the database");
			ArtWindowCommand = new Command(ArtWindow, ArtWindowCanExecute, "Art Window", "Open a window to choose artwork");
			MatchWindowCommand = new Command(MatchWindow, MatchWindowCanExecute, "Match Window", "Open a window to match release to databases");
			CacheReleasesCommand = new Command(CacheReleases, CacheReleasesCanExecute, "Cache Releases", "Cache releases from Datomatic or MAME for selected platforms.");
			WriteDBCommand = new Command(WriteDB, "Write Database", "Write all database changes to disk");
		}
	}
}

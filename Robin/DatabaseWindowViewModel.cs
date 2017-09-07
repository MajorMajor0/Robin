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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Robin
{
    public class DatabaseWindowViewModel : INotifyPropertyChanged
    {
        public int Threshold { get; set; }

        //public bool ConsiderRegions { get; set; }

        //public bool SearchSubdirectories { get; set; }

        IDB selectedIDB;
        public IDB SelectedIDB
        {
            get { return selectedIDB; }
            set
            {
                if (value != selectedIDB)
                {
                    selectedIDB = value;
                    OnPropertyChanged("SelectedIDB");
                }
            }
        }

        public ObservableCollection<IDB> IDBs { get; set; }

        public IEnumerable<IDBPlatform> PlatformsList { get; set; }

        public IList SelectedPlatforms { get; set; }

        public IDBPlatform SelectedPlatform { get; set; }

        IEnumerable gridDBObjects;
        public IEnumerable GridDBObjects
        {
            get { return gridDBObjects; }
            set
            {
                if (value != gridDBObjects)
                {
                    gridDBObjects = value;
                    OnPropertyChanged("GridDBObjects");
                }
            }
        }

        public ObservableCollection<Compares> ComparisonResults { get; set; }
        public Compares SelectedComparisonResult { get; set; }

        public DatabaseWindowViewModel()
        {
            Threshold = 0;
            //ConsiderRegions = true;
            //SearchSubdirectories = false;

            IDBs = new ObservableCollection<IDB>();

            GetDBs();

            SelectedPlatforms = new ObservableCollection<IDBPlatform>();

            ComparisonResults = new ObservableCollection<Compares>();
        }

        public async void GetDBs()
        {
            var uiContext = SynchronizationContext.Current;

            await Task.Run(() =>
            {
                uiContext.Send(x => IDBs.Add(new Datomatic()), null);
                uiContext.Send(x => IDBs.Add(new GamesDB()), null);
                uiContext.Send(x => IDBs.Add(new GiantBomb()), null);
                uiContext.Send(x => IDBs.Add(new OpenVGDB()), null);
                uiContext.Send(x => IDBs.Add(new Launchbox()), null);
            });
        }

        public async void CachePlatform()
        {
            //LocalDB localDB = SelectedIDB.DB;

            // Cache platforms to cache in case selection changes during operation
            List<IDBPlatform> IDBPlatforms = new List<IDBPlatform>();

            foreach (IDBPlatform idbPlatform in SelectedPlatforms)
            {
                IDBPlatforms.Add(idbPlatform);
            }

            Reporter.Report("Caching " + IDBPlatforms.Count + " Launchbox platforms.");

            await Task.Run(() =>
            {
                foreach (IDBPlatform idbPlatform in IDBPlatforms)
                {
                    SelectedIDB.CachePlatformReleases(idbPlatform.RPlatform);
                    SelectedIDB.CachePlatformData(idbPlatform.RPlatform);
                    idbPlatform.CacheDate = DateTime.Now;
                }

                // ReportUpdates() calls detect changes, so not necessary in save
                SelectedIDB.ReportUpdates(true);
                IDBs[0].ReportUpdates(false);
                R.Data.Save(false);
            });
        }

        public async void CompareToDBAsync()
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

        public void CompareToOVGDB(IDBPlatform idbPlatform)
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
                ovgrelease = R.Data.OVGReleases.FirstOrDefault(x => x.SHA1 == release.SHA1 && x.Region_ID == release.Region_ID && (x.BoxFrontURL != null || x.BoxBackURL != null));
                if (ovgrelease != null)
                {
                    release.ID_OVG = ovgrelease.ID;
                }
            }
        }

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

        public void CopyData()
        {
            Reporter.Report("Getting data...");
            foreach (Release release in R.Data.Releases)
            {
                release.CopyData();
            }
            Reporter.ReportInline("finished.");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}

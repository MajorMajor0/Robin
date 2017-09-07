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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Robin
{
    public class Compares : INotifyPropertyChanged
    {
        public ObservableCollection<Compare> List { get; set; }
        public List<Release> RReleases { get; set; }
        public List<IDBRelease> DBreleases { get; set; }
        public LocalDB Database { get; set; }

        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        public Compares()
        {
            List = new ObservableCollection<Compare>();
            Database = LocalDB.Unknown;
        }

        public Compares(LocalDB db, IDBPlatform idbPlatform)
        {
            Platform platform;
            List = new ObservableCollection<Compare>();
            Database = db;
            Title = idbPlatform.Title + " - " + Enum.GetName(typeof(LocalDB), db);
            DBreleases = new List<IDBRelease>();
            RReleases = new List<Release>();

            foreach (IDBRelease idbRelease in idbPlatform.Releases)
            {
                DBreleases.Add(idbRelease);
            }

            switch (db)
            {
                case LocalDB.GamesDB:
                    platform = R.Data.Platforms.FirstOrDefault(x => x.ID_GDB == idbPlatform.ID);
                    if (platform != null)
                    {
                        RReleases = new List<Release>(platform.Releases.Where(x => x.ID_GDB == null && x.IsGame));
                    }
                    break;

                case LocalDB.GiantBomb:
                    platform = R.Data.Platforms.FirstOrDefault(x => x.ID_GB == idbPlatform.ID);
                    if (platform != null)
                    {
                        RReleases = new List<Release>(platform.Releases.Where(x => x.ID_GB == null && x.IsGame));
                    }
                    break;

                case LocalDB.LaunchBox:
                    platform = R.Data.Platforms.FirstOrDefault(x => x.ID_LB == idbPlatform.ID);
                    if (platform != null)
                    {
                        RReleases = new List<Release>(platform.Releases.Where(x => x.ID_LB == null && x.IsGame));
                    }
                    break;

            }
        }

        public void CompareToDB(int threshold, bool ConsiderRegion)
        {
            List.Clear();

            // List for storing matches
            List<Compare> Matches = new List<Compare>();
            int distance;

            bool goAhead = !ConsiderRegion;
            Release release;
            IDBRelease dbRelease;

            int N_r = RReleases.Count;

            // Loop over all Releases
            for (int i_r = 0; i_r < N_r; i_r++)
            {
                if ((RReleases.Count / 10 != 0) && (i_r % (RReleases.Count / 10) == 0))
                {
                    Reporter.Report("Comparing " + i_r + @" / " + RReleases.Count);
                }

                release = RReleases[i_r];

                // Compare current Release to each relase in DBreleases 
                for (int i_db = 0; i_db < DBreleases.Count; i_db++)
                {
                    dbRelease = DBreleases[i_db];

                    if (ConsiderRegion)
                    {
                        goAhead = (release.Region.Title == dbRelease.RegionTitle);
                    }

                    if (goAhead)
                    {
                        // Get distance between Rom i and Game j
                        distance = LevenshteinDistance.Compute(release.Title.Wash(), dbRelease.Title.Wash());

                        // Add the current rom/game to a list of matches for consideration
                        Matches.Add(new Compare(distance, dbRelease.ID, i_db, dbRelease.Title, release.Title, i_r, release.Region.Title, dbRelease.RegionTitle));

                        // If the current match is perfect, store it and check the accept box. Stop looking through the regions.
                        if (Matches.Last().Distance == 0)
                        {
                            Matches.Last().AcceptMatch = true;
                            break;
                        }
                    }
                }

                if (Matches.Any())
                {
                    // Find the closest match, then add the threshold from the DataBaseWindow checkbox
                    int min = Matches.Min(em => em.Distance + threshold);

                    // Add all of the compares from the list of matches whose distances are closer than the chosen minimum
                    // These are the matches to be displayed for consideration
                    List.AddRange(new List<Compare>(Matches.Where(n => n.Distance == min).Select(n => n).ToList()));
                }
                Matches.Clear();

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Compare
    {
        public int Distance { get; set; }

        public long? DBID { get; set; }
        public string DBTitle { get; set; }
        public string DBRegion { get; set; }
        public int DBIndex { get; set; }

        public string RTitle { get; set; }
        public string RRegion { get; set; }
        public int RIndex { get; set; }

        public bool AcceptMatch { get; set; }

        public bool MatchedRegions
        {
            get
            {
                return RRegion == DBRegion;
            }
        }

        public Compare()
        {
            Distance = -1;
            DBID = 0;
            DBIndex = -1;
            DBTitle = "-";
            RTitle = "-";
            RIndex = -1;
            AcceptMatch = false;
        }

        public Compare(int dist, long? gam_id, int gam_ind, string gam_title, string rom_title, int rom_ind, string rregion, string dbregion)
        {
            Distance = dist;
            DBID = gam_id;
            DBIndex = gam_ind;
            DBTitle = gam_title;
            //RID = 0;
            RTitle = rom_title;
            RIndex = rom_ind;
            RRegion = rregion;
            DBRegion = dbregion;

            AcceptMatch = false;
        }
    }
}

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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Robin
{
    public partial class Game : IDBobject, INotifyPropertyChanged
    {
        Stopwatch Watch = new Stopwatch();

        private string _title = null;
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

        public string Overview
        {
            get { return Releases[0].Overview; }
        }

        public string Developer
        {
            get { return Releases[0].Developer; }
        }

        public string Publisher
        {
            get { return Releases[0].Publisher; }
        }

        string genres;
        public string Genres //TODO: Genres does not look right
        {
            get
            {
                if (genres == null)
                {
                    genres = Releases[0].Genre ?? "Unknown";
                }
                return genres;
            }
        }

        List<string> genreList;
        public List<string> GenreList
        {
            get
            {
                if (genreList == null)
                {
                    genreList = Genres.Split(',').Select(x => x.Trim()).ToList();
                }
                return genreList;
            }
        }

        public string Players
        {
            get { return Releases[0].Players; }
        }

        public string Platform
        {
            get { return Releases[0].Platform.Title; }
        }

        public long Platform_ID
        {
            get { return Releases[0].Platform_ID; }
        }


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

        public DateTime? Date
        {
            get { return Releases[0].Date; }
        }

        public long? ID_GDB
        {
            get
            { return Releases[0].ID_GDB; }
        }

        public long? ID_OVG
        {
            get { return Releases[0].ID_OVG; }
        }

        public bool Included
        {
            get { return Releases.Where(x => x.Included).Any(); }
        }


        public bool IsCrap
        {
            get { return Releases[0].IsCrap; }
            set
            {
                foreach (Release release in Releases)
                {
                    release.IsCrap = value;
                }
                OnPropertyChanged("IsCrap");
            }

        }

        public bool IsGame
        {
            get { return Releases[0].IsGame; }
            set
            {
                foreach (Release release in Releases)
                {
                    release.IsGame = value;
                }
                OnPropertyChanged("IsGame");
            }
        }

        public bool IsBeaten
        {
            get { return Releases[0].IsBeaten; }
            set
            {
                foreach (Release release in Releases)
                {
                    release.IsBeaten = value;
                }
                OnPropertyChanged("IsBeaten");
            }
        }

        public bool Unlicensed
        {
            get { return Releases[0].Unlicensed; }
        }

        public bool HasArt
        {
            get { return Releases.Any(x => x.HasArt); }
        }

        public string MainDisplay
        {
            get
            {
#if DEBUG
                Stopwatch Watch = Stopwatch.StartNew();
#endif
                string returner = null;

                if (Platform_ID == CONSTANTS.ARCADE_PLATFORM_ID)
                {
                    returner = LogoFile ?? BoxFrontFile ?? MarqueeFile ?? Releases[0].Platform.ControllerFile;
                }

                else
                {
                    returner = BoxFrontFile ?? LogoFile ?? MarqueeFile ?? Releases[0].Platform.ControllerFile;
                }

#if DEBUG
                Debug.WriteLine("MainDisplay: " + Title + " " + Watch.ElapsedMilliseconds);
#endif
                return returner;
            }
        }

        private int? _releaseCount = null;
        private int? ReleaseCount
        {
            get
            {
                if (_releaseCount == null)
                {
                    _releaseCount = Releases.Count();
                }
                return _releaseCount;
            }
        }

        private string _boxFrontFile = null;
        public string BoxFrontFile
        {
            get
            {
                if (_boxFrontFile == null)
                {
                    foreach (Release release in Releases)
                    {
                        if (File.Exists(release.BoxFrontPath))
                        {
                            _boxFrontFile = release.BoxFrontPath;
                            break;
                        }
                    }
                }

                return _boxFrontFile;
            }
        }

        private string _boxFrontThumbFile = null;
        public string BoxFrontThumbFile
        {
            get
            {
                if (_boxFrontThumbFile == null)
                {
                    foreach (Release release in Releases)
                    {
                        if (File.Exists(release.BoxFrontThumbPath))
                        {
                            _boxFrontThumbFile = release.BoxFrontThumbPath;
                            break;
                        }
                    }
                }
                return _boxFrontThumbFile;
            }
        }

        public string BoxBackFile
        {
            get { return Releases[0].BoxBackPath; }
        }

        public string BannerFile
        {
            get { return Releases[0].BannerPath; }
        }

        public string ScreenFile
        {
            get { return Releases[0].ScreenPath; }
        }

        public string LogoFile
        {
            get { return Releases[0].LogoPath; }
        }

        public string MarqueeFile
        {
            get { return Releases[0].MarqueePath; }
        }

        private Release _preferredRelease = null;
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

        bool IDBobject.Preferred
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Play(Release release)
        {
            if (release == null)
            {
                release = PreferredRelease;
            }

            release.Play(null);
        }

        public void OnPropertyChanged(string prop, string pubchoose)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void ScrapeArt()
        {
        }
    }
}


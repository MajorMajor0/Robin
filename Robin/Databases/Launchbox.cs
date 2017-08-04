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
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Robin
{
    class Launchbox : IDB
    {
        private bool platformsCached;

        public string Title { get { return "LaunchBox"; } }

        public LocalDB DB { get { return LocalDB.LaunchBox; } }

        public DbSet Platforms => R.Data.LBPlatforms;

        public DbSet Releases => R.Data.LBReleases;

        public bool HasRegions => true;

        XDocument launchboxFile;

        static string LaunchBoxDataZipFile = FileLocation.Data + "LBdata.zip";

        bool disposed;

        public Launchbox()
        {
            Reporter.Tic("Loading LaunchBox local cache...");

            R.Data.LBPlatforms.Load();
            R.Data.LBImages.Load();
            R.Data.LBReleases.Load();
            R.Data.LBGames.Load();
            
            platformsCached = false;

            Reporter.Toc();
        }

        public void GetLaunchBoxFile()
        {
            using (WebClient webclient = new WebClient())
            {
                if (File.Exists(LaunchBoxDataZipFile) && (File.GetLastWriteTime(LaunchBoxDataZipFile).Date == DateTime.Today))
                {
                    Reporter.Report("Found up to date LaunchBox zip file.");
                }
                else
                {
                    Reporter.Tic("Updating LaunchBox zip file...");

                    webclient.DownloadFileFromDB(@"http://gamesdb.launchbox-app.com/Metadata.zip", LaunchBoxDataZipFile);
                    Reporter.Toc();
                }
            }

            Reporter.Tic("Extracting info from zip file...");
            using (ZipArchive archive = ZipFile.Open(LaunchBoxDataZipFile, ZipArchiveMode.Read))
            using (var dattext = archive.GetEntry("Metadata.xml").Open())
            {
                launchboxFile = XDocument.Load(dattext);
            }
            Reporter.Toc();
        }

        public void CachePlatformData(IDBPlatform idbPlatform)
        {
            if (!platformsCached)
            {
                Reporter.Tic("Caching data for all platforms to save time.");
                if (launchboxFile == null)
                {
                    GetLaunchBoxFile();
                }

                List<XElement> platformElements = launchboxFile.Root.Elements("Platform").ToList();

                foreach (XElement platformElement in platformElements)
                {
                    string tempTitle = platformElement.SafeGetA("Name");
                    LBPlatform lbPlatform = R.Data.LBPlatforms.FirstOrDefault(x => x.Title == tempTitle);
                    if (lbPlatform == null)
                    {
                        lbPlatform = new LBPlatform();
                        R.Data.LBPlatforms.Add(lbPlatform);
                    }

                    lbPlatform.Title = tempTitle;
                    lbPlatform.Date = DateTimeRoutines.SafeGetDate(platformElement.SafeGetA("Date"));
                    lbPlatform.Developer = platformElement.SafeGetA("Developer");
                    lbPlatform.Manufacturer = platformElement.SafeGetA("Manufacturer");
                    lbPlatform.Cpu = platformElement.SafeGetA("Cpu");
                    lbPlatform.Memory = platformElement.SafeGetA("Memory");
                    lbPlatform.Graphics = platformElement.SafeGetA("Graphics");
                    lbPlatform.Sound = platformElement.SafeGetA("Sound");
                    lbPlatform.Display = platformElement.SafeGetA("Display");
                    lbPlatform.Media = platformElement.SafeGetA("Media");
                    lbPlatform.Display = platformElement.SafeGetA("Display");
                    lbPlatform.Controllers = platformElement.SafeGetA("MaxControllers");
                    lbPlatform.Category = platformElement.SafeGetA("Category");
                }
                Reporter.Toc();

                platformsCached = true;
            }

            else
            {
                Reporter.Report("Platforms recently cached--no need to cache again");
            }
        }

        public void CachePlatformReleases(IDBPlatform idbPlatform)
        {
            LBPlatform lbPlatform = idbPlatform as LBPlatform;

            if (launchboxFile == null)
            {
                GetLaunchBoxFile();
            }

            List<XElement> gameElements = launchboxFile.Root.Elements("Game").Where(x => x.Element("Platform").Value == lbPlatform.Title).ToList();
            List<XElement> imageElements = launchboxFile.Root.Elements("GameImage").ToList();
            List<XElement> gameImageElements;

            int gameCount = gameElements.Count;
            Reporter.Report("Found " + gameCount + " " + lbPlatform.Title + " games in LaunchBox file.");
            Reporter.Tic("Scanning information...");
            int j = 0;
            foreach (XElement gameElement in gameElements)
            {
                if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
                {
                    Reporter.Report("  Working " + j + " / " + gameCount + " " + lbPlatform.Title + " games.");
                }

                int id = int.Parse(gameElement.SafeGetA("DatabaseID"));
                LBGame lbGame = lbPlatform.LBGames.FirstOrDefault(x => x.ID == id);
                if (lbGame == null)
                {
                    lbGame = new LBGame();
                    lbGame.ID = id;
                    lbPlatform.LBGames.Add(lbGame);

                    Debug.WriteLine("New game: " + lbGame.Title);
                }

                // If a release has changed platforms, catch it and zero out match
                if (lbGame.LBPlatform_ID != lbPlatform.ID)
                {
                    //lbGame.LBPlatform_ID = lbPlatform.ID;
                    lbGame.LBPlatform = lbPlatform;
                    Release release = R.Data.Releases.FirstOrDefault(x => x.ID_LB == lbGame.ID);
                    if (release != null)
                    {
                        release.ID_LB = null;
                    }
                }

                lbGame.Title = gameElement.SafeGetA("Name"); ;
                lbGame.Date = DateTimeRoutines.SafeGetDateTime(gameElement.SafeGetA("ReleaseDate") ?? gameElement.SafeGetA("ReleaseYear") + @"-01-01 00:00:00");
                lbGame.Overview = gameElement.SafeGetA("Overview");
                lbGame.Genres = gameElement.SafeGetA("Genres");
                lbGame.Developer = gameElement.SafeGetA("Developer");

                lbGame.Publisher = gameElement.SafeGetA("Publisher");
                lbGame.VideoURL = gameElement.SafeGetA("VideoURL");
                lbGame.WikiURL = gameElement.SafeGetA("WikipediaURL");
                lbGame.Players = gameElement.SafeGetA("MaxPlayers");

                gameImageElements = imageElements.Where(x => x.Element("DatabaseID").Value == lbGame.ID.ToString()).ToList();
                int regionID;

                // Cache images
                foreach (XElement imageElement in gameImageElements)
                {
                    string imageElementFileName = imageElement.Element("FileName").Value;

                    LBImage lbImage = R.Data.LBImages.Local.FirstOrDefault(x => x.FileName == imageElementFileName);

                    if (lbImage == null)
                    {
                        lbImage = new LBImage();
                        lbImage.FileName = imageElement.Element("FileName").Value;
                        lbGame.LBImages.Add(lbImage);

                        Debug.WriteLine("New image: " + lbGame.Title);
                    }

                    lbImage.Type = imageElement.Element("Type").Value;
                    lbImage.LBRegion = imageElement.SafeGetA("Region") ?? "United States";

                    if (RegionDictionary.TryGetValue(lbImage.LBRegion, out regionID))
                    {
                        lbImage.Region = R.Data.Regions.FirstOrDefault(x => x.ID == regionID);
                    }

                    else
                    {
                        lbImage.Region = R.Data.Regions.FirstOrDefault(x => x.ID == 0);
                        Reporter.Report("Couldn't find " + lbImage.LBRegion + " in LB image dictionary.");
                    }

                }

                lbGame.CreateReleases();
            }
        }

        public void CachePlatformGames(IDBPlatform idbPlatform)
        {
            // TODO: split up cache platform games and cache platform releases
            throw new NotImplementedException();
        }

        public void CachePlatforms()
        {
            CachePlatformData(null);
        }

        [Conditional("DEBUG")]
        public async void CreateReleases()
        {
            await Task.Run(() =>
            {
                int i = 0;
                foreach (LBGame lbGame in R.Data.LBGames)
                {
                    lbGame.CreateReleases();
                    Reporter.Report(i++.ToString());
                }
            });
        }

        public void ReportUpdates(bool detect)
        {


#if DEBUG
            Stopwatch Watch = Stopwatch.StartNew();
#endif
            if (detect)
            {
                R.Data.ChangeTracker.DetectChanges();
                Debug.WriteLine("Detect changes: " + Watch.ElapsedMilliseconds);
#if DEBUG
                Watch.Restart();
#endif
            }
            var lbReleaseEntries = R.Data.ChangeTracker.Entries<LBRelease>();
            var lbImageEntries = R.Data.ChangeTracker.Entries<LBImage>();
            var lbGameEntries = R.Data.ChangeTracker.Entries<LBGame>();

            Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);

            int lbReleaseAddCount = lbReleaseEntries.Count(x => x.State == EntityState.Added);


            int lbImageAddCount = lbImageEntries.Count(x => x.State == EntityState.Added);
            int lbGameAddCount = lbGameEntries.Count(x => x.State == EntityState.Added);

            int lbReleaseModCount = lbReleaseEntries.Count(x => x.State == EntityState.Modified);
            int lbImageModCount = lbImageEntries.Count(x => x.State == EntityState.Modified);
            int lbGameModCount = lbGameEntries.Count(x => x.State == EntityState.Modified);

            Reporter.Report("LBReleases added: " + lbReleaseAddCount + ", LBReleases updated: " + lbReleaseModCount);
            Reporter.Report("LBImages added: " + lbImageAddCount + ", LBImages updated: " + lbImageModCount);
            Reporter.Report("LBGames added: " + lbGameAddCount + ", LBgames updated: " + lbGameModCount);
        }

        public static Dictionary<string, int> RegionDictionary = new Dictionary<string, int>
        {
            { "Asia", 1 },
            { "Australia", 2 },
            { "Brazil", 3 },
            { "Canada", 4 },
            { "China", 5 },
            { "Denmark", 6 },
            { "Europe", 7 },
            { "Finland", 8 },
            { "France", 9 },
            { "Germany", 10 },
            { "Hong Kong", 11 },
            { "Italy", 12 },
            { "Japan", 13 },
            { "Korea", 14 },
            { "The Netherlands", 15 },
            { "Russia", 16 },
            { "Spain", 17 },
            { "Sweden", 18 },
            { "United States", 21 },
            { "World", 22 },
            { "Norway", 25 },
            { "United Kingdom", 28 },
            { "North America", 21 },
            { "Oceania", 2 },
            { "South America", 3 }

        };

        public const string IMAGESURL = @"http://images.launchbox-app.com/";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Launchbox()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                //OVdata.Dispose();
            }

            // release any unmanaged objects
            // set the object references to null

            //OVdata = null;

            disposed = true;
        }
    }
}

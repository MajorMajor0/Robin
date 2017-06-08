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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Robin
{
    class Launchbox : IDB
    {
        public string Title { get { return "LaunchBox"; } }

        RobinDataEntities Rdata;

        public LocalDB DB { get { return LocalDB.GamesDB; } }

        public IEnumerable<IDBPlatform> Platforms { get { return Rdata.LBPlatforms; } }

        public IEnumerable<IDBRelease> Releases { get { return Rdata.LBGames; } }

        XDocument launchboxFile;

        static string LaunchBoxDataZipFile = FileLocation.Data + "LBdata.zip";

        bool disposed;

        public Launchbox(ref RobinDataEntities rdata)
        {
            Rdata = rdata;

            Reporter.Tic("Loading LaunchBox local cache...");
            Rdata.LBPlatforms.Load();
            Rdata.LBImages.Load();
			Rdata.LBGames.Load();

			Reporter.Toc();

            GetLaunchBoxFile();
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

        public void CachePlatforms()
        {
            //XDocument launchboxFile;

            //using (WebClient webclient = new WebClient())
            //{
            //    if (File.GetLastWriteTime(LaunchBoxDataZipFile).Date == DateTime.Today)
            //    {
            //        webclient.DownloadFileFromDB(@"http://gamesdb.launchbox-app.com/Metadata.zip", LaunchBoxDataZipFile);
            //    }
            //}

            //using (ZipArchive archive = ZipFile.Open(LaunchBoxDataZipFile, ZipArchiveMode.Read))
            //using (var dattext = archive.GetEntry("Metadata.xml").Open())
            //{
            //    launchboxFile = XDocument.Load(dattext);
            //}

            List<XElement> platformElements = launchboxFile.Root.Elements("Platform").ToList();

            foreach (XElement platformElement in platformElements)
            {
                string tempTitle = platformElement.SafeGetA("Name");
                LBPlatform lbPlatform = Rdata.LBPlatforms.FirstOrDefault(x => x.Title == tempTitle);
                if (lbPlatform == null)
                {
                    lbPlatform = new LBPlatform();
                    Rdata.LBPlatforms.Add(lbPlatform);
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

            Rdata.ChangeTracker.DetectChanges();
            Rdata.Save();
        }

        public void CachePlatformReleases(Platform platform)
        {
            List<XElement> gameElements = launchboxFile.Root.Elements("Game").Where(x => x.Element("Platform").Value == platform.LBPlatform.Title).ToList();
            List<XElement> imageElements = launchboxFile.Root.Elements("GameImage").ToList();
            List<XElement> gameImageElements;

            int gameCount = gameElements.Count;
            Reporter.Report("Found " + gameCount + " " + platform.Title + " games.");
            Reporter.Report("Scanning information...");
            int j = 0;
            foreach (XElement gameElement in gameElements)
            {
                if (++j % (gameCount / 10) == 0)
                {
                    Reporter.Report("Working " + j + " / " + gameCount);
                }

                int id = int.Parse(gameElement.SafeGetA("DatabaseID"));
                LBGame lbGame = platform.LBPlatform.LBGames.FirstOrDefault(x => x.ID == id);
                if (lbGame == null)
                {
                    lbGame = new LBGame();
                    lbGame.ID = id;
                    platform.LBPlatform.LBGames.Add(lbGame);
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
                foreach (XElement imageElement in gameImageElements)
                {
                    string imageElementFileName = imageElement.Element("FileName").Value;

                    LBImage lbImage = Rdata.LBImages.Local.FirstOrDefault(x => x.FileName == imageElementFileName);

                    if (lbImage == null)
                    {
                        lbImage = new LBImage();
                        lbImage.FileName = imageElement.Element("FileName").Value;
                        Rdata.LBImages.Add(lbImage);

                    }

                    lbImage.Type = imageElement.Element("Type").Value;
                    lbImage.Game_ID = lbGame.ID;
                    lbImage.LBRegion = imageElement.SafeGetA("Region") ?? "United States";

                    if (RegionDictionary.TryGetValue(lbImage.LBRegion, out regionID))
                    {
                        lbImage.Region_ID = regionID; ;
                    }

                    else
                    {
                        lbImage.Region_ID = 0;
                        Reporter.Report("Couldn't find " + lbImage.LBRegion + " in LB image dictionary.");
                    }

                    lbImage.Region_ID = RegionDictionary[lbImage.LBRegion];
                }
            }
        }

        public void CachePlatformGames(Platform platform)
        {
            Reporter.Report("Launchbox does not have games and releases--try caching releases");
        }

        public void CachePlatformData(Platform platform)
        {
            Reporter.Tic("Caching data for all platforms to save time.");
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

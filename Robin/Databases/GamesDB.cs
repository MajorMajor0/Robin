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
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Data.Entity;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

namespace Robin
{
	class GamesDB : IDB
	{
		public string Title { get { return "Games DB"; } }

		public LocalDB DB { get { return LocalDB.GamesDB; } }

		public DbSet Platforms => R.Data.GDBPlatforms;

		public DbSet Releases => R.Data.GDBReleases;

		bool disposed;

		public GamesDB()
		{
			Reporter.Tic("Opening Games DB cache...");

			R.Data.GDBPlatforms.Load();
			R.Data.GDBReleases.Load();
			Reporter.Toc();
		}

		public void CachePlatformReleases(Platform platform)
		{
			GDBPlatform gdbPlatform = platform.GDBPlatform;

			int gamesAdded = 0;
			XDocument xDocument = new XDocument();
			string downloadText;

			Reporter.Tic("Caching releases from GamesDB.net...");
			string url = @"http://thegamesdb.net/api/GetPlatformGames.php?platform=" + gdbPlatform.ID;

			using (WebClient webclient = new WebClient())
			{
				if (webclient.SafeDownloadStringDB(url, out downloadText))
				{
					xDocument = XDocument.Parse(downloadText);

					foreach (XElement element in xDocument.Root.Elements("Game"))
					{
						int id = int.Parse(element.SafeGetA("id"));
						string title = element.SafeGetA("GameTitle");

						if (!gdbPlatform.GDBReleases.Any(x => x.ID == id) && title != null)
						{
							gamesAdded++;
							gdbPlatform.GDBReleases.Add(new GDBRelease()
							{
								ID = id,
								Title = title,
								Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("ReleaseDate") ?? "01-01-1901"),
								GDBPlatform_ID = gdbPlatform.ID
							});
							Debug.WriteLine("Added game");
						}
					}
				}

				Reporter.Toc();
				Reporter.ReportInline(" " + gamesAdded + " releases added from Games DB.");
			}
		}

		public void CachePlatformData(Platform platform)
		{
			GDBPlatform gdbplatform = platform.GDBPlatform;

			XDocument xdoc = new XDocument();
			string url;
			string downloadtext = "";

			string urlbase = @"http://thegamesdb.net/api/GetPlatform.php?id=";
			using (WebClient webclient = new WebClient())
			{
				// Ensure the art directory exists
				Directory.CreateDirectory(FileLocation.Art.Console);

				// Assemble the platformsdb url from the platform data and the base API url
				url = urlbase + gdbplatform.ID;

				// Pull down the xml file containing platform data from gamesdb
				if (webclient.SafeDownloadStringDB(url, out downloadtext))
				{
					xdoc = XDocument.Parse(downloadtext);

					gdbplatform.Title = xdoc.SafeGetB("Platform", "Platform");
					gdbplatform.Developer = xdoc.SafeGetB("Platform", "developer");
					gdbplatform.Manufacturer = xdoc.SafeGetB("Platform", "manufacturer");
					gdbplatform.Cpu = xdoc.SafeGetB("Platform", "cpu");
					gdbplatform.Sound = xdoc.SafeGetB("Platform", "sound");
					gdbplatform.Display = xdoc.SafeGetB("Platform", "display");
					gdbplatform.Media = xdoc.SafeGetB("Platform", "media");
					gdbplatform.Controllers = xdoc.SafeGetB("Platform", "maxcontrollers");
					gdbplatform.Rating = decimal.Parse(xdoc.SafeGetB("Platform", "rating") ?? "0");
					gdbplatform.Overview = xdoc.SafeGetB("Platform", "overview");

					string BaseImageUrl;
					string BoxFrontUrl;
					string BoxBackUrl;
					string BannerUrl;
					string ConsoleUrl;
					string ControllerUrl;

					BaseImageUrl = xdoc.SafeGetB("baseImgUrl");

					if (BaseImageUrl != null)
					{
						BoxFrontUrl = xdoc.SafeGetBoxArt("front", type: "Platform");
						BoxBackUrl = xdoc.SafeGetBoxArt("back", type: "Platform");
						BannerUrl = xdoc.SafeGetB("Platform", "Images", "banner");
						ConsoleUrl = xdoc.SafeGetB("Platform", "Images", "consoleart");
						ControllerUrl = xdoc.SafeGetB("Platform", "Images", "controllerart");

						gdbplatform.BoxFrontURL = BoxFrontUrl != null ? BaseImageUrl + BoxFrontUrl : null;
						gdbplatform.BoxBackURL = BoxBackUrl != null ? BaseImageUrl + BoxBackUrl : null;
						gdbplatform.BannerURL = BannerUrl != null ? BaseImageUrl + BannerUrl : null;
						gdbplatform.ConsoleURL = ConsoleUrl != null ? BaseImageUrl + ConsoleUrl : null;
						gdbplatform.ControllerURL = ControllerUrl != null ? BaseImageUrl + ControllerUrl : null;
					}

				}
				else
				{
					Reporter.Warn("Failure getting " + platform.Title + " data from Games DB.");
				}
			}
		}

		public void CacheReleaseData(GDBRelease gdbRelease)
		{
			XDocument xDocument = new XDocument();
			string downloadText = "";

			string url = @"http://thegamesdb.net/api/GetGame.php?id=" + gdbRelease.ID;
			using (WebClient webclient = new WebClient())
			{
				// Ensure the art directory exists
				Directory.CreateDirectory(FileLocation.Art.Console);

				// Pull down the xml file containing platform data from gamesdb
				if (webclient.SafeDownloadStringDB(url, out downloadText))
				{
					string coop;
					xDocument = XDocument.Parse(downloadText);

					gdbRelease.Title = xDocument.SafeGetB("Game", "GameTitle");
					gdbRelease.Developer = xDocument.SafeGetB("Game", "Developer");
					gdbRelease.Publisher = xDocument.SafeGetB("Game", "Publisher");
					gdbRelease.Players = xDocument.SafeGetB("Game", "Players");
					gdbRelease.Overview = xDocument.SafeGetB("Game", "Overview");
					gdbRelease.Rating = decimal.Parse(xDocument.SafeGetB("Game", "Rating") ?? "0", CultureInfo.InvariantCulture);
					gdbRelease.Genre = string.Join(",", xDocument.Root.Descendants("genre").Select(x => x.Value));
					gdbRelease.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("Game", "ReleaseDate"));
					coop = xDocument.SafeGetB("Game", "Co-op");
					if ((coop != null) && ((coop.ToLower() == "true") || (coop.ToLower() == "yes")))
					{
						gdbRelease.Coop = true;
					}
					else
					{
						gdbRelease.Coop = false;
					}

					string BaseImageUrl;
					BaseImageUrl = xDocument.SafeGetB("baseImgUrl");

					if (BaseImageUrl != null)
					{
						url = xDocument.SafeGetBoxArt("front");
						gdbRelease.BoxFrontURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetBoxArt("back");
						gdbRelease.BoxBackURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetB("Game", "Images", "banner");
						gdbRelease.BannerURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetB("Game", "Images", "screenshot", "original");
						gdbRelease.ScreenURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetB("Game", "Images", "clearlogo");
						gdbRelease.LogoURL = url != null ? BaseImageUrl + url : null;
					}
				}
				else
				{
					Reporter.Report("Failure getting " + gdbRelease.Title + ", ID " + gdbRelease.ID + " from Games DB."); ;
				}
			}
		}

		public void CachePlatform(Platform platform)
		{
			Stopwatch Watch2 = new Stopwatch();

			GDBPlatform gdbPlatform = R.Data.GDBPlatforms.FirstOrDefault(x => x.ID == platform.ID_GDB);

			Reporter.Tic("Getting platform data from Games DB...");
			CachePlatformData(platform);
			Reporter.Toc();

			CachePlatformReleases(platform);

			Reporter.Tic("Getting release data from Games DB...");

			int releaseCount = gdbPlatform.GDBReleases.Count();
			int i = 0;
			Watch2.Start();

			foreach (GDBRelease gdbRelease in gdbPlatform.GDBReleases)
			{
				if (i++ % (releaseCount / 10) == 0)
				{
					Reporter.Report(i + " / " + releaseCount + ", " + Watch2.Elapsed.ToString(@"m\:ss")); Watch2.Restart();
				}

				CacheReleaseData(gdbRelease);
			}

			gdbPlatform.CacheDate = DateTime.Now;

			// TODO Check number of added and updated here and report
			R.Data.ChangeTracker.DetectChanges();
			int n = R.Data.Save();
			Reporter.Report("Finished caching " + gdbPlatform.Title + " from Games DB, " + n + " changes pushed to database.");
		}

		public void CachePlatformGames(Platform platform)
		{
			Reporter.Report("GamesDB does not have games and releases--try caching releases");
		}

		public void CachePlatforms()
		{
			// TODO Get list of platforms from giantbom and cache in GBPlatforms
			Reporter.Report("Cache platforms not yet implemented for GamesDB");
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GamesDB()
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
				//R.Data.Dispose();
			}

			// release any unmanaged objects
			// set the object references to null

			//R.Data = null;

			disposed = true;
		}
	}
}
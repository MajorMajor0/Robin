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

namespace Robin
{
	class GamesDB
	{
		public static int CachePlatformGames(GDBPlatform gdbPlatform)
		{
			int gamesAdded = 0;
			//ObservableCollection<GDBRelease> List = new ObservableCollection<GDBRelease>();
			XDocument xDocument = new XDocument();
			string downloadText;

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

						if (!gdbPlatform.GDBReleases.Any(x => x.ID == id) && title !=null)
						{
							gamesAdded++;
							gdbPlatform.GDBReleases.Add(new GDBRelease()
							{
								ID = id,
								Title = title,
								Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("ReleaseDate") ?? "01-01-1901"),
								Platform_ID = gdbPlatform.ID
							});
							Debug.WriteLine("Added game");
						}
					}
				}
				return gamesAdded;

			}
		}

		public static bool CachePlatformData(GDBPlatform gdbplatform)
		{
			//reporter.Report("Trying GamesDB");
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
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public static bool CacheGameData(GDBRelease game)
		{
			XDocument xDocument = new XDocument();
			string downloadText = "";

			string url = @"http://thegamesdb.net/api/GetGame.php?id=" + game.ID;
			using (WebClient webclient = new WebClient())
			{
				// Ensure the art directory exists
				Directory.CreateDirectory(FileLocation.Art.Console);

				// Pull down the xml file containing platform data from gamesdb
				if (webclient.SafeDownloadStringDB(url, out downloadText))
				{
					string coop;
					xDocument = XDocument.Parse(downloadText);

					game.Title = xDocument.SafeGetB("Game", "GameTitle");
					game.Developer = xDocument.SafeGetB("Game", "Developer");
					game.Publisher = xDocument.SafeGetB("Game", "Publisher");
					game.Players = xDocument.SafeGetB("Game", "Players");
					game.Overview = xDocument.SafeGetB("Game", "Overview");
					game.Rating = decimal.Parse(xDocument.SafeGetB("Game", "Rating") ?? "0", CultureInfo.InvariantCulture);
					game.Genre = string.Join(",", xDocument.Root.Descendants("genre").Select(x => x.Value));
					game.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("Game", "ReleaseDate"));
					coop = xDocument.SafeGetB("Game", "Co-op");
					if ((coop != null) && ((coop.ToLower() == "true") || (coop.ToLower() == "yes")))
					{
						game.Coop = true;
					}
					else
					{
						game.Coop = false;
					}

					string BaseImageUrl;
					BaseImageUrl = xDocument.SafeGetB("baseImgUrl");

					if (BaseImageUrl != null)
					{
						url = xDocument.SafeGetBoxArt("front");
						game.BoxFrontURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetBoxArt("back");
						game.BoxBackURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetB("Game", "Images", "banner");
						game.BannerURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetB("Game", "Images", "screenshot", "original");
						game.ScreenURL = url != null ? BaseImageUrl + url : null;

						url = xDocument.SafeGetB("Game", "Images", "clearlogo");
						game.LogoURL = url != null ? BaseImageUrl + url : null;
					}
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public static void CachePlatform(Platform platform)
		{
			using (RobinDataEntities Rdata = new RobinDataEntities())
			{
				int gamesAdded = 0;
				bool cacheSuccess = false;

				Stopwatch Watch1 = new Stopwatch();
				Stopwatch Watch2 = new Stopwatch();

				GDBPlatform gdbPlatform = Rdata.GDBPlatforms.FirstOrDefault(x => x.ID == platform.ID_GDB);

				Reporter.Report("Opening local cache..."); Watch1.Start();

				Rdata.Configuration.LazyLoadingEnabled = false;
				Rdata.Configuration.AutoDetectChangesEnabled = false;
				Rdata.GDBPlatforms.Load();
				Rdata.GDBReleases.Load();

				Reporter.ReportInline(Watch1.Elapsed.ToString(@"m\:ss")); Watch1.Restart();

				Reporter.Report("Getting platform data from Games DB...");
				cacheSuccess = CachePlatformData(gdbPlatform);

				if (cacheSuccess)
				{
					Reporter.Report("Got platform data, " + Watch1.Elapsed.ToString(@"m\:ss") + " elapsed."); Watch1.Restart();
				}
				else
				{
					Reporter.Report("Failure caching " + gdbPlatform.Title + " from Games DB");
				}

				Reporter.Report("Trying to get list of platform games from Games DB...");
				gamesAdded = CachePlatformGames(gdbPlatform);
				Reporter.Report(gdbPlatform.GDBReleases.Count() + " games returned, " + Watch1.Elapsed.ToString(@"m\:ss") + " elapsed.");
				Watch1.Restart();

				Reporter.Report("Getting game data from Games DB...");
				if (gdbPlatform.GDBReleases.Count() > 0)
				{

					int releaseCount = gdbPlatform.GDBReleases.Count();
					int i = 0;

					Watch2.Start();
					foreach (GDBRelease game in gdbPlatform.GDBReleases)
					{
						if (i++ % (releaseCount / 10) == 0)
						{
							Reporter.Report(i + " / " + releaseCount + ", " + Watch2.Elapsed.ToString(@"m\:ss")); Watch2.Restart();
						}

						cacheSuccess = CacheGameData(game);
						if (!cacheSuccess)
						{
							Reporter.Report("Failure getting " + game.Title + ", ID " + game.ID + " from Games DB.");
						}
					}

				}

				Reporter.Report("Updated " + gamesAdded + " games from GamesDB");

				gdbPlatform.CacheDate = DateTime.Now;

				Rdata.ChangeTracker.DetectChanges();
				Rdata.Configuration.AutoDetectChangesEnabled = true;
				int n = Rdata.Save();
				Reporter.Report("Finished caching " + gdbPlatform.Title + " from Games DB, " + Watch1.Elapsed.ToString(@"m\:ss") + " elapsed. " + n + " changes pushed to database.");
			}
		}
	}
}
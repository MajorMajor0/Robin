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
using System.Net;
using System.Xml.Linq;
using System.Data.Entity;
using System.Globalization;
using System.Collections.Generic;

namespace Robin
{
	class GamesDB : IDB
	{
		public string Title => "Games DB";

		public LocalDB DB => LocalDB.GamesDB;

		public DbSet Platforms => R.Data.GDBPlatforms;

		public DbSet Releases => R.Data.GDBReleases;

		public bool HasRegions => false;

		bool disposed;

		public GamesDB()
		{
			Reporter.Tic("Opening Games DB cache...", out int tic1);

			R.Data.GDBPlatforms.Load();
			R.Data.GDBReleases.Load();
			Reporter.Toc(tic1);
		}
		/// <summary>
		/// Implement IDB.CachePlatfomrReleases(). Go out to gamesdb.com and cache all known releases for the specified platform. Update the list of releases and store metadata for each one.
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the GDBPlatform to cache.</param>
		public void CachePlatformReleases(Platform platform)
		{
			Reporter.Tic($"Getting {platform.Title} release list from Games DB...", out int tic1);
			GDBPlatform gdbPlatform = platform.GDBPlatform;

			// Update list of GDBReleases for this platform from xml file
			using (WebClient webclient = new WebClient())
			{
				// API to get xml file containing all gamesdb releases for this platform.
				string url = @"http://thegamesdb.net/api/GetPlatformGames.php?platform=" + gdbPlatform.ID;

				// Put existing GDBReleases in a dictionary for lookup performance
				var existingGDBReleaseDict = R.Data.GDBReleases.ToDictionary(x => x.ID);
				HashSet<GDBRelease> newGDBReleases = new HashSet<GDBRelease>();

				if (webclient.SafeDownloadStringDB(url, out string downloadText))
				{
					XDocument xDocument = XDocument.Parse(downloadText);

					foreach (XElement element in xDocument.Root.Elements("Game"))
					{
						// Don't create this game if the title is null
						string title = element.Element("GameTitle")?.Value;
						if (string.IsNullOrEmpty(title))
						{
							continue;
						}

						// Check if gbdRelease exists before creating new one. Whether it exists or not, overwrite properties with properties from xml file.
						long id = long.Parse(element.Element("id")?.Value);
						if (!existingGDBReleaseDict.TryGetValue(id, out GDBRelease gdbRelease))
						{
							gdbRelease = new GDBRelease { ID = id };
							newGDBReleases.Add(gdbRelease);
						}

						gdbRelease.Title = title;
						gdbRelease.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("ReleaseDate") ?? "01-01-1901");

						// If a release has changed platforms, catch it and zero out match
						if (gdbRelease.GDBPlatform_ID != gdbPlatform.ID)
						{
							gdbRelease.GDBPlatform_ID = gdbPlatform.ID;
							Release release = R.Data.Releases.FirstOrDefault(x => x.ID_GDB == id);
							if (release != null)
							{
								release.ID_GDB = null;
							}
						}
					}
				}
				gdbPlatform.GDBReleases.UnionWith(newGDBReleases);
				Reporter.Toc(tic1);
			}

			// Temporarily set wait time to 1 ms while caching tens of thousands of games. Sorry GDB.
			int waitTimeHolder = DBTimers.GamesDB.WaitTime;
			DBTimers.GamesDB.WaitTime = 1;

			int releaseCount = gdbPlatform.GDBReleases.Count;
			int i = 0;

			// Cache metadata for each individual game
			foreach (GDBRelease gdbRelease in gdbPlatform.GDBReleases)
			{
				if (releaseCount / 10 != 0 && i++ % (releaseCount / 10) == 0)
				{
					Reporter.Report($"{i} / {releaseCount}");
				}

				CacheReleaseData(gdbRelease);
			}
			DBTimers.GamesDB.WaitTime = waitTimeHolder;
		}

		/// <summary>
		/// Implements IDB.CachePlatformdata() Update the local DB cache of platform associated metadata
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the DBPlatorm to update.</param>
		public void CachePlatformData(Platform platform)
		{
			Reporter.Tic("Getting " + platform.Title + " data from Games DB...", out int tic1);

			GDBPlatform gdbPlatform = platform.GDBPlatform;

			XDocument xdoc;
			string url;

			string urlbase = @"http://thegamesdb.net/api/GetPlatform.php?id=";
			using (WebClient webclient = new WebClient())
			{
				// Assemble the platformsdb url from the platform data and the base API url
				url = urlbase + gdbPlatform.ID;

				// Pull down the xml file containing platform data from gamesdb
				if (webclient.SafeDownloadStringDB(url, out string downloadtext))
				{
					xdoc = XDocument.Parse(downloadtext);

					gdbPlatform.Title = xdoc.SafeGetB("Platform", "Platform");
					gdbPlatform.Developer = xdoc.SafeGetB("Platform", "developer");
					gdbPlatform.Manufacturer = xdoc.SafeGetB("Platform", "manufacturer");
					gdbPlatform.Cpu = xdoc.SafeGetB("Platform", "cpu");
					gdbPlatform.Sound = xdoc.SafeGetB("Platform", "sound");
					gdbPlatform.Display = xdoc.SafeGetB("Platform", "display");
					gdbPlatform.Media = xdoc.SafeGetB("Platform", "media");
					gdbPlatform.Controllers = xdoc.SafeGetB("Platform", "maxcontrollers");
					gdbPlatform.Rating = decimal.Parse(xdoc.SafeGetB("Platform", "rating") ?? "0");
					gdbPlatform.Overview = xdoc.SafeGetB("Platform", "overview");

					string BaseImageUrl = xdoc.SafeGetB("baseImgUrl");

					if (BaseImageUrl != null)
					{
						string BoxFrontUrl = xdoc.SafeGetBoxArt("front", type: "Platform");
						string BoxBackUrl = xdoc.SafeGetBoxArt("back", type: "Platform");
						string BannerUrl = xdoc.SafeGetB("Platform", "Images", "banner");
						string ConsoleUrl = xdoc.SafeGetB("Platform", "Images", "consoleart");
						string ControllerUrl = xdoc.SafeGetB("Platform", "Images", "controllerart");

						gdbPlatform.BoxFrontURL = BoxFrontUrl != null ? BaseImageUrl + BoxFrontUrl : null;
						gdbPlatform.BoxBackURL = BoxBackUrl != null ? BaseImageUrl + BoxBackUrl : null;
						gdbPlatform.BannerURL = BannerUrl != null ? BaseImageUrl + BannerUrl : null;
						gdbPlatform.ConsoleURL = ConsoleUrl != null ? BaseImageUrl + ConsoleUrl : null;
						gdbPlatform.ControllerURL = ControllerUrl != null ? BaseImageUrl + ControllerUrl : null;
					}

				}
				else
				{
					Reporter.Warn("Failure getting " + gdbPlatform.Title + " data from Games DB.");
				}
			}
			Reporter.Toc(tic1);
		}

		/// <summary>
		/// Cache metadata from gamesdb.com API for a GDBRelease.
		/// </summary>
		/// <param name="gdbRelease">GDBRelease whose metadat is to be cached.</param>
		public void CacheReleaseData(GDBRelease gdbRelease)
		{
			// URL of gamesdb API to cache metadata for one release
			string url = @"http://thegamesdb.net/api/GetGame.php?id=" + gdbRelease.ID;
			using (WebClient webclient = new WebClient())
			{
				// Pull down the xml file containing game data from gamesdb
				if (webclient.SafeDownloadStringDB(url, out string downloadText))
				{
					XDocument xDocument = XDocument.Parse(downloadText);

					gdbRelease.Title = xDocument.SafeGetB("Game", "GameTitle") ?? gdbRelease.Title;
					gdbRelease.Developer = xDocument.SafeGetB("Game", "Developer") ?? gdbRelease.Developer;
					gdbRelease.Publisher = xDocument.SafeGetB("Game", "Publisher") ?? gdbRelease.Publisher;
					gdbRelease.Players = xDocument.SafeGetB("Game", "Players") ?? gdbRelease.Players;
					gdbRelease.Overview = xDocument.SafeGetB("Game", "Overview") ?? gdbRelease.Overview;

					gdbRelease.Rating = decimal.Parse(xDocument.SafeGetB("Game", "Rating") ?? "0", CultureInfo.InvariantCulture);
					gdbRelease.Genre = string.Join(",", xDocument.Root.Descendants("genre").Select(x => x.Value));
					gdbRelease.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("Game", "ReleaseDate"));

					string coop = xDocument.SafeGetB("Game", "Co-op");
					if ((coop != null) && ((coop.ToLower() == "true") || (coop.ToLower() == "yes")))
					{
						gdbRelease.Coop = true;
					}
					else
					{
						gdbRelease.Coop = false;
					}

					string BaseImageUrl = xDocument.SafeGetB("baseImgUrl");

					if (BaseImageUrl != null)
					{
						url = xDocument.SafeGetBoxArt("front");
						if (url != null)
						{
							gdbRelease.BoxFrontURL = BaseImageUrl + url;
						}

						url = xDocument.SafeGetBoxArt("back");
						if (url != null)
						{
							gdbRelease.BoxBackURL = BaseImageUrl + url;
						}

						url = xDocument.SafeGetB("Game", "Images", "banner");
						if (url != null)
						{
							gdbRelease.BannerURL = BaseImageUrl + url;
						}

						url = xDocument.SafeGetB("Game", "Images", "screenshot", "original");
						if (url != null)
						{
							gdbRelease.ScreenURL = BaseImageUrl + url;
						}

						url = xDocument.SafeGetB("Game", "Images", "clearlogo");
						if (url != null)
						{
							gdbRelease.LogoURL = BaseImageUrl + url;
						}
					}
				}
				else
				{
					Reporter.Report("Failure getting " + gdbRelease.Title + ", ID " + gdbRelease.ID + " from Games DB.");
				}
			}
		}

		public void CachePlatformGames(Platform platform)
		{
			Reporter.Report("GamesDB does not have games and releases--try caching releases");
		}

		/// <summary>
		/// Implements IDB.CachePlatforms(). Update the list of GDBPlatforms in the local cache.
		/// </summary>
		public void CachePlatforms()
		{
			// TODO Get list of platforms from gamesdb and cache in GDBPlatforms
			Reporter.Report("Cache platforms not yet implemented for GamesDB");
		}

		/// <summary>
		/// Implements IDB.ReportUpdates(). Report to the UI how many database entries and of what type have been updated or added since the last save changes for a local DB cache.
		/// </summary>
		/// <param name="detect">Whether to detect changes prior to reporting. Detecting changes takes about 4 seconds. This can be set to false if no changes have been made since the last detect changes. Detecting changes is only necessary for updates, it is not necessary to detect additions.</param>
		public void ReportUpdates(bool detect)
		{

#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			if (detect)
			{
				R.Data.ChangeTracker.DetectChanges();
#if DEBUG
				Debug.WriteLine("Detect changes: " + Watch.ElapsedMilliseconds);
				Watch.Restart();
#endif
			}
			var gdbReleaseEntries = R.Data.ChangeTracker.Entries<GDBRelease>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int gdbReleaseAddCount = gdbReleaseEntries.Count(x => x.State == EntityState.Added);
			int gdbReleaseModCount = gdbReleaseEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("GDBReleases added: " + gdbReleaseAddCount + ", GDBReleases updated: " + gdbReleaseModCount);
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
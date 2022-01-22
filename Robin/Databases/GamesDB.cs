﻿/*This file is part of Robin.
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
using System.Globalization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Robin
{
	class GamesDB : IDB
	{
		private readonly string baseUrl = @"http://legacy.thegamesdb.net/api/";

		public string Title => "Games DB";

		public LocalDB DB => LocalDB.GamesDB;

		public IEnumerable<IDbPlatform> Platforms =>
			R.Data.Gdbplatforms.Local.ToObservableCollection();

		public IEnumerable<IDbRelease> Releases =>
			R.Data.Gdbreleases.Local.ToObservableCollection();

		public bool HasRegions => false;

		bool disposed;

		public GamesDB()
		{
			Stopwatch watch = Stopwatch.StartNew();

			Stopwatch watch2 = Stopwatch.StartNew();
			try
			{
				Reporter.Tic("Opening Games DB cache...", out int tic1);

				R.Data.Gdbplatforms.Load();
				R.Data.Gdbreleases.Load();
				Reporter.Toc(tic1);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, $"Problem Opening GamesDB from RobinData", MessageBoxButton.OK);
			}
			catch (Microsoft.Data.Sqlite.SqliteException ex)
			{
				MessageBox.Show(ex.Message, "Sqlite Exception loading GamesDB", MessageBoxButton.OK);
			}


			Reporter.Report($"GamesDB loaded {watch.Elapsed.TotalSeconds:F1} s.");
		}
		/// <summary>
		/// Implement IDB.CachePlatfomrReleases(). Go out to gamesdb.com and cache all known releases for the specified platform. Update the list of releases and store metadata for each one.
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the Gdbplatform to cache.</param>
		public void CachePlatformReleases(Platform platform)
		{
			Reporter.Tic($"Getting {platform.Title} release list from Games DB...", out int tic1);
			Gdbplatform Gdbplatform = platform.Gdbplatform;

			// Update list of Gdbreleases for this platform from xml file
			using (WebClient webclient = new WebClient())
			{
				// API to get xml file containing all gamesdb releases for this platform.
				string url = $"{baseUrl}GetPlatformGames.php?platform=" + Gdbplatform.Id;

				// Put existing Gdbreleases in a dictionary for lookup performance
				var existingGdbreleaseDict = R.Data.Gdbreleases.ToDictionary(x => x.Id);
				HashSet<Gdbrelease> newGdbreleases = new HashSet<Gdbrelease>();

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
						if (!existingGdbreleaseDict.TryGetValue(id, out Gdbrelease Gdbrelease))
						{
							Gdbrelease = new Gdbrelease { Id = id };
							newGdbreleases.Add(Gdbrelease);
						}

						Gdbrelease.Title = title;
						Gdbrelease.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("ReleaseDate") ?? "01-01-1901");

						// If a release has changed platforms, catch it and zero out match
						if (Gdbrelease.GdbplatformId != Gdbplatform.Id)
						{
							Gdbrelease.GdbplatformId = Gdbplatform.Id;
							Release release = R.Data.Releases.FirstOrDefault(x => x.ID_GDB == id);
							if (release != null)
							{
								release.ID_GDB = null;
							}
						}
					}
				}
				Gdbplatform.Gdbreleases.UnionWith(newGdbreleases);
				Reporter.Toc(tic1);
			}

			// Temporarily set wait time to 1 ms while caching tens of thousands of games. Sorry GDB.
			int waitTimeHolder = DBTimers.GamesDB.WaitTime;
			DBTimers.GamesDB.WaitTime = 1;

			int releaseCount = Gdbplatform.Gdbreleases.Count;
			int i = 0;

			// Cache metadata for each individual game
			foreach (Gdbrelease Gdbrelease in Gdbplatform.Gdbreleases)
			{
				if (releaseCount / 10 != 0 && i++ % (releaseCount / 10) == 0)
				{
					Reporter.Report($"{i} / {releaseCount}");
				}

				CacheReleaseData(Gdbrelease);
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

			Gdbplatform Gdbplatform = platform.Gdbplatform;

			XDocument xdoc;
			string url;

			string urlbase = $"{baseUrl}GetPlatform.php?id=";
			using (WebClient webclient = new WebClient())
			{
				// Assemble the platformsdb url from the platform data and the base API url
				url = urlbase + Gdbplatform.Id;

				// Pull down the xml file containing platform data from gamesdb
				if (webclient.SafeDownloadStringDB(url, out string downloadtext))
				{
					xdoc = XDocument.Parse(downloadtext);

					Gdbplatform.Title = xdoc.SafeGetB("Platform", "Platform");
					Gdbplatform.Developer = xdoc.SafeGetB("Platform", "developer");
					Gdbplatform.Manufacturer = xdoc.SafeGetB("Platform", "manufacturer");
					Gdbplatform.Cpu = xdoc.SafeGetB("Platform", "cpu");
					Gdbplatform.Sound = xdoc.SafeGetB("Platform", "sound");
					Gdbplatform.Display = xdoc.SafeGetB("Platform", "display");
					Gdbplatform.Media = xdoc.SafeGetB("Platform", "media");
					Gdbplatform.Controllers = xdoc.SafeGetB("Platform", "maxcontrollers");
					Gdbplatform.Rating = double.Parse(xdoc.SafeGetB("Platform", "rating") ?? "0");
					Gdbplatform.Overview = xdoc.SafeGetB("Platform", "overview");

					string BaseImageUrl = xdoc.SafeGetB("baseImgUrl");

					if (BaseImageUrl != null)
					{
						string BoxFrontUrl = xdoc.SafeGetBoxArt("front", type: "Platform");
						string BoxBackUrl = xdoc.SafeGetBoxArt("back", type: "Platform");
						string BannerUrl = xdoc.SafeGetB("Platform", "Images", "banner");
						string ConsoleUrl = xdoc.SafeGetB("Platform", "Images", "consoleart");
						string ControllerUrl = xdoc.SafeGetB("Platform", "Images", "controllerart");

						Gdbplatform.BoxFrontUrl = BoxFrontUrl != null ? BaseImageUrl + BoxFrontUrl : null;
						Gdbplatform.BoxBackUrl = BoxBackUrl != null ? BaseImageUrl + BoxBackUrl : null;
						Gdbplatform.BannerUrl = BannerUrl != null ? BaseImageUrl + BannerUrl : null;
						Gdbplatform.ConsoleUrl = ConsoleUrl != null ? BaseImageUrl + ConsoleUrl : null;
						Gdbplatform.ControllerUrl = ControllerUrl != null ? BaseImageUrl + ControllerUrl : null;
					}

				}
				else
				{
					Reporter.Warn("Failure getting " + Gdbplatform.Title + " data from Games DB.");
				}
			}
			Reporter.Toc(tic1);
		}

		/// <summary>
		/// Cache metadata from gamesdb.com API for a Gdbrelease.
		/// </summary>
		/// <param name="Gdbrelease">Gdbrelease whose metadat is to be cached.</param>
		public void CacheReleaseData(Gdbrelease Gdbrelease)
		{
			// URL of gamesdb API to cache metadata for one release
			string url = $"{baseUrl}GetGame.php?id=" + Gdbrelease.Id;
			using (WebClient webclient = new WebClient())
			{
				// Pull down the xml file containing game data from gamesdb
				if (webclient.SafeDownloadStringDB(url, out string downloadText))
				{
					XDocument xDocument = XDocument.Parse(downloadText);

					Gdbrelease.Title = xDocument.SafeGetB("Game", "GameTitle") ?? Gdbrelease.Title;
					Gdbrelease.Developer = xDocument.SafeGetB("Game", "Developer") ?? Gdbrelease.Developer;
					Gdbrelease.Publisher = xDocument.SafeGetB("Game", "Publisher") ?? Gdbrelease.Publisher;
					Gdbrelease.Players = xDocument.SafeGetB("Game", "Players") ?? Gdbrelease.Players;
					Gdbrelease.Overview = xDocument.SafeGetB("Game", "Overview") ?? Gdbrelease.Overview;

					Gdbrelease.Rating = double.Parse(xDocument.SafeGetB("Game", "Rating") ?? "0", CultureInfo.InvariantCulture);
					Gdbrelease.Genre = string.Join(",", xDocument.Root.Descendants("genre").Select(x => x.Value));
					Gdbrelease.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("Game", "ReleaseDate"));

					string coop = xDocument.SafeGetB("Game", "Co-op");
					if ((coop != null) && ((coop.ToLower() == "true") || (coop.ToLower() == "yes")))
					{
						Gdbrelease.Coop = true;
					}
					else
					{
						Gdbrelease.Coop = false;
					}

					string BaseImageUrl = xDocument.SafeGetB("baseImgUrl");

					if (BaseImageUrl != null)
					{
						url = xDocument.SafeGetBoxArt("front");
						if (url != null)
						{
							Gdbrelease.BoxFrontUrl = BaseImageUrl + url;
						}

						url = xDocument.SafeGetBoxArt("back");
						if (url != null)
						{
							Gdbrelease.BoxBackUrl = BaseImageUrl + url;
						}

						url = xDocument.SafeGetB("Game", "Images", "banner");
						if (url != null)
						{
							Gdbrelease.BannerUrl = BaseImageUrl + url;
						}

						url = xDocument.SafeGetB("Game", "Images", "screenshot", "original");
						if (url != null)
						{
							Gdbrelease.ScreenUrl = BaseImageUrl + url;
						}

						url = xDocument.SafeGetB("Game", "Images", "clearlogo");
						if (url != null)
						{
							Gdbrelease.LogoUrl = BaseImageUrl + url;
						}
					}
				}
				else
				{
					Reporter.Report("Failure getting " + Gdbrelease.Title + ", ID " + Gdbrelease.Id + " from Games DB.");
				}
			}
		}

		public void CachePlatformGamesAsync(Platform platform)
		{
			Reporter.Report("GamesDB does not have games and releases--try caching releases");
		}

		/// <summary>
		/// Implements IDB.CachePlatforms(). Update the list of Gdbplatforms in the local cache.
		/// </summary>
		public void CachePlatforms()
		{
			// TODO Get list of platforms from gamesdb and cache in Gdbplatforms
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
			var GdbreleaseEntries = R.Data.ChangeTracker.Entries<Gdbrelease>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int GdbreleaseAddCount = GdbreleaseEntries.Count(x => x.State == Microsoft.EntityFrameworkCore.EntityState.Added);
			int GdbreleaseModCount = GdbreleaseEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("Gdbreleases added: " + GdbreleaseAddCount + ", Gdbreleases updated: " + GdbreleaseModCount);
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
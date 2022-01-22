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

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;

namespace Robin
{
	class GiantBomb : IDB
	{
		public string Title { get { return "GiantBomb"; } }

		public LocalDB DB { get { return LocalDB.GiantBomb; } }

		public IEnumerable<IDbPlatform> Platforms =>
			R.Data.GBPlatforms.Local.ToObservableCollection();

		public IEnumerable<IDbRelease> Releases =>
			R.Data.GBReleases.Local.ToObservableCollection();

		public bool HasRegions => true;

		bool disposed;

		const string GBURL = @"http://www.giantbomb.com/api/";
		const string DATEFORMAT = @"yyyy-MM-dd hh\:mm\:ss";

		public GiantBomb()
		{
			try
			{
				Reporter.Tic("Opening GiantBomb cache...", out int tic1);

				R.Data.GBPlatforms.Load();
				R.Data.GBReleases.Load();
				R.Data.GBGames.Load();
				Reporter.Toc(tic1);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, $"Problem Opening GiantBomb from RobinData", MessageBoxButton.OK);
			}
			catch (Microsoft.Data.Sqlite.SqliteException ex)
			{
				MessageBox.Show(ex.Message, "Sqlite Exception loading GiantBomb", MessageBoxButton.OK);
			}
		}

		public void CachePlatformReleases(Platform platform)
		{
			CachePlatformReleases(platform, false);
		}

		/// <summary>
		/// Implement IDB.CachePlatfomrReleases(). Go out to giantbomb.com and cache all known releases for the specified platform. Update the list of releases and store metadata for each one.
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the GBPlatform to cache.</param>
		public static void CachePlatformReleases(Platform platform, bool reset = false)
		{
			using WebClient webClient = new();
			GBPlatform GBPlatform = platform.GBPlatform;
			string startDate = reset ? @"1900-01-01 01:01:01" : GBPlatform.CacheDate.ToString(DATEFORMAT);
			string endDate = DateTime.Now.ToString(DATEFORMAT);

			int N_results;
			bool haveResults;
			XDocument xdoc;
			Reporter.Report($"Checking GiantBomb for {GBPlatform.Title} releases...");

			string url = $"http://www.giantbomb.com/api/releases/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate},platform:{GBPlatform.ID}&field_list=id&sort=id:asc";

			if (webClient.SafeDownloadStringDB(url, out string downloadText))
			{
				xdoc = XDocument.Parse(downloadText);
				haveResults = int.TryParse(xdoc.SafeGetB("number_of_total_results"), out N_results);
			}
			else
			{
				Reporter.Warn("Error communicating with GiantBomb.");
				return;
			}

			// If there are results, go through them
			if (haveResults && N_results != 0)
			{
				Dictionary<long, GBRelease> existingGBReleaseDict = R.Data.GBReleases.ToDictionary(x => x.ID);
				ILookup<long?, Release> releaseLookupByGB_ID = R.Data.Releases.Where(x => x.ID_GB != null).ToLookup(x => x.ID_GB);
				HashSet<GBRelease> newGBReleases = new();

				int N_pages = N_results / 100;
				Reporter.Report($"Found {N_results} {GBPlatform.Title} releases in GiantBomb");

				// Just do the first page again to save code then go through the rest of the results
				Reporter.Report("Loading sheet ");
				for (int i = 0; i <= N_pages; i++)
				{
					Reporter.ReportInline(" " + i);
					url = $"http://www.giantbomb.com/api/releases/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate}platform:{GBPlatform.ID}&offset={i * 100}&field_list=id,deck,game,image,region,name,maximum_players,release_date&sort=id:asc";

					// Put results into the GB Cache database
					if (webClient.SafeDownloadStringDB(url, out downloadText))
					{
						xdoc = XDocument.Parse(downloadText);

						foreach (XElement element in xdoc.Root.Element("results").Elements("release"))
						{
							// If the ID XML value was found
							if (int.TryParse(element.SafeGetA("id"), out int id))
							{
								// Don't create this game if the title is null
								string title = element.SafeGetA("name");
								if (string.IsNullOrEmpty(title))
								{
									continue;
								}

								// Check if GBRelease is in database prior to this update, else add it
								if (!existingGBReleaseDict.TryGetValue(id, out GBRelease GBRelease))
								{
									GBRelease = new GBRelease { ID = id, GBPlatform = platform.GBPlatform };
									newGBReleases.Add(GBRelease);
								}

								// If a release has changed platforms, catch it and zero out match
								if (GBRelease.GBPlatformId != GBPlatform.ID)
								{
									GBRelease.GBPlatformId = GBPlatform.ID;

									if (releaseLookupByGB_ID[GBRelease.ID].Any())
									{
										foreach (Release release in releaseLookupByGB_ID[GBRelease.ID])
										{
											release.ID_GB = null;
										}
									}
								}

								GBRelease.Title = title;
								GBRelease.Overview = element.SafeGetA("deck");
								if (int.TryParse(element.SafeGetA("game", "id"), out id))
								{
									GBRelease.GBGameId = id;
								}
								if (int.TryParse(element.SafeGetA("maximum_players"), out id))
								{
									GBRelease.Players = id.ToString();
								}

								GBRelease.GBPlatformId = GBPlatform.ID;
								if (int.TryParse(element.SafeGetA("region", "id"), out id))
								{
									GBRelease.Region = R.Data.Regions.FirstOrDefault(x => x.IdGb == id) ?? R.Data.Regions.FirstOrDefault(x => x.Title.Contains("Unk"));
								}

								GBRelease.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("release_date"));
								GBRelease.BoxUrl = element.SafeGetA("image", "medium_url");
								GBRelease.ScreenUrl = element.SafeGetA("image", "screen_url");
							}
						}
					}
					else
					{
						Reporter.Warn("Failure connecting to GiantBomb");
						return;
					}
				}
				R.Data.GBReleases.AddRange(newGBReleases);
				Reporter.ReportInline("Finished.");
			}
			else
			{
				Reporter.Report("No releases returned by GiantBomb");
				return;
			}
			// end using webclient
		}

		public void CachePlatformGamesAsync(Platform platform)
		{
			CachePlatformGames(platform, false);
		}

		public static void CachePlatformGames(Platform platform, bool reset = false)
		{
			GBPlatform GBPlatform = platform.GBPlatform;
			string startDate = reset ? @"1900-01-01 01:01:01" : GBPlatform.CacheDate.ToString(DATEFORMAT);
			string endDate = DateTime.Now.ToString(DATEFORMAT);

			using WebClient webClient = new();
			int N_results;
			bool haveResults;
			XDocument xdoc;
			Reporter.Report($"Checking GiantBomb for {platform.Title} games");

			var url = $"http://www.giantbomb.com/api/games/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate},platforms:{GBPlatform.ID}&field_list=id&sort=id:asc";


			if (webClient.SafeDownloadStringDB(url, out var downloadText))
			{
				xdoc = XDocument.Parse(downloadText);
				haveResults = int.TryParse(xdoc.SafeGetB("number_of_total_results"), out N_results);
			}
			else
			{
				Reporter.Report("Error communicating with GiantBomb.");
				return;
			}

			// If there are results, go through them
			if (haveResults && N_results != 0)
			{
				int N_pages = N_results / 100;
				Reporter.Report("Found " + N_results + " games in GiantBomb");

				Reporter.Report("Loading sheet ");
				// Just do the first page again to save code then go through the rest of the results
				for (int i = 0; i <= N_pages; i++)
				{
					Reporter.ReportInline(i + " ");

					url = $"http://www.giantbomb.com/api/games/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate},platforms:{GBPlatform.ID}&offset={i * 100}&field_list=id,deck,developers,publishers,genres,image,name,release_date&sort=id:asc";

					// Put results into the GB Cache database
					if (webClient.SafeDownloadStringDB(url, out downloadText))
					{
						xdoc = XDocument.Parse(downloadText);
						foreach (XElement element in xdoc.Root.Element("results").Elements("game"))
						{
							// If the ID XML value was found
							if (int.TryParse(element.SafeGetA("id"), out var intCatcher))
							{
								GBGame GBGame = R.Data.GBGames.FirstOrDefault(x => x.ID == intCatcher);

								if (GBGame == null)
								{
									GBGame = new GBGame { ID = intCatcher };
									GBPlatform.GBGames.Add(GBGame);
								}

								GBGame.Title = element.SafeGetA("name");
								GBGame.Overview = element.SafeGetA("deck");
								GBGame.GBPlatformId = GBPlatform.ID;
								GBGame.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("release_date"));
								GBGame.BoxFrontUrl = element.SafeGetA("image", "medium_url");
								GBGame.ScreenUrl = element.SafeGetA("image", "screen_url");
							}
						}
					}
					else
					{
						Reporter.Warn("Failure connecting to GiantBomb");
						return;
					}
				}

				Reporter.Report("Finished.");
			}
			else
			{
				Reporter.Report("No results returned by GiantBomb");
				return;
			}
			// end using webclient
		}

		/// <summary>
		/// Implements IDB.CachePlatformdata() Update the local DB cache of platform associated metadata
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the DBPlatorm to update.</param>
		public void CachePlatformData(Platform platform)
		{
			GBPlatform GBPlatform = platform.GBPlatform;

			XDocument xDocument;

			Reporter.Tic("Attempting to cache " + GBPlatform.Title + "...", out int tic1);

			using WebClient webClient = new();

			string url = GBURL + $"platform/{GBPlatform.ID}/?api_key ={Keys.GiantBomb}";

			if (webClient.SafeDownloadStringDB(url, out string downloadText))
			{
				xDocument = XDocument.Parse(downloadText);

				GBPlatform.Title = xDocument.SafeGetB("results", "name");
				GBPlatform.Abbreviation = xDocument.SafeGetB("results", "abbreviation");
				if (long.TryParse(xDocument.SafeGetB("results", "company"), out long company))
				{
					GBPlatform.Company = company;
				}
				GBPlatform.Deck = xDocument.SafeGetB("results", "deck");
				GBPlatform.Price = xDocument.SafeGetB("results", "original_price");
				GBPlatform.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("results", "release_date"));
				Reporter.Toc(tic1);
			}

			else
			{
				Reporter.Toc(tic1, "Error communicating with GiantBomb.");
			}
			// end using webclient
		}

		public void CachePlatforms()
		{
			// TODO Get list of platforms from giantbom and cache in GBPlatforms
			Reporter.Report("Cache platforms not yet implemented for GiantBomb");
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

			var GBReleaseEntries = R.Data.ChangeTracker.Entries<GBRelease>();
			var GBGameEntries = R.Data.ChangeTracker.Entries<GBGame>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int GBReleaseAddCount = GBReleaseEntries.Count(x => x.State == EntityState.Added);
			int GBReleaseModCount = GBReleaseEntries.Count(x => x.State == EntityState.Modified);

			int GBGameAddCount = GBGameEntries.Count(x => x.State == EntityState.Added);
			int GBGameModCount = GBGameEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("GBReleases added: " + GBReleaseAddCount + ", GBReleases updated: " + GBReleaseModCount);
			Reporter.Report("GBGames added: " + GBGameAddCount + ", GBGames updated: " + GBGameModCount);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GiantBomb()
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
				/*                R.Data.GBPlatforms.Dispose()*/

			}

			// release any unmanaged objects
			// set the object references to null

			//R.Data = null;

			disposed = true;
		}
	}
}




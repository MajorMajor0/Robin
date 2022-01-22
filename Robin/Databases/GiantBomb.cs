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
			R.Data.Gbplatforms.Local.ToObservableCollection();

		public IEnumerable<IDbRelease> Releases =>
			R.Data.Gbreleases.Local.ToObservableCollection();

		public bool HasRegions => true;

		bool disposed;

		const string GBURL = @"http://www.giantbomb.com/api/";
		const string DATEFORMAT = @"yyyy-MM-dd hh\:mm\:ss";

		public GiantBomb()
		{
			try
			{
				Reporter.Tic("Opening GiantBomb cache...", out int tic1);

				R.Data.Gbplatforms.Load();
				R.Data.Gbreleases.Load();
				R.Data.Gbgames.Load();
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
		/// <param name="platform">Robin.Platform associated with the Gbplatform to cache.</param>
		public static void CachePlatformReleases(Platform platform, bool reset = false)
		{
			using (WebClient webClient = new WebClient())
			{
				Gbplatform Gbplatform = platform.Gbplatform;
				string startDate = reset ? @"1900-01-01 01:01:01" : Gbplatform.CacheDate.ToString(DATEFORMAT);
				string endDate = DateTime.Now.ToString(DATEFORMAT);

				int N_results;
				bool haveResults;
				XDocument xdoc;
				Reporter.Report($"Checking GiantBomb for {Gbplatform.Title} releases...");

				string url = $"http://www.giantbomb.com/api/releases/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate},platform:{Gbplatform.Id}&field_list=id&sort=id:asc";

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
					Dictionary<long, Gbrelease> existingGbreleaseDict = R.Data.Gbreleases.ToDictionary(x => x.Id);
					ILookup<long?, Release> releaseLookupByGB_ID = R.Data.Releases.Where(x => x.ID_GB != null).ToLookup(x => x.ID_GB);
					HashSet<Gbrelease> newGbreleases = new HashSet<Gbrelease>();

					int N_pages = N_results / 100;
					Reporter.Report($"Found {N_results} {Gbplatform.Title} releases in GiantBomb");

					// Just do the first page again to save code then go through the rest of the results
					Reporter.Report("Loading sheet ");
					for (int i = 0; i <= N_pages; i++)
					{
						Reporter.ReportInline(" " + i);
						url = $"http://www.giantbomb.com/api/releases/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate}platform:{Gbplatform.Id}&offset={i * 100}&field_list=id,deck,game,image,region,name,maximum_players,release_date&sort=id:asc";

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

									// Check if Gbrelease is in database prior to this update, else add it
									if (!existingGbreleaseDict.TryGetValue(id, out Gbrelease Gbrelease))
									{
										Gbrelease = new Gbrelease { Id = id, Gbplatform = platform.Gbplatform };
										newGbreleases.Add(Gbrelease);
									}

									// If a release has changed platforms, catch it and zero out match
									if (Gbrelease.GbplatformId != Gbplatform.Id)
									{
										Gbrelease.GbplatformId = Gbplatform.Id;

										if (releaseLookupByGB_ID[Gbrelease.Id].Any())
										{
											foreach (Release release in releaseLookupByGB_ID[Gbrelease.Id])
											{
												release.ID_GB = null;
											}
										}
									}

									Gbrelease.Title = title;
									Gbrelease.Overview = element.SafeGetA("deck");
									if (int.TryParse(element.SafeGetA("game", "id"), out id))
									{
										Gbrelease.GbgameId = id;
									}
									if (int.TryParse(element.SafeGetA("maximum_players"), out id))
									{
										Gbrelease.Players = id.ToString();
									}

									Gbrelease.GbplatformId = Gbplatform.Id;
									if (int.TryParse(element.SafeGetA("region", "id"), out id))
									{
										Gbrelease.Region = R.Data.Regions.FirstOrDefault(x => x.IdGb == id) ?? R.Data.Regions.FirstOrDefault(x => x.Title.Contains("Unk"));
									}

									Gbrelease.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("release_date"));
									Gbrelease.BoxUrl = element.SafeGetA("image", "medium_url");
									Gbrelease.ScreenUrl = element.SafeGetA("image", "screen_url");
								}
							}
						}
						else
						{
							Reporter.Warn("Failure connecting to GiantBomb");
							return;
						}
					}
					R.Data.Gbreleases.AddRange(newGbreleases);
					Reporter.ReportInline("Finished.");
				}
				else
				{
					Reporter.Report("No releases returned by GiantBomb");
					return;
				}
			}// end using webclient
		}

		public void CachePlatformGamesAsync(Platform platform)
		{
			CachePlatformGames(platform, false);
		}

		public static void CachePlatformGames(Platform platform, bool reset = false)
		{
			Gbplatform Gbplatform = platform.Gbplatform;
			string startDate = reset ? @"1900-01-01 01:01:01" : Gbplatform.CacheDate.ToString(DATEFORMAT);
			string endDate = DateTime.Now.ToString(DATEFORMAT);

			using (WebClient webClient = new WebClient())
			{
				int N_results;
				bool haveResults;
				XDocument xdoc;
				Reporter.Report($"Checking GiantBomb for {platform.Title} games");

				var url = $"http://www.giantbomb.com/api/games/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate},platforms:{Gbplatform.Id}&field_list=id&sort=id:asc";


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

						url = $"http://www.giantbomb.com/api/games/?api_key={Keys.GiantBomb}&filter=date_last_updated:{startDate}|{endDate},platforms:{Gbplatform.Id}&offset={i * 100}&field_list=id,deck,developers,publishers,genres,image,name,release_date&sort=id:asc";

						// Put results into the GB Cache database
						if (webClient.SafeDownloadStringDB(url, out downloadText))
						{
							xdoc = XDocument.Parse(downloadText);
							foreach (XElement element in xdoc.Root.Element("results").Elements("game"))
							{
								// If the ID XML value was found
								if (int.TryParse(element.SafeGetA("id"), out var intCatcher))
								{
									Gbgame Gbgame = R.Data.Gbgames.FirstOrDefault(x => x.Id == intCatcher);

									if (Gbgame == null)
									{
										Gbgame = new Gbgame { Id = intCatcher };
										Gbplatform.Gbgames.Add(Gbgame);
									}

									Gbgame.Title = element.SafeGetA("name");
									Gbgame.Overview = element.SafeGetA("deck");
									Gbgame.GbplatformId = Gbplatform.Id;
									Gbgame.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("release_date"));
									Gbgame.BoxFrontUrl = element.SafeGetA("image", "medium_url");
									Gbgame.ScreenUrl = element.SafeGetA("image", "screen_url");
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
			}// end using webclient
		}

		/// <summary>
		/// Implements IDB.CachePlatformdata() Update the local DB cache of platform associated metadata
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the DBPlatorm to update.</param>
		public void CachePlatformData(Platform platform)
		{
			Gbplatform Gbplatform = platform.Gbplatform;

			XDocument xDocument;

			Reporter.Tic("Attempting to cache " + Gbplatform.Title + "...", out int tic1);

			using WebClient webClient = new();

			string url = GBURL + $"platform/{Gbplatform.Id}/?api_key ={Keys.GiantBomb}";

			if (webClient.SafeDownloadStringDB(url, out string downloadText))
			{
				xDocument = XDocument.Parse(downloadText);

				Gbplatform.Title = xDocument.SafeGetB("results", "name");
				Gbplatform.Abbreviation = xDocument.SafeGetB("results", "abbreviation");
				if (long.TryParse(xDocument.SafeGetB("results", "company"), out long company))
				{
					Gbplatform.Company = company;
				}
				Gbplatform.Deck = xDocument.SafeGetB("results", "deck");
				Gbplatform.Price = xDocument.SafeGetB("results", "original_price");
				Gbplatform.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("results", "release_date"));
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
			// TODO Get list of platforms from giantbom and cache in Gbplatforms
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

			var GbreleaseEntries = R.Data.ChangeTracker.Entries<Gbrelease>();
			var GbgameEntries = R.Data.ChangeTracker.Entries<Gbgame>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int GbreleaseAddCount = GbreleaseEntries.Count(x => x.State == EntityState.Added);
			int GbreleaseModCount = GbreleaseEntries.Count(x => x.State == EntityState.Modified);

			int GbgameAddCount = GbgameEntries.Count(x => x.State == EntityState.Added);
			int GbgameModCount = GbgameEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("Gbreleases added: " + GbreleaseAddCount + ", Gbreleases updated: " + GbreleaseModCount);
			Reporter.Report("Gbgames added: " + GbgameAddCount + ", Gbgames updated: " + GbgameModCount);
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
				/*                R.Data.Gbplatforms.Dispose()*/

			}

			// release any unmanaged objects
			// set the object references to null

			//R.Data = null;

			disposed = true;
		}
	}
}




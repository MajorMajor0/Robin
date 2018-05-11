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
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Robin
{
	class GiantBomb : IDB
	{
		public string Title { get { return "GiantBomb"; } }

		public LocalDB DB { get { return LocalDB.GiantBomb; } }

		public DbSet Platforms => R.Data.GBPlatforms;

		public DbSet Releases => R.Data.GBReleases;

		public bool HasRegions => true;

		bool disposed;

		const string GBAPIKEY = @"?api_key=561e43b5ca29977bb624d52357764e459e22d3bd";
		const string GBURL = @"http://www.giantbomb.com/api/";
		const string DATEFORMAT = @"yyyy-MM-dd hh\:mm\:ss";

		public GiantBomb()
		{
			Reporter.Tic("Opening GiantBomb cache...", out int tic1);

			R.Data.GBPlatforms.Load();
			R.Data.GBReleases.Load();
			R.Data.GBGames.Load();
			Reporter.Toc(tic1);
		}

		public void CachePlatformReleases(Platform platform)
		{
			CachePlatformReleases(platform, false);
		}

		public void CachePlatformReleases(Platform platform, bool reset = false)
		{
			GBPlatform gbPlatform = platform.GBPlatform;
			string startDate = reset ? @"1900-01-01 01:01:01" : gbPlatform.CacheDate.ToString(DATEFORMAT);
			string endDate = DateTime.Now.ToString(DATEFORMAT);

			int N_results;
			bool haveResults;
			string url;
			StringBuilder urlBuilder;
			XDocument xdoc;

			using (WebClient webClient = new WebClient())
			{
				Reporter.Report("Checking GiantBomb for " + gbPlatform.Title + " releases...");
				urlBuilder = new StringBuilder(@"http://www.giantbomb.com/api/releases/", 300);
				urlBuilder.Append(GBAPIKEY).Append(@"&filter=date_last_updated:").Append(startDate).Append("|").Append(endDate).Append(@",platform:").Append(gbPlatform.ID).Append(@"&field_list=id&sort=id:asc");
				url = urlBuilder.ToString();

				//url = @"http://www.giantbomb.com/api/releases/" + GBAPIKEY + @"&filter=date_last_updated:" + startDate + "|" + endDate + @",platform:" + platform.ID_GB + @"&field_list=id&sort=id:asc";

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
					int N_pages = N_results / 100;
					Reporter.Report("Found " + N_results + " " + gbPlatform.Title + " releases in GiantBomb");

					// Just do the first page again to save code then go through the rest of the results
					Reporter.Report("Loading sheet ");
					for (int i = 0; i <= N_pages; i++)
					{
						Reporter.ReportInline(" " + i);

						urlBuilder = new StringBuilder(@"http://www.giantbomb.com/api/releases/", 300);
						urlBuilder.Append(GBAPIKEY).Append(@"&filter=date_last_updated:").Append(startDate).Append("|").Append(endDate).Append(@",platform:").Append(gbPlatform.ID).Append(@"&offset=").Append(i * 100).Append(@"&field_list=id,deck,game,image,region,name,maximum_players,release_date&sort=id:asc");
						url = urlBuilder.ToString();

						// Put results into the GB Cache database
						if (webClient.SafeDownloadStringDB(url, out downloadText))
						{
							xdoc = XDocument.Parse(downloadText);

							foreach (XElement element in xdoc.Root.Element("results").Elements("release"))
							{
								// If the ID XML value was found
								if (int.TryParse(element.SafeGetA("id"), out int intCatcher))
								{
									GBRelease gbRelease = R.Data.GBReleases.FirstOrDefault(x => x.ID == intCatcher);

									string title = element.SafeGetA("name");

									// Don't create this game if the title is null
									if (string.IsNullOrEmpty(title))
									{
										continue;
									}

									if (gbRelease == null)
									{
										gbRelease = new GBRelease();
										R.Data.GBReleases.Add(gbRelease);
										gbRelease.ID = intCatcher;
									}

									// If a release has changed platforms, catch it and zero out match
									if (gbRelease.GBPlatform_ID != gbPlatform.ID)
									{
										gbRelease.GBPlatform_ID = gbPlatform.ID;
										Release release = R.Data.Releases.FirstOrDefault(x => x.ID_GB == gbRelease.ID);
										if (release != null)
										{
											release.ID_GB = null;
										}
									}

									gbRelease.Title = title;

									gbRelease.Overview = element.SafeGetA("deck");

									if (int.TryParse(element.SafeGetA("game", "id"), out intCatcher))
									{
										gbRelease.GBGame_ID = intCatcher;
									}
									if (int.TryParse(element.SafeGetA("maximum_players"), out intCatcher))
									{
										gbRelease.Players = intCatcher.ToString();
									}

									gbRelease.GBPlatform_ID = gbPlatform.ID;
									if (int.TryParse(element.SafeGetA("region", "id"), out intCatcher))
									{
										gbRelease.Region = R.Data.Regions.FirstOrDefault(x => x.ID_GB == intCatcher) ?? R.Data.Regions.FirstOrDefault(x => x.Title.Contains("Unk"));
									}

									gbRelease.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("release_date"));
									gbRelease.BoxURL = element.SafeGetA("image", "medium_url");
									gbRelease.ScreenURL = element.SafeGetA("image", "screen_url");
								}
							}
						}
						else
						{
							Reporter.Warn("Failure connecting to GiantBomb");
							return;
						}
					}

					Reporter.ReportInline("Finished.");
				}
				else
				{
					Reporter.Report("No releases returned by GiantBomb");
					return;
				}
			}// end using webclient
		}

		public void CachePlatformGames(Platform platform)
		{
			CachePlatformGames(platform, false);
		}

		public void CachePlatformGames(Platform platform, bool reset = false)
		{
			GBPlatform gbPlatform = platform.GBPlatform;
			string startDate = reset ? @"1900-01-01 01:01:01" : gbPlatform.CacheDate.ToString(DATEFORMAT);
			string endDate = DateTime.Now.ToString(DATEFORMAT);

			using (WebClient webClient = new WebClient())
			{
				int N_results;
				bool haveResults;
				XDocument xdoc;
				Reporter.Report("Checking GiantBomb for " + platform.Title + " games");

				var url = @"http://www.giantbomb.com/api/games/" + GBAPIKEY + @"&filter=date_last_updated:" + startDate + "|" + endDate + @",platforms:" + gbPlatform.ID + @"&field_list=id&sort=id:asc";


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

						url = @"http://www.giantbomb.com/api/games/" + GBAPIKEY + @"&filter=date_last_updated:" + startDate + "|" + endDate + @",platforms:" + gbPlatform.ID + @"&offset=" + i * 100 + @"&field_list=id,deck,developers,publishers,genres,image,name,release_date&sort=id:asc";

						// Put results into the GB Cache database
						if (webClient.SafeDownloadStringDB(url, out downloadText))
						{
							xdoc = XDocument.Parse(downloadText);
							foreach (XElement element in xdoc.Root.Element("results").Elements("game"))
							{
								// If the ID XML value was found
								if (int.TryParse(element.SafeGetA("id"), out var intCatcher))
								{
									GBGame gbGame = R.Data.GBGames.FirstOrDefault(x => x.ID == intCatcher);

									if (gbGame == null)
									{
										gbGame = new GBGame { ID = intCatcher };
										gbPlatform.GBGames.Add(gbGame);
									}

									gbGame.Title = element.SafeGetA("name");
									gbGame.Overview = element.SafeGetA("deck");
									gbGame.GBPlatform_ID = gbPlatform.ID;
									gbGame.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("release_date"));
									gbGame.BoxFrontURL = element.SafeGetA("image", "medium_url");
									gbGame.ScreenURL = element.SafeGetA("image", "screen_url");
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

		public void CachePlatformData(Platform platform)
		{
			GBPlatform gbPlatform = platform.GBPlatform;
			//DateTime startdate = gbPlatform.CacheDate;

			string url;
			XDocument xDocument;

			Reporter.Tic("Attempting to cache " + gbPlatform.Title + "...", out int tic1);

			using (WebClient webClient = new WebClient())
			{
				url = GBURL + @"platform/" + gbPlatform.ID + "/" + GBAPIKEY;

				if (webClient.SafeDownloadStringDB(url, out string downloadText))
				{
					xDocument = XDocument.Parse(downloadText);

					gbPlatform.Title = xDocument.SafeGetB("results", "name");
					gbPlatform.Abbreviation = xDocument.SafeGetB("results", "abbreviation");
					if (long.TryParse(xDocument.SafeGetB("results", "company"), out long company))
					{
						gbPlatform.Company = company;
					}
					gbPlatform.Deck = xDocument.SafeGetB("results", "deck");
					gbPlatform.Price = xDocument.SafeGetB("results", "original_price");
					gbPlatform.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("results", "release_date"));
					Reporter.Toc(tic1);
				}

				else
				{
					Reporter.Toc(tic1, "Error communicating with GiantBomb.");
				}

			}// end using webclient
		}

		public void CachePlatforms()
		{
			// TODO Get list of platforms from giantbom and cache in GBPlatforms
			Reporter.Report("Cache platforms not yet implemented for GiantBomb");
		}

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

			var gbReleaseEntries = R.Data.ChangeTracker.Entries<GBRelease>();
			var gbGameEntries = R.Data.ChangeTracker.Entries<GBGame>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int gbReleaseAddCount = gbReleaseEntries.Count(x => x.State == EntityState.Added);
			int gbReleaseModCount = gbReleaseEntries.Count(x => x.State == EntityState.Modified);

			int gbGameAddCount = gbGameEntries.Count(x => x.State == EntityState.Added);
			int gbGameModCount = gbGameEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("GBReleases added: " + gbReleaseAddCount + ", GBReleases updated: " + gbReleaseModCount);
			Reporter.Report("GBGames added: " + gbGameAddCount + ", GBGames updated: " + gbGameModCount);
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




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

        bool disposed;

        const string GBAPIKEY = @"?api_key=561e43b5ca29977bb624d52357764e459e22d3bd";
        const string GBURL = @"http://www.giantbomb.com/api/";
        const string DATEFORMAT = @"yyyy-MM-dd hh\:mm\:ss";

        public GiantBomb()
        {
            Reporter.Tic("Opening GiantBomb cache...");

            R.Data.GBPlatforms.Load();
            R.Data.GBReleases.Load();
            R.Data.GBGames.Load();
            Reporter.Toc();
        }

        public void CachePlatformReleases(Platform platform)
        {
            CachePlatformReleases(platform, false);
        }

        public void CachePlatformReleases(Platform platform, bool reset = false)
        {
            GBPlatform gbPlatform = R.Data.GBPlatforms.FirstOrDefault(x => x.ID == platform.ID_GB);
            string startDate = reset ? @"1900-01-01 01:01:01" : gbPlatform.CacheDate.ToString(DATEFORMAT);
            string endDate = DateTime.Now.ToString(DATEFORMAT);

            int N_results = 0;
            int N_added = 0;
            bool haveResults = false;
            int intCatcher = 0;
            string downloadText;
            string url;
            StringBuilder urlBuilder;
            XDocument xdoc = new XDocument();

            Stopwatch Watch = new Stopwatch();
            Watch.Start();

            using (WebClient webClient = new WebClient())
            {
                Reporter.Report("Checking GiantBomb for releases...");
                urlBuilder = new StringBuilder(@"http://www.giantbomb.com/api/releases/", 300);
                urlBuilder.Append(GBAPIKEY).Append(@"&filter=date_last_updated:").Append(startDate).Append("|").Append(endDate).Append(@",platform:").Append(platform.ID_GB).Append(@"&field_list=id&sort=id:asc");
                url = urlBuilder.ToString();

                //url = @"http://www.giantbomb.com/api/releases/" + GBAPIKEY + @"&filter=date_last_updated:" + startDate + "|" + endDate + @",platform:" + platform.ID_GB + @"&field_list=id&sort=id:asc";

                if (webClient.SafeDownloadStringDB(url, out downloadText))
                {
                    Watch.Restart();
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
                    Reporter.Report("Found " + N_results + " new releases in GiantBomb");

                    // Just do the first page again to save code then go through the rest of the results
                    Reporter.Report("Loading sheet ");
                    for (int i = 0; i <= N_pages; i++)
                    {
                        Reporter.ReportInline(" " + i);

                        // Need a non-generic user-agent for giantbomb API
                        webClient.SetStandardHeaders();

                        urlBuilder = new StringBuilder(@"http://www.giantbomb.com/api/releases/", 300);
                        urlBuilder.Append(GBAPIKEY).Append(@"&filter=date_last_updated:").Append(startDate).Append("|").Append(endDate).Append(@",platform:").Append(platform.ID_GB).Append(@"&offset=").Append(i * 100).Append(@"&field_list=id,deck,game,image,region,name,maximum_players,release_date&sort=id:asc");
                        //url = @"http://www.giantbomb.com/api/releases/" + GBAPIKEY + @"&filter=date_last_updated:" + startDate + "|" + endDate + @",platform:" + platform.ID_GB + @"&offset=" + i * 100 + @"&field_list=id,deck,game,image,region,name,maximum_players,release_date&sort=id:asc";
                        url = urlBuilder.ToString();

                        // Put results into the GB Cache database
                        if (webClient.SafeDownloadStringDB(url, out downloadText))
                        {
                            xdoc = XDocument.Parse(downloadText);
                            Watch.Restart();

                            foreach (XElement element in xdoc.Root.Element("results").Elements("release"))
                            {
                                // If the ID XML value was found
                                if (int.TryParse(element.SafeGetA("id"), out intCatcher))
                                {
                                    GBRelease gbRelease = R.Data.GBReleases.FirstOrDefault(x => x.ID == intCatcher);

                                    if (gbRelease == null)
                                    {
                                        gbRelease = new GBRelease();
                                        R.Data.GBReleases.Add(gbRelease);
                                        N_added++;
                                    }

                                    gbRelease.ID = intCatcher;
                                    gbRelease.Title = element.SafeGetA("name");
                                    gbRelease.Overview = element.SafeGetA("deck");

                                    if (int.TryParse(element.SafeGetA("game", "id"), out intCatcher))
                                    {
                                        gbRelease.GBGame_ID = intCatcher;
                                    }
                                    if (int.TryParse(element.SafeGetA("maximum_players"), out intCatcher))
                                    {
                                        gbRelease.Players = intCatcher.ToString();
                                    }

                                    gbRelease.GBPlatform_ID = (long)platform.ID_GB;
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

            R.Data.ChangeTracker.DetectChanges();

            int N_total = R.Data.Save();
            Reporter.Report(N_added + " releases added to database, " + (N_total - N_added) + " releases updated.");

        }

        public void CachePlatformGames(Platform platform)
        {
            CachePlatformGames(platform, false);
        }

        public void CachePlatformGames(Platform platform, bool reset = false)
        {
            R.Data.Configuration.AutoDetectChangesEnabled = false;

            GBPlatform gbPlatform = R.Data.GBPlatforms.FirstOrDefault(x => x.ID == platform.ID_GB);
            string startDate = reset ? @"1900-01-01 01:01:01" : gbPlatform.CacheDate.ToString(DATEFORMAT);
            string endDate = DateTime.Now.ToString(DATEFORMAT);

            int N_results = 0;
            int N_added = 0;
            bool haveResults = false;
            int intCatcher = 0;
            string downloadText;
            string url;
            XDocument xdoc = new XDocument();

            Stopwatch Watch = new Stopwatch();
            Watch.Start();

            using (WebClient webClient = new WebClient())
            {
                Reporter.Report("Checking GiantBomb for games");

                url = @"http://www.giantbomb.com/api/games/" + GBAPIKEY + @"&filter=date_last_updated:" + startDate + "|" + endDate + @",platforms:" + platform.ID_GB + @"&field_list=id&sort=id:asc";


                if (webClient.SafeDownloadStringDB(url, out downloadText))
                {
                    xdoc = XDocument.Parse(downloadText);
                    haveResults = int.TryParse(xdoc.SafeGetB("number_of_total_results"), out N_results);
                }
                else
                {
                    Reporter.Report("Error communicating with GiantBomb.");
                    return;
                }
                Watch.Restart();

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

                        url = @"http://www.giantbomb.com/api/games/" + GBAPIKEY + @"&filter=date_last_updated:" + startDate + "|" + endDate + @",platforms:" + platform.ID_GB + @"&offset=" + i * 100 + @"&field_list=id,deck,developers,publishers,genres,image,name,release_date&sort=id:asc";

                        // Put results into the GB Cache database
                        if (webClient.SafeDownloadStringDB(url, out downloadText))
                        {
                            xdoc = XDocument.Parse(downloadText);
                            Watch.Restart();
                            bool gameAlreadyExisted;
                            foreach (XElement element in xdoc.Root.Element("results").Elements("game"))
                            {
                                Watch.Restart();
                                // If the ID XML value was found
                                if (int.TryParse(element.SafeGetA("id"), out intCatcher))
                                {
                                    GBGame gbGame = new GBGame();
                                    if (R.Data.GBGames.Any(x => x.ID == intCatcher))
                                    {
                                        gbGame = R.Data.GBGames.FirstOrDefault(x => x.ID == intCatcher);
                                        gameAlreadyExisted = true;
                                    }
                                    else
                                    {
                                        gbGame = new GBGame();
                                        gameAlreadyExisted = false;
                                    }

                                    gbGame.ID = intCatcher;

                                    gbGame.Title = element.SafeGetA("name");
                                    gbGame.Overview = element.SafeGetA("deck");
                                    gbGame.GBPlatform_ID = platform.ID_GB;
                                    gbGame.Date = DateTimeRoutines.SafeGetDate(element.SafeGetA("release_date"));
                                    gbGame.BoxURL = element.SafeGetA("image", "medium_url");
                                    gbGame.ScreenURL = element.SafeGetA("image", "screen_url");

                                    if (!gameAlreadyExisted)
                                    {
                                        R.Data.GBGames.Add(gbGame);
                                        N_added++;
                                    }

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

            R.Data.ChangeTracker.DetectChanges();
            R.Data.Configuration.AutoDetectChangesEnabled = true;

            int N_total = R.Data.Save();
            Reporter.Report(N_added + " games added to Games DB cache, " + (N_total - N_added) + " games updated.");
        }

        public void CachePlatformData(Platform platform)
        {
            GBPlatform gbPlatform = R.Data.GBPlatforms.FirstOrDefault(x => x.ID == platform.ID_GB);
            DateTime startdate = gbPlatform.CacheDate;

            string downloadText;
            string url;
            XDocument xDocument = new XDocument();

            Reporter.Tic("Attempting to cache " + gbPlatform.Title + "...");

            using (WebClient webClient = new WebClient())
            {
                url = GBURL + @"platform/" + platform.ID_GB + "/" + GBAPIKEY;

                if (webClient.SafeDownloadStringDB(url, out downloadText))
                {
                    long company;
                    xDocument = XDocument.Parse(downloadText);

                    gbPlatform.Title = xDocument.SafeGetB("results", "name");
                    gbPlatform.Abbreviation = xDocument.SafeGetB("results", "abbreviation");
                    if (long.TryParse(xDocument.SafeGetB("results", "company"), out company))
                    {
                        gbPlatform.Company = company;
                    }
                    gbPlatform.Deck = xDocument.SafeGetB("results", "deck");
                    gbPlatform.Price = xDocument.SafeGetB("results", "original_price");
                    gbPlatform.Date = DateTimeRoutines.SafeGetDate(xDocument.SafeGetB("results", "release_date"));
                    Reporter.Toc();
                }

                else
                {
                    Reporter.Toc("Error communicating with GiantBomb.");
                }

            }// end using webclient

            int n = R.Data.Save();
            Reporter.Report(n + " changes pushed to database.");

        }

        public void CachePlatform(Platform platform)
        {
            GBPlatform gbPlatform = R.Data.GBPlatforms.FirstOrDefault(x => x.ID == platform.ID_GB);
            if (gbPlatform != null)
            {
                Reporter.Report("Caching " + platform.Title);
                CachePlatformReleases(platform, true);
                CachePlatformGames(platform, true);
                CachePlatformData(platform);
                gbPlatform.CacheDate = DateTime.Now;
            }
            else
            {
                Reporter.Report("Platform does not exist in GiantBomb");
            }

        }

        public void CachePlatforms()
        {
            // TODO Get list of platforms from giantbom and cache in GBPlatforms
            Reporter.Report("Cache platfomrs not yet implemented for GiantBomb");
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
/*                R.Data.GBPlatforms.Dispose()*/;
            }

            // release any unmanaged objects
            // set the object references to null

            //R.Data = null;

            disposed = true;
        }
    }
}




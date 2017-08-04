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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Robin
{
    public class Datomatic : IDB
    {
        public string Title { get { return "Datomatic (Robin)"; } }

        public LocalDB DB { get { return LocalDB.Datomatic; } }

        public DbSet Platforms => R.Data.Platforms;

        public DbSet Releases => R.Data.Releases;

        public Datomatic()
        {
            R.Data.Languages.Load();
        }

        public bool HasRegions => true;

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

            var ReleaseEntries = R.Data.ChangeTracker.Entries<Release>();

            Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);

            int ReleaseAddCount = ReleaseEntries.Count(x => x.State == EntityState.Added);

            int ReleaseModCount = ReleaseEntries.Count(x => x.State == EntityState.Modified);

            Reporter.Report("Releases added: " + ReleaseAddCount + ", Releases updated: " + ReleaseModCount);
        }

        public void CachePlatforms()
        {
            throw new NotImplementedException();
        }

        public void CachePlatformGames(IDBPlatform idbPlatform)
        {
            throw new NotImplementedException();
        }

        public void CachePlatformData(IDBPlatform idbPlatform)
        {
            // TODO make this copy data from DBs to Platforms
            throw new NotImplementedException();
        }

        bool disposed;

        const string UNL = @".*\(Unl\).*";

        public void CachePlatformReleases(IDBPlatform _platform)
        {
            Platform platform = R.Data.Platforms.Where(x => x.ID == _platform.ID).FirstOrDefault();

            //TODO: customize dialog window to explain what is going on here--i.e., that you have to find a datomatic file
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog Dialog = new Microsoft.Win32.OpenFileDialog();
            Dialog.FileName = "Document"; // Default file name
            Dialog.DefaultExt = ".dat"; // Default file extension
            Dialog.Filter = "DAT files (.xml; .txt; .dat)|*.xml;*.txt;*.dat|ZIP files (.zip)|*.zip|All files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            bool? result = Dialog.ShowDialog() ?? false;

            // Process XML file
            if (result == true && File.Exists(Dialog.FileName))
            {
                Stopwatch Watch1 = Stopwatch.StartNew();
#if DEBUG
                Stopwatch Watch2 = Stopwatch.StartNew();
#endif
                Stopwatch Watch3 = Stopwatch.StartNew();

                XDocument DatomaticFile = new XDocument();

                if (Path.GetExtension(Dialog.FileName) == ".zip")
                {
                    using (ZipArchive archive = ZipFile.Open(Dialog.FileName, ZipArchiveMode.Read))
                    {
                        string zipentryname = Path.GetFileNameWithoutExtension(Dialog.FileName) + ".dat";
                        using (var dattext = archive.GetEntry(zipentryname).Open())
                        {
                            DatomaticFile = XDocument.Load(dattext);
                        }
                    }
                }

                else
                {
                    DatomaticFile = XDocument.Load(Dialog.FileName);
                }

                List<Region> datomaticRegions = R.Data.Regions.Where(x => x.Datomatic != null).ToList();
                
                // Add release where required to make sure xelements have standardized info
                foreach (XElement gameElement in DatomaticFile.Root.Elements("game"))
                {
                    if (gameElement.Descendants("release").Count() == 0)
                    {
                        string elementName = gameElement.SafeGetA(attribute: "name") ?? "Unk";
                        string regionName = "UNK";

                        // Get the region using regex from parenthesis
                        foreach (Region region in datomaticRegions)
                        {
                            if (Regex.IsMatch(elementName, @"\([^)]*" + region.Datomatic + @"[^)]*\)"))
                            {
                                regionName = region.Datomatic;
                                break;
                            }
                        }
                        if (regionName == "UNK")
                        {
                            foreach (Region region in datomaticRegions)
                            {
                                if (Regex.IsMatch(elementName, @"\([^)]*" + region.Title + @"[^)]*\)"))
                                {
                                    regionName = region.Datomatic;
                                    break;
                                }
                            }
                        }

                        XElement ReleaseNode = new XElement("release");
                        ReleaseNode.Add(new XAttribute("name", elementName));
                        ReleaseNode.Add(new XAttribute("region", regionName));
                        gameElement.Add(ReleaseNode);
                    }
                }

                try
                {
                    List<XElement> parentElements = DatomaticFile.Root.Descendants("game").Where(x => x.SafeGetA(attribute: "cloneof") == null).ToList();
                    List<XElement> childElements = DatomaticFile.Root.Descendants("game").Where(x => x.SafeGetA(attribute: "cloneof") != null).ToList();
                    Watch2.Restart();
                    Debug.WriteLine("00: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
                    int j = 0;
                    int parentCount = parentElements.Count();
                    int releaseCount = DatomaticFile.Root.Descendants("release").Count();
                    int romCount = DatomaticFile.Root.Descendants("rom").Count();

                    Reporter.Report("Found " + parentCount + " games, " + romCount + " ROMs and " + releaseCount + " releases.");

                    //Extract releases from the datomatic file
                    foreach (XElement parentElement in parentElements)
                    {

                        // For reporting only
                        if (j++ % (parentCount / 10) == 0)
                        {
                            Reporter.Report("Getting " + j + @" / " + parentCount + " " + Watch1.Elapsed.ToString(@"m\:ss") + " elapsed.");
                            Watch1.Restart();
                        }

                        Game game = null;
                        string parentTitle = parentElement.SafeGetA(attribute: "name");

                        Watch2.Restart();
                        // Collect all child roms
                        List<XElement> romElements = childElements.Where(x => x.SafeGetA(attribute: "cloneof") == parentTitle).ToList();
                        romElements.Insert(0, parentElement);
                        Debug.WriteLine("A: " + Watch2.ElapsedMilliseconds); Watch2.Restart();

                        // Check if game exists
                        foreach (XElement romElement in romElements)
                        {
                            string romElementSha1 = romElement.SafeGetA(element1: "rom", attribute: "sha1");

                            Watch2.Restart();
                            Release release = platform.Releases.FirstOrDefault(x => x.Rom.SHA1 == romElementSha1);
                            if (release != null)
                            {
                                game = release.Game;
                                break; // Game exists--no need to keep looking
                            }
                            Debug.WriteLine("AA: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
                        }
                        Debug.WriteLine("B: " + Watch2.ElapsedMilliseconds); Watch2.Restart();

                        // If the game wasn't found, create a new one and add it
                        if (game == null)
                        {
                            game = new Game();
                            R.Data.Games.Add(game);
                        }

                        // Check if each rom exists
                        foreach (XElement romElement in romElements)
                        {
                            Rom rom = null;
                            string romElementSha1 = romElement.SafeGetA(element1: "rom", attribute: "sha1");
                            if (romElementSha1 == null)
                            {
                                continue; // Malformed element
                            }

                            rom = R.Data.Roms.FirstOrDefault(x => x.SHA1 == romElementSha1);

                            // Not found, create a new one
                            if (rom == null)
                            {
                                rom = new Rom() { Platform_ID = platform.ID };
                                R.Data.Roms.Add(rom);
                            }
                            Watch2.Restart();
                            // Whether existing or new, overwrite properties with new data
                            ParseElementToRom(romElement, rom);
                            Debug.WriteLine("C: " + Watch2.ElapsedMilliseconds); Watch2.Restart();

                            // Get the releases from the rom element
                            foreach (XElement releaseElement in romElement.Descendants("release"))
                            {
                                string releaseTitle = releaseElement.SafeGetA(attribute: "name");
                                string releaseRegionTitle = releaseElement.SafeGetA(attribute: "region");
                                if (releaseRegionTitle == null)
                                {

                                }
                                long? regionID = R.Data.Regions.FirstOrDefault(x => (x.Datomatic == releaseRegionTitle) || (x.Title == releaseRegionTitle)).ID;

                                if (regionID == null)
                                {
                                    Reporter.Report("Skipped Datomatic release (SHA1: " + romElementSha1 + ", Region: " + releaseRegionTitle + ") because the region wasn't recognized. Consider adding this region to the database");
                                    continue;
                                }
                                Watch2.Restart();
                                Release release = platform.Releases.FirstOrDefault(x => x.Rom_ID == rom.ID && x.Region_ID == regionID);
                                if (release == null)
                                {
                                    release = new Release();
                                    release.Game = game;
                                    release.Rom = rom;
                                    release.Region_ID = regionID;
                                    platform.Releases.Add(release);
                                }
                                Debug.WriteLine("D: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
                                ParseElementToRelease(releaseElement, release);
                                Debug.WriteLine("E: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
                            }
                        }
                    }

                    // Update platform cache date
                    string dateString = DatomaticFile.SafeGetB("header", "date");
                    DateTime cacheDate = new DateTime();
                    if (dateString != null)
                    {
                        CultureInfo enUS = new CultureInfo("en-US");
                        if (DateTime.TryParseExact(dateString, "yyyyMMdd-hhmmss", enUS,
                             DateTimeStyles.None, out cacheDate))
                        {
                            platform.CacheDate = cacheDate;
                        }
                        else
                        {
                            platform.CacheDate = DateTime.Now;
                        }
                    }

                    Reporter.Report("Finished. " + Watch2.Elapsed.ToString(@"m\:s") + " total elapsed");
                }

                catch (NullReferenceException)
                {
                    // Ignore
                }

            }
            else
            {
                Reporter.Report("File not found.");
            }
        }

        void ParseElementToRelease(XElement releaseElement, Release release)
        {
            // Get release properties from release node
            release.Title = releaseElement.SafeGetA(attribute: "name");

            // Check if release is unlicensed
            release.Unlicensed = Regex.IsMatch(release.Title, UNL);

            // Check if release is a BIOS
            if (release.Title.Contains(@"[BIOS]"))
            {
                release.IsGame = false;
            }

            else
            {
                release.IsGame = true;
            }

            // Get languages from datomatic using regex from parenthesis
            release.Language = "";

            foreach (Language language in R.Data.Languages)
            {
                if (Regex.IsMatch(release.Title, @"\([^)]*" + language.Abbreviation + @"[^)]*\)"))
                {
                    release.Language += language.Abbreviation + ",";
                }
            }
            release.Language = release.Language.TrimEnd(',');

            // Get special info using regex from parenthesis
            release.Special = "";

            MatchCollection matchList = Regex.Matches(release.Title, @"\([^)]*\)");
            var Matches = matchList.Cast<System.Text.RegularExpressions.Match>().Select(match => match.Value).ToList();

            Matches.RemoveAll(x => Regex.IsMatch(x, @".*Rev.*|.*v.*|.*Beta.*|.*Proto.*|.*NTSC.*|.*PAL.*|.*Unl.*|.*Japan.*|.*USA.*"));

            for (int i = 0; i < Matches.Count(); i++)
            {
                Matches[i] = Matches[i].Replace("(", "").Replace(")", "");
            }

            foreach (Region region in R.Data.Regions)
            {
                Matches.Remove(region.Title);
            }

            Matches.Remove(release.Language);
            string languages = @"\w{2},\w{2},.*";
            Matches.RemoveAll(x => Regex.IsMatch(x, languages));

            if (Matches.Count() > 0)
            {
                release.Special = string.Join(", ", Matches);
            }

            // Get version from datomatic using regex from parenthesis
            release.Version = Regex.Match(release.Title, @"(?<=\()Rev [^)]*(?=\))|(?<=\()v[^)]*(?=\))|(?<=\()Beta[^)]*(?=\))|(?<=\()Proto[^)]*(?=\))").ToString();

            // Get the video format from datomatic using regex from parenthesis
            release.VideoFormat = Regex.Match(release.Title, @"(?<=\()NTSC(?=\))|(?<=\()PAL(?=\))").ToString();

            // Remove all parenthetical info from title
            release.Title = Regex.Replace(release.Title, @"\([^)]*\)", string.Empty);
            release.Title = release.Title.TrimEnd(' ');



            // Move "the" to front of title
            if (release.Title.Length > 3 && release.Title.Substring(release.Title.Length - 3, 3).ToLower() == "the")
            {
                release.Title = "The " + release.Title.Substring(0, release.Title.Length - 5);
            }
        }

        void ParseElementToRom(XElement romElement, Rom rom)
        {
            // Get rom properties from parent node
            rom.Source = "Datomatic";
            rom.CRC32 = romElement.SafeGetA(element1: "rom", attribute: "crc");
            rom.MD5 = romElement.SafeGetA(element1: "rom", attribute: "md5");
            rom.SHA1 = romElement.SafeGetA(element1: "rom", attribute: "sha1"); ;
            rom.Size = romElement.SafeGetA(element1: "rom", attribute: "size");
            rom.Title = romElement.SafeGetA(element1: "rom", attribute: "name");
            rom.Title = Path.GetFileNameWithoutExtension(rom.Title);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Datomatic()
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
                // R.Data.GBPlatforms.Dispose()
                ;
            }

            // release any unmanaged objects
            // set the object references to null

            //R.Data = null;

            disposed = true;
        }
    }
}

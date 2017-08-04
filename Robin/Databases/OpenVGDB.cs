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
using System.Threading.Tasks;

namespace Robin
{
    class OpenVGDB : IDB
    {
        public string Title { get { return "Open VGDB"; } }

        OpenVGDBEntities OVdata;

        public LocalDB DB { get { return LocalDB.OpenVGDB; } }

        public DbSet Platforms => R.Data.OVGPlatforms;

        public DbSet Releases => R.Data.OVGReleases;

        public bool HasRegions => true;

        bool disposed;

        public OpenVGDB()
        {
            Reporter.Tic("Opening Open VGDB cache...");

            R.Data.OVGPlatforms.Load();
            R.Data.OVGReleases.Load();
            //R.Data.Releases.Include(x => x.OVGRelease);

            Reporter.Toc();
        }

        void LoadOVData()
        {
            OVdata = new OpenVGDBEntities();

            OVdata.VGDBPLATFORMS.Load();
            OVdata.VGDBRELEASES.Load();
            OVdata.VGDBROMS.Load();
            OVdata.VGDBREGIONS.Load();
        }

        public void CachePlatformReleases(IDBPlatform idbPlatform)
        {
            if (OVdata == null)
            {
                LoadOVData();
            }

            OVGPlatform ovgPlatform = idbPlatform as OVGPlatform;

            List<VGDBRELEAS> list = OVdata.VGDBRELEASES.Where(x => x.VGDBROM.systemID == ovgPlatform.ID).ToList();

            foreach (VGDBRELEAS vgdbr in list)
            {
                ovgPlatform.OVGReleases.Add(vgdbr);
            }
        }

        public void CachePlatforms()
        {
            Reporter.Report("Open VGDB uses Robin's platforms and does not need platforms cached.");
        }

        public void CachePlatformData(IDBPlatform idbPlatform)
        {
            Reporter.Report("Open VGDB uses Robin's platforms and does not need platform data cached.");
        }

        public void CachePlatformGames(IDBPlatform idbPlatform)
        {
            Reporter.Report("GamesDB does not have games and releases--try caching releases");
        }

        [Conditional("DEBUG")]
        public static async void GetAtariGames()
        {
            using (OpenVGDBEntities OVGdata = new OpenVGDBEntities())
            {
                await Task.Run(() =>
                {
                    OVGdata.Configuration.LazyLoadingEnabled = false;

                    OVGdata.VGDBROMS.Load();
                    OVGdata.VGDBRELEASES.Load();
                    OVGdata.VGDBRELEASES.Include(XamlGeneratedNamespace => XamlGeneratedNamespace.VGDBROM).Load();

                    List<VGDBROM> atariVgdbRoms = OVGdata.VGDBROMS.Where(x => x.systemID == 3 && !x.romFileName.Contains(@"(Hack)") && !x.romFileName.Contains(@"(208 in 1)") && !x.romFileName.Contains(@"(CCE)") && !x.romFileName.Contains(@"(2600 Screen Search Console)")).OrderBy(x => x.romFileName).ToList();
                    List<Release> newAtariReleases = new List<Release>();
                    List<VGDBROM> finishedVgdbRoms = new List<VGDBROM>();

                    int z = 0;
                    Platform atariPlatform = R.Data.Platforms.FirstOrDefault(x => x.Title.Contains("2600"));
                    foreach (VGDBROM atariVgdbRom in atariVgdbRoms)
                    {
                        if (!finishedVgdbRoms.Contains(atariVgdbRom))
                        {
                            Reporter.Report(z++.ToString());
                            Game game = new Game();
                            R.Data.Games.Add(game);

                            List<VGDBROM> currentGameVgdbRoms = atariVgdbRoms
                            .Where(x =>
                                x.AtariParentTitle == atariVgdbRom.AtariParentTitle ||
                                (!string.IsNullOrEmpty(x.AKA) && x.AKA == atariVgdbRom.AtariParentTitle) ||
                                (!string.IsNullOrEmpty(atariVgdbRom.AKA) && atariVgdbRom.AKA == x.AtariParentTitle) &&
                                !finishedVgdbRoms.Contains(x)
                                 ).ToList();

                            VGDBROM parentRom = currentGameVgdbRoms.FirstOrDefault(x => x.romFileName.Contains(@"~"));
                            if (parentRom != null)
                            {
                                currentGameVgdbRoms.Remove(parentRom);
                                currentGameVgdbRoms.Insert(0, parentRom);
                            }

                            foreach (VGDBROM gameVgdbRom in currentGameVgdbRoms)
                            {
                                Rom rom = gameVgdbRom;

                                if (!atariPlatform.Roms.Any(x => x.SHA1 == rom.SHA1))
                                {
                                    atariPlatform.Roms.Add(rom);
                                    foreach (VGDBRELEAS atariVgdbRelease in gameVgdbRom.VGDBRELEASES)
                                    {
                                        Release release = atariVgdbRelease;
                                        release.Rom = rom;
                                        release.Game = game;
                                        newAtariReleases.Add(release);
                                    }
                                }
                            }
                            finishedVgdbRoms.AddRange(currentGameVgdbRoms);
                        }
                    }

                    newAtariReleases = newAtariReleases.OrderBy(x => x.Title).ToList();
                    atariPlatform.Releases.AddRange(newAtariReleases);

                    R.Data.Save();
                    //TODO Report total removed
                });
            }
        }

        public void ReportUpdates( bool detect)
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
            var ovgReleaseEntries = R.Data.ChangeTracker.Entries<OVGRelease>();

            Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);

            int ovgReleaseAddCount = ovgReleaseEntries.Count(x => x.State == EntityState.Added);
            int ovgReleaseModCount = ovgReleaseEntries.Count(x => x.State == EntityState.Modified);

            Reporter.Report("OVGReleases added: " + ovgReleaseAddCount + ", OVGReleases updated: " + ovgReleaseModCount);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OpenVGDB()
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

                if (OVdata != null)
                {
                    OVdata.Dispose();
                }

            }

            // release any unmanaged objects
            // set the object references to null

            OVdata = null;

            disposed = true;
        }
    }
}

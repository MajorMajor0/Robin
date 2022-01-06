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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Robin
{
	class OpenVGDB : IDB
	{
		public string Title => "Open VGDB";

		public LocalDB DB => LocalDB.OpenVGDB;

		public IEnumerable<IDBPlatform> Platforms => R.Data.Ovgplatforms;

		public IEnumerable<IDBRelease> Releases => R.Data.Ovgreleases;

		OpenVGDBEntities OVdata;

		public bool HasRegions => true;

		bool disposed;

		public OpenVGDB()
		{
			Reporter.Tic("Opening Open VGDB cache...", out int tic1);

			R.Data.Ovgplatforms.Load();
			R.Data.Ovgreleases.Load();
			//R.Data.Releases.Include(x => x.Ovgrelease);

			Reporter.Toc(tic1);
		}

		void LoadOVData()
		{
			OVdata = new OpenVGDBEntities();

			OVdata.VGdbplatforms.Load();
			OVdata.VGdbreleaseS.Load();
			OVdata.VGDBROMS.Load();
			OVdata.VGDBREGIONS.Load();
		}

		public void CachePlatformReleases(Platform platform)
		{
			if (OVdata == null)
			{
				LoadOVData();
			}

			Ovgplatform Ovgplatform = platform.Ovgplatform;

			List<VGDBRELEAS> list = OVdata.VGdbreleaseS.Where(x => x.VGDBROM.systemID == Ovgplatform.Id).ToList();

			foreach (VGDBRELEAS vgdbr in list)
			{
				Ovgplatform.Ovgreleases.Add(vgdbr);
			}
		}

		public void CachePlatforms()
		{
			Reporter.Report("Open VGDB uses Robin's platforms and does not need platforms cached.");
		}

		/// <summary>
		/// Implements IDB.CachePlatformdata() Update the local DB cache of platform associated metadata. Not implemented for OVG.
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the DBPlatorm to update.</param>
		public void CachePlatformData(Platform platform)
		{
			Reporter.Report("Open VGDB uses Robin's platforms and does not need platform data cached.");
		}

		public void CachePlatformGames(Platform platform)
		{
			Reporter.Report("GamesDB does not have games and releases--try caching releases");
		}

#if DEBUG
		public static async Task GetAtariGamesAsync()
		{
			using (OpenVGDBEntities OVGdata = new OpenVGDBEntities())
			{
				await Task.Run(() =>
				{
					OVGdata.Configuration.LazyLoadingEnabled = false;

					OVGdata.VGDBROMS.Load();
					OVGdata.VGdbreleaseS.Load();
					OVGdata.VGdbreleaseS.Include(XamlGeneratedNamespace => XamlGeneratedNamespace.VGDBROM).Load();

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

								if (!atariPlatform.Roms.Any(x => x.Sha1 == rom.Sha1))
								{
									atariPlatform.Roms.Add(rom);
									foreach (VGDBRELEAS atariVGdbrelease in gameVgdbRom.VGdbreleaseS)
									{
										Release release = atariVGdbrelease;
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

					R.Save();
					//TODO Report total removed
				});
			}
		}
#endif
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
			var OvgreleaseEntries = R.Data.ChangeTracker.Entries<Ovgrelease>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int OvgreleaseAddCount = OvgreleaseEntries.Count(x => x.State == EntityState.Added);
			int OvgreleaseModCount = OvgreleaseEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("Ovgreleases added: " + OvgreleaseAddCount + ", Ovgreleases updated: " + OvgreleaseModCount);
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

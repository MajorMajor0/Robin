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

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Robin;

class OpenVGDB : IDB
{
	public string Title => "Open VGDB";

	public LocalDB DB => LocalDB.OpenVGDB;

	public IEnumerable<IDbPlatform> Platforms
		=> R.Data.OVGPlatforms.Local.ToObservableCollection();

	public IEnumerable<IDbRelease> Releases
		=> R.Data.OVGReleases.Local.ToObservableCollection();

	OpenVGDBEntities OVdata;

	public bool HasRegions => true;

	bool disposed;

	public OpenVGDB()
	{
		try
		{
			Reporter.Tic("Opening Open VGDB cache...", out int tic1);

			R.Data.OVGPlatforms.Load();
			R.Data.OVGReleases.Load();
			//R.Data.Releases.Include(x => x.OVGRelease);

			Reporter.Toc(tic1);
		}
		catch (InvalidOperationException ex)
		{
			MessageBox.Show(ex.Message, $"Problem Opening Open VGDB from RobinData", MessageBoxButton.OK);
		}
		catch (Microsoft.Data.Sqlite.SqliteException ex)
		{
			MessageBox.Show(ex.Message, "Sqlite Exception loading Open VGDB", MessageBoxButton.OK);
		}
	}

	void LoadOVData()
	{
		OVdata = new OpenVGDBEntities();

		OVdata.VGDBPlatforms.Load();
		OVdata.VGDBReleaseS.Load();
		OVdata.VGDBROMS.Load();
		OVdata.VGDBREGIONS.Load();
	}

	public void CachePlatformReleases(Platform platform)
	{
		if (OVdata == null)
		{
			LoadOVData();
		}

		OVGPlatform OVGPlatform = platform.OVGPlatform;

		List<VGDBRELEAS> list = OVdata.VGDBReleaseS.Where(x => x.VGDBROM.systemID == OVGPlatform.ID).ToList();

		foreach (VGDBRELEAS vgdbr in list)
		{
			OVGPlatform.OVGReleases.Add(vgdbr);
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

	public void CachePlatformGamesAsync(Platform platform)
	{
		Reporter.Report("GamesDB does not have games and releases--try caching releases");
	}

#if DEBUG
	public static async Task GetAtariGamesAsync()
	{
		using OpenVGDBEntities OVGdata = new();
		await Task.Run(() =>
		{
			OVGdata.Configuration.LazyLoadingEnabled = false;

			OVGdata.VGDBROMS.Load();
			OVGdata.VGDBReleaseS.Load();
			OVGdata.VGDBReleaseS.Include(XamlGeneratedNamespace => XamlGeneratedNamespace.VGDBROM).Load();

			List<VGDBROM> atariVgdbRoms = OVGdata.VGDBROMS.Where(x => x.systemID == 3 && !x.romFileName.Contains(@"(Hack)") && !x.romFileName.Contains(@"(208 in 1)") && !x.romFileName.Contains(@"(CCE)") && !x.romFileName.Contains(@"(2600 Screen Search Console)")).OrderBy(x => x.romFileName).ToList();
			List<Release> newAtariReleases = new();
			List<VGDBROM> finishedVgdbRoms = new();

			int z = 0;
			Platform atariPlatform = R.Data.Platforms.FirstOrDefault(x => x.Title.Contains("2600"));
			foreach (VGDBROM atariVgdbRom in atariVgdbRoms)
			{
				if (!finishedVgdbRoms.Contains(atariVgdbRom))
				{
					Reporter.Report(z++.ToString());
					Game game = new();
					R.Data.Games.Add(game);

					List<VGDBROM> currentGameVgdbRoms = atariVgdbRoms
					.Where(x =>
						x.AtariParentTitle == atariVgdbRom.AtariParentTitle ||
						(!string.IsNullOrEmpty(x.AKA) && x.AKA == atariVgdbRom.AtariParentTitle) ||
						(!string.IsNullOrEmpty(atariVgdbRom.AKA) && atariVgdbRom.AKA == x.AtariParentTitle) &&
						!finishedVgdbRoms.Contains(x)
						 ).ToList();

					VGDBROM parentRom = currentGameVgdbRoms.FirstOrDefault(x => x.romFileName.Contains('~'));
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
							foreach (VGDBRELEAS atariVGDBRelease in gameVgdbRom.VGDBReleaseS)
							{
								Release release = atariVGDBRelease;
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
		var OVGReleaseEntries = R.Data.ChangeTracker.Entries<OVGRelease>();
#if DEBUG
		Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
		int OVGReleaseAddCount = OVGReleaseEntries.Count(x => x.State == EntityState.Added);
		int OVGReleaseModCount = OVGReleaseEntries.Count(x => x.State == EntityState.Modified);

		Reporter.Report("OVGReleases added: " + OVGReleaseAddCount + ", OVGReleases updated: " + OVGReleaseModCount);
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

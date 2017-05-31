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
 
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Robin
{
	class OpenVGDB
	{
		public static int CachePlatformGames(Platform platform)
		{
			using (OpenVGDBEntities OVdata = new OpenVGDBEntities())
			{
				Reporter.Report("Loading OVGDB");
				OVdata.VGDBPLATFORMS.Load();
				OVdata.VGDBRELEASES.Load();
				OVdata.VGDBROMS.Load();
				OVdata.VGDBREGIONS.Load();
				Reporter.Report("OVGDB, caching games");
				List<VGDBRELEAS> list = OVdata.VGDBRELEASES.Where(x => x.VGDBROM.systemID == platform.ID).ToList();

				foreach (VGDBRELEAS vgdbr in list)
				{
					platform.OVGReleases.Add(vgdbr);
				}
				Reporter.Report("Cached " + list.Count() + " games.");
				return list.Count();
			}
		}

		public static async void GetAtariGames()
		{
			using (RobinDataEntities Rdata = new RobinDataEntities())
			using (OpenVGDBEntities OVGdata = new OpenVGDBEntities())
			{
				await Task.Run(() =>
				{
					Rdata.Configuration.LazyLoadingEnabled = false;
					Rdata.Configuration.AutoDetectChangesEnabled = false;
					OVGdata.Configuration.LazyLoadingEnabled = false;

					Rdata.Roms.Load();
					Rdata.Platforms.Load();
					Rdata.Games.Load();
					Rdata.Regions.Load();
					Rdata.Releases.Load();
					Rdata.Releases.Include(x => x.Rom).Load();

					OVGdata.VGDBROMS.Load();
					OVGdata.VGDBRELEASES.Load();
					OVGdata.VGDBRELEASES.Include(XamlGeneratedNamespace => XamlGeneratedNamespace.VGDBROM).Load();

					List<VGDBROM> atariVgdbRoms = OVGdata.VGDBROMS.Where(x => x.systemID == 3 && !x.romFileName.Contains(@"(Hack)") && !x.romFileName.Contains(@"(208 in 1)") && !x.romFileName.Contains(@"(CCE)") && !x.romFileName.Contains(@"(2600 Screen Search Console)")).OrderBy(x => x.romFileName).ToList();
					List<Release> newAtariReleases = new List<Release>();
					List<VGDBROM> finishedVgdbRoms = new List<VGDBROM>();

					int z = 0;
					Platform atariPlatform = Rdata.Platforms.FirstOrDefault(x => x.Title.Contains("2600"));
					foreach (VGDBROM atariVgdbRom in atariVgdbRoms)
					{
						if (!finishedVgdbRoms.Contains(atariVgdbRom))
						{
							Reporter.Report(z++.ToString());
							Game game = new Game();
							Rdata.Games.Add(game);

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
					Rdata.ChangeTracker.DetectChanges();
					int i = Rdata.Save();
					Reporter.Report(i + " changes pushed to database.");
				});
			}
		}
	}
}

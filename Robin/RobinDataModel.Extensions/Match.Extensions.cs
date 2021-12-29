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
 
//using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Robin
{
	public partial class Match
	{
		public static implicit operator Match(Release release)
		{
			Match match = new Match();
			match.Id = release.Id;
			match.IdGb = release.ID_GB;
			match.IdGdb = release.ID_GDB;
			match.IdOvg = release.ID_OVG;
			match.Sha1 = release.Rom.Sha1;
			match.RegionId = release.RegionId;

			return match;
		}

		public static void StoreMatches()
		{
			R.Data.ChangeTracker.AutoDetectChangesEnabled = false;
			R.Data.ChangeTracker.LazyLoadingEnabled = false;
			R.Data.Matches.Load();
			int i = 0;
			foreach (Release release in R.Data.Releases)
			{
				if (release.Rom.Sha1 != null && (release.ID_GB != null || release.ID_GDB != null || release.ID_OVG != null))
				{
					R.Data.Matches.Add(release);
				}
				Debug.WriteLine(i++);
			}
			R.Data.Save();
            // TODO Report total
        }

        public static async void RestoreMatches()
		{
			R.Data.ChangeTracker.AutoDetectChangesEnabled = false;
			R.Data.ChangeTracker.LazyLoadingEnabled = false;
			R.Data.Matches.Load();
			int i = 0;
			await Task.Run(() =>
			{
				foreach (Match match in R.Data.Matches)
				{
					Release release = R.Data.Releases.FirstOrDefault(x => x.Rom.Sha1 == match.Sha1 && x.RegionId == match.RegionId);
					if (release != null)
					{
						Reporter.Report((i++).ToString() + " Matched " + release.TitleAndRegion);
						release.ID_GB = release.ID_GB ?? match.IdGb;
						release.ID_GDB = release.ID_GDB ?? match.IdGdb;
						release.ID_OVG = release.ID_OVG ?? match.IdOvg;
					}
				}

				R.Data.Save();
                // TODO Report total
            });
		}
	}
}

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
 
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Robin
{
	public partial class Match
	{
		public static implicit operator Match(Release release)
		{
			Match match = new Match();
			match.ID = release.ID;
			match.ID_GB = release.ID_GB;
			match.ID_GDB = release.ID_GDB;
			match.ID_OVG = release.ID_OVG;
			match.SHA1 = release.SHA1;
			match.Region_ID = release.Region_ID;

			return match;
		}

		public static void StoreMatches(RobinDataEntities Rdata)
		{
			Rdata.Configuration.AutoDetectChangesEnabled = false;
			Rdata.Configuration.LazyLoadingEnabled = false;
			Rdata.Matches.Load();
			int i = 0;
			foreach (Release release in Rdata.Releases)
			{
				if (release.SHA1 != null && (release.ID_GB != null || release.ID_GDB != null || release.ID_OVG != null))
				{
					Rdata.Matches.Add(release);
				}
				Debug.WriteLine(i++);
			}
			Rdata.ChangeTracker.DetectChanges();
			int o = Rdata.Save();
			Rdata.Configuration.AutoDetectChangesEnabled = true;
			Reporter.Report(o + " changes pushed to database");
		}

		public static async void RestoreMatches(RobinDataEntities Rdata)
		{
			Rdata.Configuration.AutoDetectChangesEnabled = false;
			Rdata.Configuration.LazyLoadingEnabled = false;
			Rdata.Matches.Load();
			int i = 0;
			await Task.Run(() =>
			{
				foreach (Match match in Rdata.Matches)
				{

					Release release = Rdata.Releases.FirstOrDefault(x => x.Rom.SHA1 == match.SHA1 && x.Region_ID == match.Region_ID);
					if (release != null)
					{
						Reporter.Report((i++).ToString() + " Matched " + release.TitleAndRegion);
						release.ID_GB = release.ID_GB ?? match.ID_GB;
						release.ID_GDB = release.ID_GDB ?? match.ID_GDB;
						release.ID_OVG = release.ID_OVG ?? match.ID_OVG;
					}
				}
				Rdata.ChangeTracker.DetectChanges();
				int o = Rdata.Save();
				Rdata.Configuration.AutoDetectChangesEnabled = true;
				Reporter.Report(o + " changes pushed to database");
			});
		}
	}
}

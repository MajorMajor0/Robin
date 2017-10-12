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
using System.Diagnostics;
using System.Linq;

namespace Robin
{
	public partial class LBGame
	{
		//public static List<LBGame> GetGames(Platform platform)
		//{
		//    R.Data.LBGames.Load();
		//    R.Data.LBGames.Include(x => x.LBImages).Load();
		//    R.Data.Regions.Load();
		//    return R.Data.LBGames.Where(x => x.LBPlatform_ID == platform.ID_LB).ToList();
		//}

		public string RegionTitle
		{
			get; set;
		}

		public Region Region { get { return null; } }

		public string Regions
		{
			get
			{
				if (LBImages != null)
				{
					return string.Join(", ", LBImages.Select(x => x.Region.Title).Distinct());
				}
				return null;
			}
		}

		public List<LBImage> LBImages => LBReleases.SelectMany(x => x.LBImages).ToList();


		[Conditional("DEBUG")]

		public void SetLBReleasePlatform()
		{
			foreach (LBRelease lbRelease in LBReleases)
			{
				lbRelease.LBPlatform = LBPlatform;
			}
		}

		public void CreateReleases()
		{
			List<LBImage> lbImages = LBImages.Where(x => x.LBRelease_ID == null).ToList();

			for (int i = 0; i < lbImages.Count; i++)
			{
				LBRelease lbRelease = LBReleases.FirstOrDefault(x => x.Region == lbImages[i].Region);

				if (lbRelease == null)
				{
					lbRelease = new LBRelease();
					LBReleases.Add(lbRelease);
					lbRelease.Region = lbImages[i].Region;
					lbRelease.LBPlatform = LBPlatform;
				}

				lbRelease.LBImages.Add(lbImages[i]);
			}
		}
	}
}

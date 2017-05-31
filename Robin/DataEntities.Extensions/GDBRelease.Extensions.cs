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

namespace Robin
{
	public partial class GDBRelease : IComparableDB
	{
		public string BoxArtFrontThumbURL
		{
			get
			{
				if (BoxFrontURL == null)
				{
					return null;
				}
				else
				{
					return BoxFrontURL.Replace(@"/boxart", @"/boxart/thumb");
				}

			}
		}

		public string BoxArtBackThumbURL
		{
			get
			{
				if (BoxBackURL == null)
				{
					return null;
				}
				else
				{
					return BoxBackURL.Replace(@"/boxart", @"/boxart/thumb");
				}

			}
		}

		public string RegionTitle
		{
			get
			{
				return null;
			}
		}

		public static List<GDBRelease> GetGames(Platform platform)
		{
			using (RobinDataEntities Rdata = new RobinDataEntities())
			{
				Rdata.GDBReleases.Load();
				Rdata.Regions.Load();
				return Rdata.GDBReleases.Where(x => x.Platform_ID == platform.ID_GDB).ToList();
			}
		}
	}
}

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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Robin
{
	public partial class GDBPlatform : IDBPlatform
	{
		public IList Releases => GDBReleases.ToList();

		public Platform RPlatform => R.Data.Platforms.Local.FirstOrDefault(x => x.ID_GDB == ID);

		public int MatchedReleaseCount
		{
			get
			{
				if (RPlatform != null)
				{
					return RPlatform.MatchedToGamesDB;
				}
				return 0;
			}
		}

		public bool Preferred
		{
			get
			{
				if (RPlatform != null)
				{
					return RPlatform.Preferred;
				}
				return false;
			}
		}
	}
}

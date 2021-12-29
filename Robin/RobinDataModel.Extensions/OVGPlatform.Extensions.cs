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
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Robin
{
	public partial class Ovgplatform : IDBPlatform
	{
		[NotMapped]
		public IList Releases => Ovgreleases.ToList();

		[NotMapped]
		public Platform RPlatform => R.Data.Platforms.FirstOrDefault(x => x.Id == Id);

		[NotMapped]
		public int MatchedReleaseCount
		{
			get
			{
				if (RPlatform != null)
				{
					return RPlatform.MatchedToOpenVG;
				}
				return 0;
			}
		}

		[NotMapped]
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

		[NotMapped]
		public string Manufacturer => null;

		[NotMapped]
		public DateTime? Date => null;

		[NotMapped]
		public DateTime CacheDate { get; set; }
	}
}

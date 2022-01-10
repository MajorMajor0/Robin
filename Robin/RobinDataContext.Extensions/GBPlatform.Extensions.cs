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

using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Robin
{
	public partial class Gbplatform : IDBPlatform
	{
		[NotMapped]
		public IList Releases => Gbreleases.ToList();
		
		[NotMapped]
		public Platform RPlatform => R.Data.Platforms.FirstOrDefault((System.Linq.Expressions.Expression<Func<Platform, bool>>)(x => x.ID_GB == this.Id));
		
		[NotMapped]
		public int MatchedReleaseCount
		{
			get
			{
				if (RPlatform != null)
				{
					return RPlatform.MatchedToGiantBomb;
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
		DateTime IDBPlatform.CacheDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
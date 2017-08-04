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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
    public partial class OVGPlatform : IDBPlatform
    {
        public IList Releases => OVGReleases;

        public string Manufacturer => null;

        public DateTime? Date => null;

        public Platform RPlatform
        {
            get
            {
                return R.Data.Platforms.FirstOrDefault(x => x.ID == ID);
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

        public DateTime CacheDate { get; set; }
    }
}

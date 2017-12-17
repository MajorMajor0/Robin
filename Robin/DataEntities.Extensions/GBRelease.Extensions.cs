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
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;

namespace Robin
{
	public partial class GBRelease : IDBRelease
	{
		public static List<GBRelease> GetGames(Platform platform)
		{
			R.Data.GBReleases.Load();
			R.Data.Regions.Load();
			return R.Data.GBReleases.Where(x => x.GBPlatform_ID == platform.ID_GB).ToList();
		}

		public string RegionTitle
		{
			get { return Region.Title; }
		}

        const string typeString = "GBR";

        public Banner Banner => null;

        public Box3D Box3D => null;

        public BoxBack BoxBack => null;

        //BoxFront boxFront;
        //public BoxFront BoxFront
        //{
        //    get
        //    {
        //        if (boxFront == null && BoxURL != null)
        //        {
        //            boxFront = new BoxFront(BoxURL, typeString, ID);
        //        }
        //        return boxFront;
        //    }
        //}

        public Cart3D Cart3D => null;

        public CartBack CartBack => null;

        public CartFront CartFront => null;

        public ControlInformation ControlInformation => null;

        public ControlPanel ControlPanel => null;


        public Logo Logo => null;

        public Marquee Marquee => null;

        public string BoxURL => BoxFront?.URL;

        public string ScreenURL => Screen?.URL;

        //Screen screen;
        //public Screen Screen
        //{
        //    get
        //    {
        //        if (screen == null && ScreenURL != null)
        //        {
        //            screen = new Screen(ScreenURL, typeString, ID);
        //        }
        //        return screen;
        //    }
        //}
    }
}

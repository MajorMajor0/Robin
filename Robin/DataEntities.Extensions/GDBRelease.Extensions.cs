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
using System.IO;
using System.Net;

namespace Robin
{
    public partial class GDBRelease : IDBRelease
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

        public Region Region
        {
            get { return null; }
        }

        const string typeString = "GDBR";

        Banner banner;
        public Banner Banner
        {
            get
            {
                if (banner == null && BannerURL != null)
                {
                    banner = new Banner(BannerURL);
                }
                return banner;
            }
        }

        public Box3D Box3D => null;

        BoxBack boxBack;
        public BoxBack BoxBack
        {
            get
            {
                if (boxBack == null && BoxBackURL != null)
                {
                    boxBack = new BoxBack(BoxBackURL);
                }
                return boxBack;
            }
        }

        BoxFront boxFront;
        public BoxFront BoxFront
        {
            get
            {
                if (boxFront == null && BoxFrontURL != null)
                {
                    boxFront = new BoxFront(BoxFrontURL);
                }
                return boxFront;
            }
        }

        public Cart3D Cart3D => null;

        public CartBack CartBack => null;

        public CartFront CartFront => null;

        public ControlInformation ControlInformation => null;

        public ControlPanel ControlPanel => null;

        Logo logo;
        public Logo Logo
        {
            get
            {
                if (logo == null && LogoURL != null)
                {
                    logo = new Logo(LogoURL);
                }
                return logo;
            }
        }

        public Marquee Marquee => null;

        Screen screen;
        public Screen Screen
        {
            get
            {
                if (screen == null && ScreenURL != null)
                {
                    screen = new Screen(ScreenURL);
                }
                return screen;
            }
        }


    }
}

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
using System.Linq;
using System.Net;

namespace Robin
{
	public partial class LBRelease : IDBRelease
	{
		public Region Region => R.Data.Regions.Local.FirstOrDefault(x => x.ID == Region_ID);

		public LBPlatform LBPlatform => R.Data.LBPlatforms.FirstOrDefault(x => x.ID == LBPlatform_ID);

		public string RegionTitle => Region.Title;

		public string Overview => LBGame.Overview;

		public DateTime? Date => LBGame.Date;

		public string Developer => LBGame.Developer;

		public string Publisher => LBGame.Publisher;

		public string Players => LBGame.Players;

		string typeString => "LBR";

		public string BannerURL => GetURL("Banner");
		public string Box3DURL => GetURL("Box - 3D");
		public string BoxBackURL => GetURL("Box - Back");
		public string BoxFrontURL => GetURL("Box - Front");
		public string Cart3DURL => GetURL("Cart - 3D");
		public string CartBackURL => GetURL("Cart - Back");
		public string CartFrontURL => GetURL("Cart - Front");
		public string ControlInformationURL => GetURL("Arcade - Controls Information");
		public string ControlPanelURL => GetURL("Arcade - Control Panel");
		public string LogoURL => GetURL("Clear Logo");
		public string MarqueeURL => GetURL("Arcade - Marquee");
		public string ScreenURL => GetURL("Screenshot - Gameplay");

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

        Box3D box3D;
        public Box3D Box3D
        {
            get
            {
                if (box3D == null && Box3DURL != null)
                {
                    box3D = new Box3D(Box3DURL);
                }
                return box3D;
            }
        }

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

        Cart3D cart3D;
        public Cart3D Cart3D
        {
            get
            {
                if (cart3D == null && Cart3DURL != null)
                {
                    cart3D = new Cart3D(Cart3DURL);
                }
                return cart3D;
            }
        }

        CartBack cartBack;
        public CartBack CartBack
        {
            get
            {
                if (cartBack == null && CartBackURL != null)
                {
                    cartBack = new CartBack(CartBackURL);
                }
                return cartBack;
            }
        }

        CartFront cartFront;
        public CartFront CartFront
        {
            get
            {
                if (cartFront == null && CartFrontURL != null)
                {
                    cartFront = new CartFront(CartFrontURL);
                }
                return cartFront;
            }
        }

        ControlInformation controlInformation;
        public ControlInformation ControlInformation
        {
            get
            {
                if (controlInformation == null && ControlInformationURL != null)
                {
                    controlInformation = new ControlInformation(ControlInformationURL);
                }
                return controlInformation;
            }
        }

        ControlPanel controlPanel;
        public ControlPanel ControlPanel
        {
            get
            {
                if (controlPanel == null && ControlPanelURL != null)
                {
                    controlPanel = new ControlPanel(ControlPanelURL);
                }
                return controlPanel;
            }
        }

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

        Marquee marquee;
        public Marquee Marquee
        {
            get
            {
                if (marquee == null && MarqueeURL != null)
                {
                    marquee = new Marquee(MarqueeURL);
                }
                return marquee;
            }
        }

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


        string GetURL(string type)
		{
			LBImage lbImage = LBImages.FirstOrDefault(x => x.Type == type);

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}
	}
}

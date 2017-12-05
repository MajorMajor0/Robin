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
		public string RegionTitle => Region.Title;

		public string Overview => LBGame.Overview;

		public DateTime? Date => LBGame.Date;

		public string Developer => LBGame.Developer;

		public string Publisher => LBGame.Publisher;

		public string Players => LBGame.Players;

		string typeString => "LBR";

		BannerArt banner;
		public BannerArt Banner
		{
			get
			{
				if (banner == null && GetURL("Banner") != null)
				{
					banner = new BannerArt(GetURL("Banner"), typeString, ID);
				}
				return banner;
			}
		}

		Box3DArt box3D;
		public Box3DArt Box3D
		{
			get
			{
				if (box3D == null && GetURL("Box - 3D") != null)
				{
					box3D = new Box3DArt(GetURL("Box - 3D"), typeString, ID);
				}
				return box3D;
			}
		}

		BoxBackArt boxBack;
		public BoxBackArt BoxBack
		{
			get
			{
				if (boxBack == null && GetURL("Box - Back") != null)
				{
					boxBack = new BoxBackArt(GetURL("Box - Back"), typeString, ID);
				}
				return boxBack;
			}
		}

		BoxFrontArt boxFront;
		public BoxFrontArt BoxFront
		{
			get
			{
				if (boxFront == null && GetURL("Box - Front") != null)
				{
					boxFront = new BoxFrontArt(GetURL("Box - Front"), typeString, ID);
				}
				return boxFront;
			}
		}

		Cart3DArt cart3D;
		public Cart3DArt Cart3D
		{
			get
			{
				if (cart3D == null && GetURL("Cart - 3D") != null)
				{
					cart3D = new Cart3DArt(GetURL("Cart - 3D"), typeString, ID);
				}
				return cart3D;
			}
		}

		CartBackArt cartBack;
		public CartBackArt CartBack
		{
			get
			{
				if (cartBack == null && GetURL("Cart - Back") != null)
				{
					cartBack = new CartBackArt(GetURL("Cart - Back"), typeString, ID);
				}
				return cartBack;
			}
		}

		CartFrontArt cartFront;
		public CartFrontArt CartFront
		{
			get
			{
				if (cartFront == null && GetURL("Cart - Front") != null)
				{
					cartFront = new CartFrontArt(GetURL("Cart - Front"), typeString, ID);
				}
				return cartFront;
			}
		}

		ControlInformationArt controlInformation;
		public ControlInformationArt ControlInformation
		{
			get
			{
				if (controlInformation == null && GetURL("Arcade - Control Information") != null)
				{
					controlInformation = new ControlInformationArt(GetURL("Arcade - Control Information"), typeString, ID);
				}
				return controlInformation;
			}
		}


		ControlPanelArt controlPanel;
		public ControlPanelArt ControlPanel
		{
			get
			{
				if (controlPanel == null && GetURL("Arcade - Control Panel") != null)
				{
					controlPanel = new ControlPanelArt(GetURL("Arcade - Control Panel"), typeString, ID);
				}
				return controlPanel;
			}
		}



		public Art ControlPanelArt => FileLocation.Temp + "LBR-" + ID + "-BXF.jpg";
		public Art LogoArt => FileLocation.Temp + "LBR-" + ID + "-LGO.jpg";
		public Art MarqueeArt => FileLocation.Temp + "LBR-" + ID + "-MAR.jpg";
		public Art ScreenArt => FileLocation.Temp + "LBR-" + ID + "-SCR.jpg";





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

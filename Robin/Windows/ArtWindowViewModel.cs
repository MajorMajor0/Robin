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
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	class ArtWindowViewModel
	{
		public Release Release { get; }

		public Lbrelease Lbrelease => Release.Lbrelease;

		public Gdbrelease Gdbrelease => Release.Gdbrelease;

		public Gbrelease Gbrelease => Release.Gbrelease;


		//public List<Art> LBArt { get; set; }

		//public List<Art> GDBArt { get; set; }

		//public List<Art> GBArt { get; set; }

		//public List<Art> OVGArt { get; set; }


		public ArtWindowViewModel(Release release)
		{
			Release = release;
			//LBArt = new List<Art>();

			Launchbox launchbox = new Launchbox();

			//R.Data.Releases.Include(x => x.Lbrelease).Load();

			//if (Lbrelease != null)
			//{
			//	if (!String.IsNullOrEmpty(Lbrelease.BoxFrontPath))
			//	{
			//		LBArt.Add(new Art("Banner", Lbrelease.BoxFrontUrl, Lbrelease.BoxFrontPath));
			//	}
			//	if (!String.IsNullOrEmpty(Lbrelease.BoxBackPath))
			//	{
			//		LBArt.Add(new Art("Banner", Lbrelease.BoxBackUrl, Lbrelease.BoxBackPath));
			//	}
			//	if (!String.IsNullOrEmpty(Lbrelease.BannerUrl))
			//	{
			//		LBArt.Add(new Art("Banner", Lbrelease.BannerUrl, Lbrelease.BannerPath));
			//	}

			//	if (!String.IsNullOrEmpty(Lbrelease.Box3DURL))
			//	{
			//		LBArt.Add(new Art("Banner", Lbrelease.Box3DURL, Lbrelease.Box3DPath));
			//	}

			//	if (!String.IsNullOrEmpty(Lbrelease.Cart3DURL))
			//	{
			//		LBArt.Add(new Art("Banner", Lbrelease.Cart3DURL, Lbrelease.Cart3DPath));
			//	}

			//	if (!String.IsNullOrEmpty(Lbrelease.CartBackURL))
			//	{
			//		LBArt.Add(new Art("Cartridge Back", Lbrelease.CartBackURL, Lbrelease.CartBackPath));
			//	}

			//	if (!String.IsNullOrEmpty(Lbrelease.ControlPanelURL))
			//	{
			//		LBArt.Add(new Art("Control Panel", Lbrelease.ControlPanelURL, Lbrelease.ControlPanelPath));
			//	}

			//	if (!String.IsNullOrEmpty(Lbrelease.LogoUrl))
			//	{
			//		LBArt.Add(new Art("Clear Logo", Lbrelease.LogoUrl, Lbrelease.LogoPath));
			//	}

			//	if (!String.IsNullOrEmpty(Lbrelease.MarqueeURL))
			//	{
			//		LBArt.Add(new Art("Marquee", Lbrelease.MarqueeURL, Lbrelease.MarqueePath));
			//	}

			//}

			//if (Gdbrelease != null)
			//{
			//	if (!String.IsNullOrEmpty(Gdbrelease.BoxFrontPath))
			//	{
			//		GDBArt.Add(new Art("Banner", Gdbrelease.BoxFrontUrl, Gdbrelease.BoxFrontPath));
			//	}
			//	if (!String.IsNullOrEmpty(Gdbrelease.BoxBackPath))
			//	{
			//		GDBArt.Add(new Art("Banner", Gdbrelease.BoxBackUrl, Gdbrelease.BoxBackPath));
			//	}
			//	if (!String.IsNullOrEmpty(Gdbrelease.BannerUrl))
			//	{
			//		GDBArt.Add(new Art("Banner", Gdbrelease.BannerUrl, Gdbrelease.BannerPath));
			//	}

			//	if (!String.IsNullOrEmpty(Gdbrelease.LogoUrl))
			//	{
			//		GDBArt.Add(new Art("Clear Logo", Gdbrelease.LogoUrl, Gdbrelease.LogoPath));
			//	}

			//	if (!String.IsNullOrEmpty(Gdbrelease.))
			//	{
			//		GDBArt.Add(new Art("Marquee", Gdbrelease.MarqueeURL, Gdbrelease.MarqueePath));
			//	}

			//}


		}



	}

	
}


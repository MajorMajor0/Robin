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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	class ArtWindowViewModel
	{
		public Release Release { get; }

		public LBRelease LBRelease => Release.LBRelease;

		public GDBRelease GDBRelease => Release.GDBRelease;

		public GBRelease GBRelease => Release.GBRelease;


		public List<Art> LBArt { get; set; }

		public List<Art> GDBArt { get; set; }

		public List<Art> GBArt { get; set; }

		public List<Art> OVGArt { get; set; }


		public ArtWindowViewModel(Release release)
		{
			Release = release;
			LBArt = new List<Art>();

			Launchbox launchbox = new Launchbox();

			//R.Data.Releases.Include(x => x.LBRelease).Load();

			if (LBRelease != null)
			{
				if (!String.IsNullOrEmpty(LBRelease.BoxFrontPath))
				{
					LBArt.Add(new Art("Banner", LBRelease.BoxFrontURL, LBRelease.BoxFrontPath));
				}
				if (!String.IsNullOrEmpty(LBRelease.BoxBackPath))
				{
					LBArt.Add(new Art("Banner", LBRelease.BoxBackURL, LBRelease.BoxBackPath));
				}
				if (!String.IsNullOrEmpty(LBRelease.BannerURL))
				{
					LBArt.Add(new Art("Banner", LBRelease.BannerURL, LBRelease.BannerPath));
				}

				if (!String.IsNullOrEmpty(LBRelease.Box3DURL))
				{
					LBArt.Add(new Art("Banner", LBRelease.Box3DURL, LBRelease.Box3DPath));
				}

				if (!String.IsNullOrEmpty(LBRelease.Cart3DURL))
				{
					LBArt.Add(new Art("Banner", LBRelease.Cart3DURL, LBRelease.Cart3DPath));
				}

				if (!String.IsNullOrEmpty(LBRelease.CartBackURL))
				{
					LBArt.Add(new Art("Cartridge Back", LBRelease.CartBackURL, LBRelease.CartBackPath));
				}

				if (!String.IsNullOrEmpty(LBRelease.ControlPanelURL))
				{
					LBArt.Add(new Art("Control Panel", LBRelease.ControlPanelURL, LBRelease.ControlPanelPath));
				}

				if (!String.IsNullOrEmpty(LBRelease.LogoURL))
				{
					LBArt.Add(new Art("Clear Logo", LBRelease.LogoURL, LBRelease.LogoPath));
				}

				if (!String.IsNullOrEmpty(LBRelease.MarqueeURL))
				{
					LBArt.Add(new Art("Marquee", LBRelease.MarqueeURL, LBRelease.MarqueePath));
				}

			}

			if (GDBRelease != null)
			{
				if (!String.IsNullOrEmpty(GDBRelease.BoxFrontPath))
				{
					GDBArt.Add(new Art("Banner", GDBRelease.BoxFrontURL, GDBRelease.BoxFrontPath));
				}
				if (!String.IsNullOrEmpty(GDBRelease.BoxBackPath))
				{
					GDBArt.Add(new Art("Banner", GDBRelease.BoxBackURL, GDBRelease.BoxBackPath));
				}
				if (!String.IsNullOrEmpty(GDBRelease.BannerURL))
				{
					GDBArt.Add(new Art("Banner", GDBRelease.BannerURL, GDBRelease.BannerPath));
				}

				if (!String.IsNullOrEmpty(GDBRelease.LogoURL))
				{
					GDBArt.Add(new Art("Clear Logo", GDBRelease.LogoURL, GDBRelease.LogoPath));
				}

				if (!String.IsNullOrEmpty(GDBRelease.Marquee))
				{
					GDBArt.Add(new Art("Marquee", GDBRelease.MarqueeURL, GDBRelease.MarqueePath));
				}

			}


		}



	}

	
}


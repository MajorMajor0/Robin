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
using System.IO;
using System.Linq;
using System.Net;

namespace Robin
{
	public partial class LBRelease : IDBRelease
	{
		public LocalDB LocalDB => LocalDB.LaunchBox;

		public string RegionTitle => Region.Title;

		public string Overview => LBGame.Overview;

		public DateTime? Date => LBGame.Date;

		public string Developer => LBGame.Developer;

		public string Publisher => LBGame.Publisher;

		public string Players => LBGame.Players;

		public string BannerPath => FileLocation.Temp + "LBR-" + ID + "-BNR.jpg";
		public string Box3DPath => FileLocation.Temp + "LBR-" + ID + "-BX3.jpg";
		public string BoxBackPath => FileLocation.Temp + "LBR-" + ID + "-BXB.jpg";
		public string BoxFrontPath => FileLocation.Temp + "LBR-" + ID + "-BXF.jpg";
		public string Cart3DPath => FileLocation.Temp + "LBR-" + ID + "-C3D.jpg";
		public string CartBackPath => FileLocation.Temp + "LBR-" + ID + "-CB.jpg";
		public string CartFrontPath => FileLocation.Temp + "LBR-" + ID + "-CF.jpg";
		public string ControlPanelPath => FileLocation.Temp + "LBR-" + ID + "-BXF.jpg";
		public string LogoPath => FileLocation.Temp + "LBR-" + ID + "-LGO.jpg";
		public string MarqueePath => FileLocation.Temp + "LBR-" + ID + "-MAR.jpg";
		public string ScreenPath => FileLocation.Temp + "LBR-" + ID + "-SCR.jpg";

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

		public int ScrapeBanner()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BannerPath))
				{
					if (BannerURL != null)
					{
						Reporter.Report("Getting banner for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(BannerURL, BannerPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("BannerPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No banner URL exists for LB release " + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeBox3D()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(Box3DPath))
				{
					if (Box3DURL != null)
					{
						Reporter.Report("Getting 3D box art for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(Box3DURL, Box3DPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("Box3DPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No 3D box art URL exists for LB release " + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeBoxBack()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxBackPath))
				{
					if (BoxBackURL != null)
					{
						Reporter.Report("Getting back box art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxBackURL, BoxBackPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("BoxBackPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No back box art URL exists.");
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeBoxFront()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (BoxFrontURL != null)
					{
						Reporter.Report("Getting front box art for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxFrontURL, BoxFrontPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("BoxFrontPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No front box art URL exists for LB release " + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeCart3D()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(Cart3DPath))
				{
					if (BoxFrontURL != null)
					{
						Reporter.Report("Getting 3D cartridge art for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(Cart3DURL, Cart3DPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("Cart3DPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No 3D cartridge art URL exists for LB release" + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeCartFront()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(CartFrontPath))
				{
					if (CartFrontURL != null)
					{
						Reporter.Report("Getting front cartridge art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(CartFrontURL, CartFrontPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("CartFrontPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No front cartridge art URL exists.");
					}
				}

				else
				{
					Reporter.Report("File already exists for LB release" + Title);
				}
			}
			return 0;
		}

		public int ScrapeControlPanel()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(ControlPanelPath))
				{
					if (ControlPanelURL != null)
					{
						Reporter.Report("Getting control panel art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(ControlPanelURL, ControlPanelPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("ControlPanelPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No control panel art URL exists for LB release" + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeLogo()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(LogoPath))
				{
					if (BoxFrontURL != null)
					{
						Reporter.Report("Getting clear logo for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(LogoURL, LogoPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("LogoPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No clear logo URL exists for LB release" + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeMarquee()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(MarqueePath))
				{
					if (MarqueeURL != null)
					{
						Reporter.Report("Getting marquee art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(MarqueeURL, MarqueePath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("MarqueePath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No marquee art URL exists for LB release" + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}

		public int ScrapeScreen()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(ScreenPath))
				{
					if (ScreenURL != null)
					{
						Reporter.Report("Getting screen shot for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(ScreenURL, ScreenPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("ScreenPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}

					else
					{
						Reporter.Report("No screen shot URL exists for LB release" + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return 0;
		}
	}
}

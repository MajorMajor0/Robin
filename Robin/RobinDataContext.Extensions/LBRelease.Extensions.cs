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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net;

namespace Robin
{
	public partial class Lbrelease : IDBRelease
	{
		[NotMapped]
		public LocalDB LocalDB => LocalDB.LaunchBox;

		[NotMapped]
		public string RegionTitle => Region.Title;

		[NotMapped]
		public string Overview => Lbgame.Overview;

		[NotMapped]
		public DateTime? Date => Lbgame.Date;

		[NotMapped]
		public string Developer => Lbgame.Developer;

		[NotMapped]
		public string Publisher => Lbgame.Publisher;

		[NotMapped]
		public string Players => Lbgame.Players;

		[NotMapped]
		public string BannerPath => FileLocation.Temp + "LBR-" + Id + "-BNR.jpg";

		[NotMapped] 
		public string Box3DPath => FileLocation.Temp + "LBR-" + Id + "-BX3.jpg";

		[NotMapped] 
		public string BoxBackPath => FileLocation.Temp + "LBR-" + Id + "-BXB.jpg";
		[NotMapped]
		public string BoxFrontPath => FileLocation.Temp + "LBR-" + Id + "-BXF.jpg";
		[NotMapped]
		public string Cart3DPath => FileLocation.Temp + "LBR-" + Id + "-C3D.jpg";
		[NotMapped]
		public string CartBackPath => FileLocation.Temp + "LBR-" + Id + "-CB.jpg";
		[NotMapped]
		public string CartFrontPath => FileLocation.Temp + "LBR-" + Id + "-CF.jpg";
		[NotMapped]
		public string ControlPanelPath => FileLocation.Temp + "LBR-" + Id + "-BXF.jpg";
		[NotMapped]
		public string LogoPath => FileLocation.Temp + "LBR-" + Id + "-LGO.jpg";
		[NotMapped]
		public string MarqueePath => FileLocation.Temp + "LBR-" + Id + "-MAR.jpg";
		[NotMapped]
		public string ScreenPath => FileLocation.Temp + "LBR-" + Id + "-SCR.jpg";

		[NotMapped]
		public string BannerUrl => GetURL("Banner");
		[NotMapped]
		public string Box3DUrl => GetURL("Box - 3D");
		[NotMapped]
		public string BoxBackUrl => GetURL("Box - Back");
		[NotMapped]
		public string BoxFrontUrl => GetURL("Box - Front");
		[NotMapped]
		public string Cart3DUrl => GetURL("Cart - 3D");
		[NotMapped]
		public string CartBackUrl => GetURL("Cart - Back");
		[NotMapped]
		public string CartFrontUrl => GetURL("Cart - Front");
		[NotMapped]
		public string ControlInformationUrl => GetURL("Arcade - Controls Information");
		[NotMapped]
		public string ControlPanelUrl => GetURL("Arcade - Control Panel");
		[NotMapped]
		public string LogoUrl => GetURL("Clear Logo");
		[NotMapped]
		public string MarqueeUrl => GetURL("Arcade - Marquee");
		[NotMapped]
		public string ScreenUrl => GetURL("Screenshot - Gameplay");

		string GetURL(string type)
		{
			Lbimage Lbimage = Lbimages.FirstOrDefault(x => x.Type == type);

			if (Lbimage != null)
			{
				return Launchbox.IMAGESURL + Lbimage.FileName;
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
					if (BannerUrl != null)
					{
						Reporter.Report("Getting banner for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(BannerUrl, BannerPath))
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
					if (Box3DUrl != null)
					{
						Reporter.Report("Getting 3D box art for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(Box3DUrl, Box3DPath))
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
					if (BoxBackUrl != null)
					{
						Reporter.Report("Getting back box art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxBackUrl, BoxBackPath))
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
					if (BoxFrontUrl != null)
					{
						Reporter.Report("Getting front box art for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxFrontUrl, BoxFrontPath))
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
					if (BoxFrontUrl != null)
					{
						Reporter.Report("Getting 3D cartridge art for LB release " + Title + "...");

						if (webclient.DownloadFileFromDB(Cart3DUrl, Cart3DPath))
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
					if (CartFrontUrl != null)
					{
						Reporter.Report("Getting front cartridge art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(CartFrontUrl, CartFrontPath))
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
					if (ControlPanelUrl != null)
					{
						Reporter.Report("Getting control panel art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(ControlPanelUrl, ControlPanelPath))
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
					if (BoxFrontUrl != null)
					{
						Reporter.Report("Getting clear logo for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(LogoUrl, LogoPath))
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
					if (MarqueeUrl != null)
					{
						Reporter.Report("Getting marquee art for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(MarqueeUrl, MarqueePath))
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
					if (ScreenUrl != null)
					{
						Reporter.Report("Getting screen shot for LB Release " + Title + "...");

						if (webclient.DownloadFileFromDB(ScreenUrl, ScreenPath))
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}

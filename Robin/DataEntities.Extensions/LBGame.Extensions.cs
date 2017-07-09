using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public partial class LBGame : IDBRelease
	{
		public static List<LBGame> GetGames(Platform platform)
		{
			R.Data.LBGames.Load();
			R.Data.LBGames.Include(x => x.LBImages).Load();
			R.Data.Regions.Load();
			return R.Data.LBGames.Where(x => x.LBPlatform_ID == platform.ID_LB).ToList();
		}

		public string RegionTitle
		{
			get; set;
		}

		public Region Region { get { return null; } }

		public string Regions
		{
			get
			{
				if (LBImages != null)
				{
					return string.Join(", ", LBImages.Select(x => x.Region.Title).Distinct());
				}
				return null;
			}
		}

		public string BoxFrontURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Front"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Front"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string BoxBackURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Back"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Back"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string Box3DURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - 3D"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - 3D"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string LogoURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Clear Logo"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Clear Logo"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string BannerURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Banner"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Banner"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string MarqueeURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Marquee"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Marquee"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string ControlPanelURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Control Panel"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Control Panel"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string ControlInformationURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Controls Information"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Controls Information"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string CartFrontURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Front"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Front"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string CartBackURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Back"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Back"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string Cart3DURL(long? regionID)
		{
			LBImage lbImage = null;
			if (regionID != null)
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - 3D"));
			}

			else
			{
				lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - 3D"));
			}

			if (lbImage != null)
			{
				return Launchbox.IMAGESURL + lbImage.FileName;
			}

			else
			{
				return null;
			}
		}

		public string BoxFrontPath
		{
			get { return FileLocation.Temp + ID + "LBR-BXF.jpg"; }
		}

		public void ScrapeBoxFront()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (BoxFrontURL(null) != null)
					{
						Reporter.Report("Getting front box art for LBGame " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxFrontURL(null), BoxFrontPath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("BoxFrontPath");
						}
						else
						{
							Reporter.ReportInline("dammit!");
						}
					}

					else
					{
						Reporter.Report("No box art URL exists.");
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
		}
	}
}

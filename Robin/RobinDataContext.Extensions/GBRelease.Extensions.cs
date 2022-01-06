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

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net;

namespace Robin
{
	public partial class Gbrelease : IDBRelease
	{
		[NotMapped]
		public LocalDB LocalDB => LocalDB.GiantBomb;

		[NotMapped]
		public string RegionTitle => Region.Title;

		[NotMapped]
		public string BoxFrontPath => FileLocation.Temp + Id + "GBR-BXF.jpg";

		[NotMapped]
		public string ScreenPath => FileLocation.Temp + Id + "GBR-SCR.jpg";

		[NotMapped]
		public string BannerPath => null;


		public static List<Gbrelease> GetGames(Platform platform)
		{
			R.Data.Gbreleases.Load();
			R.Data.Regions.Load();
			return R.Data.Gbreleases.Where(x => x.GbplatformId == platform.ID_GB).ToList();
		}

		public int ScrapeBoxFront()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (BoxUrl != null)
					{
						Reporter.Report("Getting front box art for Gbrelease " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxUrl, BoxFrontPath))
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
						Reporter.Report("No box art URL exists.");
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
			throw new NotImplementedException();
		}

		public int ScrapeBox3D()
		{
			throw new NotImplementedException();
		}

		public int ScrapeScreen()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(ScreenPath))
				{
					if (ScreenUrl != null)
					{
						Reporter.Report("Getting screen shot for Gbrelease " + Title + "...");

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
						Reporter.Report("No screen shot URL exists.");
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
			throw new NotImplementedException();
		}

		public int ScrapeBanner()
		{
			throw new NotImplementedException();
		}

		public int ScrapeCartFront()
		{
			throw new NotImplementedException();
		}

		public int ScrapeCart3D()
		{
			throw new NotImplementedException();
		}

		public int ScrapeMarquee()
		{
			throw new NotImplementedException();
		}

		public int ScrapeControlPanel()
		{
			throw new NotImplementedException();
		}

	}
}

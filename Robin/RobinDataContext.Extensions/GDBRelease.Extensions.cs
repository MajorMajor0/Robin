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
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Net;

namespace Robin
{
	public partial class Gdbrelease
	{
		[NotMapped]
		public LocalDB LocalDB => LocalDB.GamesDB;

		[NotMapped]
		public string RegionTitle => null;

		[NotMapped]
		public string BoxArtFrontThumbURL
		{
			get
			{
				if (BoxFrontUrl == null)
				{
					return null;
				}
				else
				{
					return BoxFrontUrl.Replace(@"/boxart", @"/boxart/thumb");
				}
			}
		}

		[NotMapped]
		public string BoxArtBackThumbURL
		{
			get
			{
				if (BoxBackUrl == null)
				{
					return null;
				}
				else
				{
					return BoxBackUrl.Replace(@"/boxart", @"/boxart/thumb");
				}
			}
		}

		[NotMapped]
		public Region Region => null;

		[NotMapped]
		public string BoxFrontPath => FileLocation.Temp + "GDBR-" + ID + "-BXF.jpg";

		[NotMapped]
		public string BoxBackPath => FileLocation.Temp + "GDBR-" + ID + "-BXB.jpg";

		[NotMapped]
		public string ScreenPath => FileLocation.Temp + "GDBR-" + ID + "-SCR.jpg";

		[NotMapped]
		public string BannerPath => FileLocation.Temp + "GDBR-" + ID + "-BNR.jpg";

		[NotMapped]
		public string LogoPath => FileLocation.Temp + "GDBR-" + ID + "-LGO.jpg";

		public int ScrapeBox3D()
		{
			throw new NotImplementedException();
		}

		public int ScrapeBoxFront()
		{
			using (WebClient webclient = new())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (BoxFrontUrl != null)
					{
						Reporter.Report("Getting front box art for Gdbrelease " + Title + "...");

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
						Reporter.Report("No front box art URL exists.");
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
			using (WebClient webclient = new())
			{
				if (!File.Exists(BoxBackPath))
				{
					if (BoxBackUrl != null)
					{
						Reporter.Report("Getting back box art for Gdbrelease " + Title + "...");

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

		public int ScrapeBanner()
		{
			using (WebClient webclient = new())
			{
				if (!File.Exists(BannerPath))
				{
					if (BannerUrl != null)
					{
						Reporter.Report("Getting banner for Gdbrelease " + Title + "...");

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
						Reporter.Report("No banner URL exists.");
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
			using (WebClient webclient = new())
			{
				if (!File.Exists(ScreenPath))
				{
					if (ScreenUrl != null)
					{
						Reporter.Report("Getting screen shot for Gdbrelease " + Title + "...");

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
			using (WebClient webclient = new())
			{
				if (!File.Exists(LogoPath))
				{
					if (LogoUrl != null)
					{
						Reporter.Report("Getting clear logo for Gdbrelease " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxFrontUrl, BoxFrontPath))
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
						Reporter.Report("No clear logo URL exists.");
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
			throw new NotImplementedException();
		}

		public int ScrapeCart3D()
		{
			throw new NotImplementedException();
		}

		public int ScrapeControlPanel()
		{
			throw new NotImplementedException();
		}

		public int ScrapeMarquee()
		{
			throw new NotImplementedException();
		}

	}
}

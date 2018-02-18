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
		public LocalDB LocalDB => LocalDB.GamesDB;

		public string RegionTitle => null;

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


		public Region Region => null;

		public string BoxFrontPath => FileLocation.Temp + "GDBR-" + ID + "-BXF.jpg";

		public string BoxBackPath => FileLocation.Temp + "GDBR-" + ID + "-BXB.jpg";

		public string ScreenPath => FileLocation.Temp + "GDBR-" + ID + "-SCR.jpg";

		public string BannerPath => FileLocation.Temp + "GDBR-" + ID + "-BNR.jpg";

		public string LogoPath => FileLocation.Temp + "GDBR-" + ID + "-LGO.jpg";

		public int ScrapeBox3D()
		{
			throw new NotImplementedException();
		}

		public int ScrapeBoxFront()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (BoxFrontURL != null)
					{
						Reporter.Report("Getting front box art for GDBRelease " + Title + "...");

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
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxBackPath))
				{
					if (BoxBackURL != null)
					{
						Reporter.Report("Getting back box art for GDBRelease " + Title + "...");

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

		public int ScrapeBanner()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BannerPath))
				{
					if (BannerURL != null)
					{
						Reporter.Report("Getting banner for GDBRelease " + Title + "...");

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
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(ScreenPath))
				{
					if (ScreenURL != null)
					{
						Reporter.Report("Getting screen shot for GDBRelease " + Title + "...");

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
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(LogoPath))
				{
					if (LogoURL != null)
					{
						Reporter.Report("Getting clear logo for GDBRelease " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxFrontURL, BoxFrontPath))
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

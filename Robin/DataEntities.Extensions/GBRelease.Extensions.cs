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
using System.IO;
using System.Linq;
using System.Net;

namespace Robin
{
	public partial class GBRelease : IDBRelease
	{
		public static List<GBRelease> GetGames(Platform platform)
		{
			R.Data.GBReleases.Load();
			R.Data.Regions.Load();
			return R.Data.GBReleases.Where(x => x.GBPlatform_ID == platform.ID_GB).ToList();
		}

		public string RegionTitle
		{
			get { return Region.Title; }
		}

		public string BoxFrontPath
		{
			get { return FileLocation.Temp + ID + "GBR-BXF.jpg"; }
		}

        public string ScreenPath
        {
            get { return FileLocation.Temp + ID + "GBR-SCR.jpg"; }
        }

        public int ScrapeBoxFront()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (BoxURL != null)
					{
						Reporter.Report("Getting front box art for GBRelease " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxURL, BoxFrontPath))
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
                    if (ScreenURL != null)
                    {
                        Reporter.Report("Getting screen shot for GBRelease " + Title + "...");

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

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

using System.IO;
using System.Net;

namespace Robin
{
	public partial class GDBRelease : IDBRelease
	{
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

		public string RegionTitle
		{
			get
			{
				return null;
			}
		}

		public Region Region
		{
			get { return null; }
		}


		public string BoxFrontPath
		{
			get { return FileLocation.Temp + ID + "GDBR-BXF.jpg"; }
		}

		public void ScrapeBoxFront()
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

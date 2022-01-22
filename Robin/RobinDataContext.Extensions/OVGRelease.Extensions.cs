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
using System.Net;

namespace Robin
{
	public partial class Ovgrelease : IDbRelease
	{
		[NotMapped]
		public LocalDB LocalDB => LocalDB.OpenVGDB;

		[NotMapped]
		public string RegionTitle => Region.Title;

		[NotMapped]
		public string BoxFrontPath => FileLocation.Temp + "OVGR-" + Id + "-BXF.jpg";

		[NotMapped]
		public string BoxBackPath => FileLocation.Temp + "OVGR-" + Id + "-BXB.jpg";

		[NotMapped]
		public string BannerPath => null;

		public static implicit operator Ovgrelease(VGDBRELEAS vGdbrelease)
		{
			Ovgrelease Ovgrelease = new Ovgrelease()
			{
				RegionId = vGdbrelease.regionLocalizedID,
				Title = string.IsNullOrEmpty(vGdbrelease.releaseTitleName) ? null : vGdbrelease.releaseTitleName,
				Overview = string.IsNullOrEmpty(vGdbrelease.releaseDescription) ? null : vGdbrelease.releaseDescription,
				Developer = string.IsNullOrEmpty(vGdbrelease.releaseDeveloper) ? null : vGdbrelease.releaseDeveloper,
				Publisher = string.IsNullOrEmpty(vGdbrelease.releasePublisher) ? null : vGdbrelease.releasePublisher,
				Genre = string.IsNullOrEmpty(vGdbrelease.releaseGenre) ? null : vGdbrelease.releaseGenre,
				Date = DateTimeRoutines.SafeGetDate(string.IsNullOrEmpty(vGdbrelease.releaseDate) ? null : vGdbrelease.releaseDate),

				Crc = string.IsNullOrEmpty(vGdbrelease.VGDBROM.romHashCRC) ? null : vGdbrelease.VGDBROM.romHashCRC,
				Md5 = string.IsNullOrEmpty(vGdbrelease.VGDBROM.romHashMd5) ? null : vGdbrelease.VGDBROM.romHashMd5,
				Sha1 = string.IsNullOrEmpty(vGdbrelease.VGDBROM.romHashSha1) ? null : vGdbrelease.VGDBROM.romHashSha1,
				Size = vGdbrelease.VGDBROM.romSize?.ToString(),
				Header = string.IsNullOrEmpty(vGdbrelease.VGDBROM.romHeader) ? null : vGdbrelease.VGDBROM.romHeader,
				Language = string.IsNullOrEmpty(vGdbrelease.VGDBROM.romLanguage) ? null : vGdbrelease.VGDBROM.romLanguage,
				Serial = string.IsNullOrEmpty(vGdbrelease.VGDBROM.romSerial) ? null : vGdbrelease.VGDBROM.romSerial,

				BoxFrontUrl = string.IsNullOrEmpty(vGdbrelease.releaseCoverFront) ? null : vGdbrelease.releaseCoverFront,
				BoxBackUrl = string.IsNullOrEmpty(vGdbrelease.releaseCoverBack) ? null : vGdbrelease.releaseCoverBack,
				ReferenceUrl = string.IsNullOrEmpty(vGdbrelease.releaseReferenceURL) ? null : vGdbrelease.releaseReferenceURL,
				ReferenceImageUrl = string.IsNullOrEmpty(vGdbrelease.releaseReferenceImageURL) ? null : vGdbrelease.releaseReferenceImageURL
			};
			return Ovgrelease;
		}

		public int ScrapeBoxFront()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (BoxFrontUrl != null)
					{
						Reporter.Report("Getting front box art for Ovgrelease " + Title + "...");

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
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxBackPath))
				{
					if (BoxFrontUrl != null)
					{
						Reporter.Report("Getting front box art for Ovgrelease " + Title + "...");

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

		public int ScrapeBox3D()
		{
			throw new NotImplementedException();
		}

		public int ScrapeScreen()
		{
			throw new NotImplementedException();
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

}

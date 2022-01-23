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

namespace Robin;

public partial class OVGRelease : IDbRelease
{
	[NotMapped]
	public LocalDB LocalDB => LocalDB.OpenVGDB;

	[NotMapped]
	public string RegionTitle => Region.Title;

	[NotMapped]
	public string BoxFrontPath => FileLocation.Temp + "OVGR-" + ID + "-BXF.jpg";

	[NotMapped]
	public string BoxBackPath => FileLocation.Temp + "OVGR-" + ID + "-BXB.jpg";

	[NotMapped]
	public string BannerPath => null;

	public static implicit operator OVGRelease(VGDBRELEAS vGDBRelease)
	{
		OVGRelease OVGRelease = new()
		{
			Region_ID = vGDBRelease.regionLocalizedID,
			Title = string.IsNullOrEmpty(vGDBRelease.releaseTitleName) ? null : vGDBRelease.releaseTitleName,
			Overview = string.IsNullOrEmpty(vGDBRelease.releaseDescription) ? null : vGDBRelease.releaseDescription,
			Developer = string.IsNullOrEmpty(vGDBRelease.releaseDeveloper) ? null : vGDBRelease.releaseDeveloper,
			Publisher = string.IsNullOrEmpty(vGDBRelease.releasePublisher) ? null : vGDBRelease.releasePublisher,
			Genre = string.IsNullOrEmpty(vGDBRelease.releaseGenre) ? null : vGDBRelease.releaseGenre,
			Date = DateTimeRoutines.SafeGetDate(string.IsNullOrEmpty(vGDBRelease.releaseDate) ? null : vGDBRelease.releaseDate),

			Crc = string.IsNullOrEmpty(vGDBRelease.VGDBROM.romHashCRC) ? null : vGDBRelease.VGDBROM.romHashCRC,
			MD5 = string.IsNullOrEmpty(vGDBRelease.VGDBROM.romHashMD5) ? null : vGDBRelease.VGDBROM.romHashMD5,
			SHA1 = string.IsNullOrEmpty(vGDBRelease.VGDBROM.romHashSHA1) ? null : vGDBRelease.VGDBROM.romHashSHA1,
			Size = vGDBRelease.VGDBROM.romSize?.ToString(),
			Header = string.IsNullOrEmpty(vGDBRelease.VGDBROM.romHeader) ? null : vGDBRelease.VGDBROM.romHeader,
			Language = string.IsNullOrEmpty(vGDBRelease.VGDBROM.romLanguage) ? null : vGDBRelease.VGDBROM.romLanguage,
			Serial = string.IsNullOrEmpty(vGDBRelease.VGDBROM.romSerial) ? null : vGDBRelease.VGDBROM.romSerial,

			BoxFrontUrl = string.IsNullOrEmpty(vGDBRelease.releaseCoverFront) ? null : vGDBRelease.releaseCoverFront,
			BoxBackUrl = string.IsNullOrEmpty(vGDBRelease.releaseCoverBack) ? null : vGDBRelease.releaseCoverBack,
			ReferenceUrl = string.IsNullOrEmpty(vGDBRelease.releaseReferenceURL) ? null : vGDBRelease.releaseReferenceURL,
			ReferenceImageUrl = string.IsNullOrEmpty(vGDBRelease.releaseReferenceImageURL) ? null : vGDBRelease.releaseReferenceImageURL
		};
		return OVGRelease;
	}

	public int ScrapeBoxFront()
	{
		using (WebClient webclient = new())
		{
			if (!File.Exists(BoxFrontPath))
			{
				if (BoxFrontUrl != null)
				{
					Reporter.Report("Getting front box art for OVGRelease " + Title + "...");

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
				if (BoxFrontUrl != null)
				{
					Reporter.Report("Getting front box art for OVGRelease " + Title + "...");

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

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

namespace Robin
{
	public partial class OVGRelease
	{
		public static implicit operator OVGRelease(VGDBRELEAS vgdbrelease)
		{
			OVGRelease ovgrelease = new OVGRelease();
			ovgrelease.Region_ID = vgdbrelease.regionLocalizedID;
			ovgrelease.Title = string.IsNullOrEmpty(vgdbrelease.releaseTitleName) ? null : vgdbrelease.releaseTitleName;
			ovgrelease.Overview = string.IsNullOrEmpty(vgdbrelease.releaseDescription) ? null : vgdbrelease.releaseDescription;
			ovgrelease.Developer = string.IsNullOrEmpty(vgdbrelease.releaseDeveloper) ? null : vgdbrelease.releaseDeveloper;
			ovgrelease.Publisher = string.IsNullOrEmpty(vgdbrelease.releasePublisher) ? null : vgdbrelease.releasePublisher;
			ovgrelease.Genre = string.IsNullOrEmpty(vgdbrelease.releaseGenre) ? null : vgdbrelease.releaseGenre;
			ovgrelease.Date = string.IsNullOrEmpty(vgdbrelease.releaseDate) ? null : vgdbrelease.releaseDate;

			ovgrelease.CRC = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHashCRC) ? null : vgdbrelease.VGDBROM.romHashCRC;
			ovgrelease.MD5 = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHashMD5) ? null : vgdbrelease.VGDBROM.romHashMD5;
			ovgrelease.SHA1 = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHashSHA1) ? null : vgdbrelease.VGDBROM.romHashSHA1;
			ovgrelease.Size = vgdbrelease.VGDBROM.romSize == null ? null: vgdbrelease.VGDBROM.romSize.ToString();
			ovgrelease.Header = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHeader) ? null : vgdbrelease.VGDBROM.romHeader;
			ovgrelease.Language = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romLanguage) ? null : vgdbrelease.VGDBROM.romLanguage;
			ovgrelease.Serial = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romSerial) ? null : vgdbrelease.VGDBROM.romSerial;

			ovgrelease.BoxFrontURL = string.IsNullOrEmpty(vgdbrelease.releaseCoverFront) ? null : vgdbrelease.releaseCoverFront;
			ovgrelease.BoxBackURL = string.IsNullOrEmpty(vgdbrelease.releaseCoverBack) ? null : vgdbrelease.releaseCoverBack;
			ovgrelease.ReferenceURL = string.IsNullOrEmpty(vgdbrelease.releaseReferenceURL) ? null : vgdbrelease.releaseReferenceURL;
			ovgrelease.ReferenceImageURL = string.IsNullOrEmpty(vgdbrelease.releaseReferenceImageURL) ? null : vgdbrelease.releaseReferenceImageURL;

			return ovgrelease;
		}
	}

}

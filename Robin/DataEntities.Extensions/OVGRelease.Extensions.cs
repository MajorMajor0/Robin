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
    public partial class OVGRelease : IDBRelease
    {
        public string RegionTitle { get { return Region.Title; } }

        const string typeString = "OVGR";

        public static implicit operator OVGRelease(VGDBRELEAS vgdbrelease)
        {
            OVGRelease ovgrelease = new OVGRelease()
            {
                Region_ID = vgdbrelease.regionLocalizedID,
                Title = string.IsNullOrEmpty(vgdbrelease.releaseTitleName) ? null : vgdbrelease.releaseTitleName,
                Overview = string.IsNullOrEmpty(vgdbrelease.releaseDescription) ? null : vgdbrelease.releaseDescription,
                Developer = string.IsNullOrEmpty(vgdbrelease.releaseDeveloper) ? null : vgdbrelease.releaseDeveloper,
                Publisher = string.IsNullOrEmpty(vgdbrelease.releasePublisher) ? null : vgdbrelease.releasePublisher,
                Genre = string.IsNullOrEmpty(vgdbrelease.releaseGenre) ? null : vgdbrelease.releaseGenre,
                Date = DateTimeRoutines.SafeGetDate(string.IsNullOrEmpty(vgdbrelease.releaseDate) ? null : vgdbrelease.releaseDate),

                CRC = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHashCRC) ? null : vgdbrelease.VGDBROM.romHashCRC,
                MD5 = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHashMD5) ? null : vgdbrelease.VGDBROM.romHashMD5,
                SHA1 = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHashSHA1) ? null : vgdbrelease.VGDBROM.romHashSHA1,
                Size = vgdbrelease.VGDBROM.romSize?.ToString(),
                Header = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romHeader) ? null : vgdbrelease.VGDBROM.romHeader,
                Language = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romLanguage) ? null : vgdbrelease.VGDBROM.romLanguage,
                Serial = string.IsNullOrEmpty(vgdbrelease.VGDBROM.romSerial) ? null : vgdbrelease.VGDBROM.romSerial,

                BoxFrontURL = string.IsNullOrEmpty(vgdbrelease.releaseCoverFront) ? null : vgdbrelease.releaseCoverFront,
                BoxBackURL = string.IsNullOrEmpty(vgdbrelease.releaseCoverBack) ? null : vgdbrelease.releaseCoverBack,
                ReferenceURL = string.IsNullOrEmpty(vgdbrelease.releaseReferenceURL) ? null : vgdbrelease.releaseReferenceURL,
                ReferenceImageURL = string.IsNullOrEmpty(vgdbrelease.releaseReferenceImageURL) ? null : vgdbrelease.releaseReferenceImageURL
            };
            return ovgrelease;
        }

        public Banner Banner => null;

        public Box3D Box3D => null;

        BoxBack boxBack;
        public BoxBack BoxBack
        {
            get
            {
                if (boxBack == null && BoxBackURL != null)
                {
                    boxBack = new BoxBack(BoxBackURL);
                }
                return boxBack;
            }
        }

        BoxFront boxFront;
        public BoxFront BoxFront
        {
            get
            {
                if (boxFront == null && BoxFrontURL != null)
                {
                    boxFront = new BoxFront(BoxFrontURL);
                }
                return boxFront;
            }
        }

        public Cart3D Cart3D => null;

        public CartBack CartBack => null;

        public CartFront CartFront => null;

        public ControlInformation ControlInformation => null;

        public ControlPanel ControlPanel => null;

        public Logo Logo => null;

        public Marquee Marquee => null;

        public Screen Screen => null;
    }

}

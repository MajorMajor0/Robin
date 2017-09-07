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

using System.Text.RegularExpressions;

namespace Robin
{
	public partial class VGDBROM
	{
	    public string AtariTitle => romFileName.Split(new[] {" ("}, 0)[0];

	    public string AtariParentTitle => AtariTitle.Split(new[] {" - "}, 0)[0];

	    public string AKA => Regex.Match(romFileName, @"(?<=\(AKA\ )(.*?)(?=\))").Value;

	    public static implicit operator Rom(VGDBROM vgdbRom)
		{
			Rom rom = new Rom();
			rom.Platform_ID = vgdbRom.systemID;
			rom.CRC32 = vgdbRom.romHashCRC;
			rom.MD5 = vgdbRom.romHashMD5;
			rom.SHA1 = vgdbRom.romHashSHA1;
			rom.Size = vgdbRom.romSize.ToString();
			rom.Title = vgdbRom.romExtensionlessFileName;
			rom.Source = "OpenVGDB";

			return rom;
		}
	}

}

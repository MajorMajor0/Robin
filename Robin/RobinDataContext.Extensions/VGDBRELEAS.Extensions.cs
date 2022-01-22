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
    public partial class VGDBRELEAS
    {
        public static implicit operator Release(VGDBRELEAS vGdbrelease)
        {
            Release release = new();
            release.PlatformId = vGdbrelease.VGDBROM.systemID;
            release.RegionId = vGdbrelease.regionLocalizedID ?? 0;
            release.Title = vGdbrelease.releaseTitleName;
            release.IsGame = true;

            return release;
        }
    }
}

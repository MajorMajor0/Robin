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

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Robin
{
	public partial class Rom
	{

		[NotMapped]
		public bool IsBios => Regex.IsMatch(Title, @"\[BIOS\]");

		[NotMapped]
		public string FilePath => Platform.RomDirectory + FileName;

		[NotMapped]
		public decimal PlatformId => Releases[0].PlatformId;

		[NotMapped]
		public Platform Platform => Releases[0].Platform;

		public void StoreFileName(string extension)
		{
			if (PlatformId != CONSTANTS.PlatformId.Arcade)
			{
				string washed = Regex.Replace(Title, @"\A(A |The |La |El )", "");

				washed = washed.Replace("IV", "4").Replace("III", "3").Replace("II", "2").
			   Replace(", A", "").Replace(", The", "").Replace(", An", "").Replace(", La", "").Replace(", El", "").ToLower();

			   washed = Regex.Replace(washed, @"(!|@|#|\$|%|\^|&|\*|\(|\)|-|_|\+|=|\{|\}|\[|\]|\||\\|:|;|'|\<|,|\>|\?|/|\.| |)", "");

			   FileName = washed + Platform.Abbreviation + extension;
			}
		}
	}
}


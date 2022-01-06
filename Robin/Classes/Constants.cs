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
	public struct CONSTANTS
	{
		public const string VERSION = @"Robin 0.0";

		public const string HEADER_ACCEPT = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

		public const string HEADER_ACCEPT_ENCODING = @"gzip, deflate, br";

		public const string HEADER_ACCEPT_LANGUAGE = "en-US,en;q=0.5";

		public struct PlatformId
		{
			public const long Arcade = 1;
			public const long Lynx = 8;
			public const long ChannelF = 15;
		}

		public struct EmulatorId
		{
			public const long Mame = 5;
			public const long Retroarch = 18;
			public const long Higan = 4;
		}

		public struct RegionId
		{
			public const long World = 22;
			public const long Unk = 0;
		}
	}
}

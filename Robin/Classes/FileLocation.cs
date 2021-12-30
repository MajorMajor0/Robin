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
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Robin
{
	public static class FileLocation
	{
		public static string Folder { get; } = Path
			.GetDirectoryName(Path
			.GetDirectoryName(Assembly
				.GetEntryAssembly()
				.Location));
		public static class Art
		{
			public static string Folder { get; } = FileLocation.Folder + @"\Art\";
			public static string BoxFront { get; } = Folder + @"BoxFront\";
			public static string BoxFrontThumbs { get; } = Folder + @"BoxFront\Thumbs\";
			public static string BoxBack { get; } = Folder + @"BoxBack\";
			public static string Cabinet { get; } = Folder + @"Cabinet\";
			public static string Banner { get; } = Folder + @"Banner\";
			public static string Flyer { get; } = Folder + @"Flyer\";
			public static string Logo { get; } = Folder + @"Logo\";
			public static string Screen { get; } = Folder + @"Screen\";
			public static string Console { get; } = Folder + @"Console\";
		}

		public static string Images { get; } = Folder + @"\Images\";

		public static string Roms { get; } = Folder + @"\Roms\";

		public static string RomsBackup { get; } = Folder + @"\Roms\Backup\";

		public static string HiganRoms { get; } = Roms + @"Higan\";

		public static string Emulators { get; } = Folder + @"\Emulators\";

		public static string SaveStates { get; } = Folder + @"\SaveStates\";

		public static string Backup { get; } = Folder + @"\data\backup\";

		public static string MAME { get; } = Folder + @"\Emulators\MAME\mame64.exe";

		public static string Marquee { get; } = Folder + @"\Emulators\MAME\marquees\";

		//public static string Data = Folder + @"\Data\";
		public static string Data { get; } = @"..\data\";

		public static string RobinData { get; } = Data + @"RobinData.db3";

		public static string MameData { get; } = Data + @"MAME.db3";

		public static string Temp { get; } = Folder + @"\Temp\";

		public static string HandyConverter { get; } = Emulators + @"Handy\make_lnx.exe";

		public static string Flags { get; } = Images + @"Flags\";

		public static void CreateDirectories()
		{
			Directory.CreateDirectory(Art.BoxFront);
			Directory.CreateDirectory(Art.BoxFrontThumbs);
			Directory.CreateDirectory(Art.BoxBack);
			Directory.CreateDirectory(Art.Screen);
			Directory.CreateDirectory(Art.Banner);
			Directory.CreateDirectory(Art.Logo);
			Directory.CreateDirectory(Art.Console);
		}

		static FileLocation()
		{
			Debug.WriteLine($"Launched from {Environment.CurrentDirectory}");
			Debug.WriteLine($"Physical location {AppDomain.CurrentDomain.BaseDirectory}");
			Debug.WriteLine($"AppContext.BaseDir {AppContext.BaseDirectory}");
			Debug.WriteLine($"Runtime Call {Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}");
		}
	}
}

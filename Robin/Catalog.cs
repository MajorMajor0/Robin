using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Robin
{
	public static class Catalog
	{
		public static HashSet<string> Art { get; set; }
		public static HashSet<string> Roms { get; set; }
		public static HashSet<string> Emulators { get; set; }
		public static HashSet<string> Images { get; set; }

		static Catalog()
		{
			Stopwatch watch = Stopwatch.StartNew();
			var artFiles = Directory.GetFiles(FileLocation.Art.Folder, "", SearchOption.AllDirectories);
			Art = artFiles.ToHashSet();
			watch.Stop();
			Debug.WriteLine($"Art hashset: {watch.ElapsedMilliseconds}\n");


			watch.Restart();
			var romFiles = Directory.GetFiles(FileLocation.Roms, "", SearchOption.AllDirectories);
			Roms = romFiles.ToHashSet();
			watch.Stop();
			Debug.WriteLine($"Rom hashset: {watch.ElapsedMilliseconds}\n");


			watch.Restart();
			var emuFiles = Directory.GetFiles(FileLocation.Emulators, "*.exe", SearchOption.AllDirectories);
			Emulators = emuFiles.ToHashSet();
			watch.Stop();
			Debug.WriteLine($"Emulator hashset: {watch.ElapsedMilliseconds}\n");


			watch.Restart();
			var imageFiles = Directory.GetFiles(FileLocation.Images, "", SearchOption.AllDirectories);
			Images = imageFiles.ToHashSet();
			watch.Stop();
			Debug.WriteLine($"Image hashset: {watch.ElapsedMilliseconds}\n");
		}
	}
}

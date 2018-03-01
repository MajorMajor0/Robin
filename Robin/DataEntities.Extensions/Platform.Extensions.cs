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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Robin

{
	public partial class Platform : IDBobject, IDBPlatform
	{
		public IEnumerable<Game> Games => R.Data.Games.Local.Where(x => x.Platform_ID == ID);

		public Platform RPlatform => this;

		public OVGPlatform OVGPlatform => R.Data.OVGPlatforms.FirstOrDefault(x => x.ID == ID);

		public int MatchedToGamesDB => Releases.Count(x => x.ID_GDB != null);

		public int MatchedToGiantBomb => Releases.Count(x => x.ID_GB != null);

		public int MatchedToOpenVG => Releases.Count(x => x.ID_OVG != null);

		public int MatchedToLaunchBox => Releases.Count(x => x.ID_LB != null);

		public int MatchedReleaseCount => Releases.Count(x => x.MatchedToSomething);

		public int MatchedGameCount => Games.Count(x => x.MatchedToSomething);

		public int ReleasesWithArt => Releases.Count(x => x.HasArt);

		public int GamesesWithArt => Games.Count(x => x.HasArt);

		public int ReleasesIncluded => Releases.Count(x => x.Included);


		public bool Included => HasEmulator && HasRelease;

		public bool HasEmulator => Emulators.Any(x => x.Included);

		public bool HasRelease => Releases.Any(x => x.Included);

		public List<Rom> Roms => R.Data.Roms.Where(x => x.Platform_ID == ID).ToList();

		public string WhyCantIPlay
		{
			get
			{
				if (Included)
				{
					return Title + " is ready to play.";
				}
				string and = HasRelease || HasEmulator ? "" : " and ";
				string emulatorTrouble = HasEmulator ? "" : "no emulator appears to be installed for it";
				string releaseTrouble = HasRelease ? "" : "no rom files appear to be available";
				return Title + " can't launch because " + releaseTrouble + and + emulatorTrouble + ".";
			}
		}

		public bool HasArt => File.Exists(BoxFrontPath);

		public bool IsCrap { get; set; }

		public bool Unlicensed => false;

		public bool Preferred { get; set; }//TODO: 


		public string MainDisplay => ConsolePath;

		public string RomDirectory => FileLocation.Roms + FileName + @"\";

		public string BoxFrontURL => GDBPlatform != null ? GDBPlatform.BoxFrontURL : null;

		public string BoxBackURL => GDBPlatform != null ? GDBPlatform.BoxBackURL : null;

		public string BannerURL => GDBPlatform != null ? GDBPlatform.BannerURL : null;

		public string ConsoleURL => GDBPlatform != null ? GDBPlatform.ConsoleURL : null;

		public string ControllerURL => GDBPlatform != null ? GDBPlatform.ControllerURL : null;

		public string BoxFrontPath => FileLocation.Art.Console + ID + "P-BXF.jpg";

		public string BoxBackPath => FileLocation.Art.Console + ID + "P-BXB.jpg";

		public string BannerPath => FileLocation.Art.Console + ID + "P-BNR.jpg";

		public string ConsolePath => FileLocation.Art.Console + ID + "P-CNSL.jpg";

		public string ControllerPath => FileLocation.Art.Console + ID + "P-CTRL.jpg";


		IList IDBPlatform.Releases => Releases;


		public void GetGames()
		{
			//Games = GamesDB.GetPlatformGames(this);
		}

		//public int ScrapeBoxFront()
		//{
		//	WebClient webclient = new WebClient();
		//	try
		//	{
		//		if (BoxFrontURL != null && !File.Exists(BoxFrontPath))
		//		{
		//			webclient.SetStandardHeaders();
		//			webclient.DownloadFile(BoxFrontURL, BoxFrontPath);
		//			OnPropertyChanged("BoxFrontPath");
		//			return 0;
		//		}
		//		else
		//		{
		//			return 1;
		//		}
		//	}
		//	catch (WebException)
		//	{
		//		return 2;
		//	}
		//}

		public int ScrapeArt(LocalDB localDB)
		{
			// TODO: Add other db options for art
			WebClient webclient = new WebClient();
			Stopwatch Watch = Stopwatch.StartNew();

			if (BoxFrontURL != null && !File.Exists(BoxFrontPath))
			{
				Reporter.Report("  Box front...");
				webclient.DownloadFileFromDB(BoxFrontURL, BoxFrontPath);
				OnPropertyChanged("BoxFrontPath");
				Reporter.ReportInline(Watch.Elapsed.ToString("ss"));
			}
			Watch.Restart();

			if (BoxBackURL != null && !File.Exists(BoxBackPath))
			{
				Reporter.Report("  Box back...");
				webclient.DownloadFileFromDB(BoxBackURL, BoxBackPath);
				OnPropertyChanged("BoxBackPath");
				Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
			}
			Watch.Restart();

			if (BannerURL != null && !File.Exists(BannerPath))
			{
				Reporter.Report("  Banner...");
				webclient.DownloadFileFromDB(BannerURL, BannerPath);
				OnPropertyChanged("BannerPath");
				Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
			}
			Watch.Restart();

			if (ConsoleURL != null && !File.Exists(ConsolePath))
			{
				Reporter.Report("  Console...");
				webclient.DownloadFileFromDB(ConsoleURL, ConsolePath);
				OnPropertyChanged("ConsolePath");
				OnPropertyChanged("MainDisplay");
				Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
			}
			Watch.Restart();

			if (ControllerURL != null && !File.Exists(ControllerPath))
			{
				Reporter.Report("  Controller...");
				webclient.DownloadFileFromDB(ControllerURL, ControllerPath);
				OnPropertyChanged("ControllerFile");
				Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
			}
			Watch.Restart();

			//TODO: Make the return integers make sense across all scrapeart functions
			return 0;
		}

		public async void GetReleaseDirectoryAsync(string[] paths)
		{
			Reporter.Report("Getting " + Title + " files.");
			Directory.CreateDirectory(FileLocation.RomsBackup);

			await Task.Run(() =>
			{
				int[] totals = { 0, 0 };
				string[] deeperPaths;
				int k;

				Directory.CreateDirectory(RomDirectory);
				foreach (string path in paths)
				{
					if (Directory.Exists(path))
					{
						deeperPaths = Directory.GetFiles(path, "*", searchOption: SearchOption.AllDirectories);
						Reporter.Report("Found " + deeperPaths.Count() + " in " + path);
					}
					else
					{
						deeperPaths = paths;
					}

					foreach (string file in deeperPaths)
					{
						if (Path.GetExtension(file) == ".zip")
						{
							k = GetRomsFromZipFile(file);
						}
						else
						{
							k = GetRomFromFile(file);
						}

						totals[0] += k;
						if (k > 0)
						{
							totals[1] += 1;
						}
					}
				}
				Reporter.Report("Added " + totals[1] + " ROMs to " + RomDirectory);
				Reporter.Report("Updated " + totals[0] + " releases.");
			});
		}

		public int GetRomsFromZipFile(string filename)
		{
			int total = 0;
			string sha1;
			Rom matchedRom;

			try
			{
				using (ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Read))
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						using (Stream stream = entry.Open())
						using (MemoryStream memoryStream = new MemoryStream())
						{
							int count;
							do
							{
								byte[] buffer = new byte[1024];
								count = stream.Read(buffer, 0, 1024);
								memoryStream.Write(buffer, 0, count);
							} while (stream.CanRead && count > 0);

							//sha1 = GetHash(memoryStream, "SHA1", (int)HeaderLength, (int)entry.Length);
							sha1 = GetHash(memoryStream, "SHA1", (int)HeaderLength);
							matchedRom = Roms.FirstOrDefault(x => sha1.Equals(x.SHA1, StringComparison.OrdinalIgnoreCase));

							if (matchedRom == null && HeaderLength > 0)
							{
								//sha1 = GetHash(memoryStream, "SHA1", 0, (int)entry.Length);
								sha1 = GetHash(memoryStream, "SHA1", 0);

								matchedRom = Roms.FirstOrDefault(x => sha1.Equals(x.SHA1, StringComparison.OrdinalIgnoreCase));
							}

							// Have found a match so do this stuff with it
							if (matchedRom != null)
							{
								// Check that the release has no filename, or the file doesn't yet exist
								if (string.IsNullOrEmpty(matchedRom.FileName) || !File.Exists(matchedRom.FilePath))
								{
									string extension = Path.GetExtension(entry.Name);
									matchedRom.StoreFileName(extension);

									if (File.Exists(matchedRom.FilePath))
									{
										File.Move(matchedRom.FilePath, FileLocation.RomsBackup + matchedRom.FileName);
									}

									if (matchedRom.Platform_ID == CONSTANTS.LYNX_PLATFORM_ID)
									{
										//TODO: This looks pretty shady
										string tempFile = "lnxtmp.lyx";
										string tempFile2 = "lnxtmp.lnx";
										string tempPath = Path.GetDirectoryName(FileLocation.HandyConverter) + @"\" + tempFile;
										string tempPath2 = Path.GetDirectoryName(FileLocation.HandyConverter) + @"\" + tempFile2;
										File.Delete(tempPath);
										File.Delete(tempPath2);

										entry.ExtractToFile(tempPath);
										Handy.ConvertLynx(tempFile);
										File.Move(tempPath, matchedRom.FilePath);
									}
									else
									{
										entry.ExtractToFile(matchedRom.FilePath);
									}
									total += 1;
								}
							}
						}
					}
				}
			}

			catch (Exception)
			{
			}

			return total;
		}

		public int GetRomFromFile(string foundFilePath)
		{
			int total = 0;

			var sha1 = GetHash(foundFilePath, "SHA1", (int)HeaderLength);
			Rom matchedRom = Roms.FirstOrDefault(x => sha1.Equals(x.SHA1, StringComparison.OrdinalIgnoreCase));
			if (matchedRom == null && HeaderLength > 0)
			{
				sha1 = GetHash(foundFilePath, "SHA1", 0);
				matchedRom = Roms.FirstOrDefault(x => sha1.Equals(x.SHA1, StringComparison.OrdinalIgnoreCase));
			}

			// Have found a match so do this stuff with it
			if (matchedRom != null)
			{
				// Check whether the release has a filename stored and that file exists
				if (string.IsNullOrEmpty(matchedRom.FileName) || !File.Exists(matchedRom.FilePath))
				{
					string extension = Path.GetExtension(foundFilePath);
					matchedRom.StoreFileName(extension);

					if (foundFilePath != matchedRom.FilePath)
					{
						if (File.Exists(matchedRom.FilePath))
						{
							File.Move(matchedRom.FilePath, FileLocation.RomsBackup + matchedRom.FileName);
						}

						if (matchedRom.Platform_ID == CONSTANTS.LYNX_PLATFORM_ID)
						{
							string tempFile = "lnxtmp.lyx";
							string tempFile2 = "lnxtmp.lnx";
							string tempPath = Path.GetDirectoryName(FileLocation.HandyConverter) + @"\" + tempFile;
							string tempPath2 = Path.GetDirectoryName(FileLocation.HandyConverter) + @"\" + tempFile2;

							File.Delete(tempPath);
							Thread.Sleep(100);
							File.Copy(foundFilePath, tempPath);
							Thread.Sleep(100);
							if (Handy.ConvertLynx(tempFile))
							{
								File.Move(tempPath2, matchedRom.FilePath);
							}

						}

						else
						{
							File.Copy(foundFilePath, matchedRom.FilePath);
						}
					}
					total = 1;
				}
			}
			return total;
		}

		public static string GetHash(string file, string method, int headerlength = 0)
		{
			string hash;
			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				hash = GetHash(stream, method, headerlength);
			}
			return hash;
		}

		public static string GetHash(Stream stream, string method, int headerlength = 0)
		{
			string hash = "";
			int streamLength = (int)stream.Length;

			if (streamLength < headerlength)
			{
				return "";
			}
			// Read header
			//byte[] header = new byte[headerlength];
			stream.Seek(headerlength, SeekOrigin.Begin);
			//stream.Read(header, 0, headerlength);

			// Read buffer
			byte[] buffer = new byte[streamLength - headerlength];
			stream.Read(buffer, 0, (streamLength - headerlength));

			// Read buffer
			//byte[] buffer = new byte[streamLength - headerlength];
			//stream.Seek(headerlength, SeekOrigin.Begin);
			//stream.Read(buffer, 0, (streamLength - headerlength));

			switch (method)
			{
				case "SHA1":
					SHA1Managed managedSHA1 = new SHA1Managed();
					byte[] shaBuffer = managedSHA1.ComputeHash(buffer);
					foreach (byte b in shaBuffer)
					{
						hash += b.ToString("x2").ToUpper();
					}
					break;
			}

			return hash;
		}

		public void MarkPreferred(Emulator emulator)
		{
			if (emulator != null)
			{
				PreferredEmulator_ID = emulator.ID;
			}
		}

		public void Play()
		{
			// TODO: Launch prefered emulator
			throw new NotImplementedException();
		}

		public void CopyOverview()
		{

			if (ID_GDB != null)
			{
				Overview = GDBPlatform.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GB != null)
			{
				Overview = GBPlatform.Deck;
			}

		}

		public void CopyDeveloper()
		{
			if (ID_LB != null)
			{
				Developer = LBPlatform.Developer;
			}

			if ((Developer == null || Developer.Length < 2) && ID_GDB != null)
			{
				Developer = GDBPlatform.Developer;
			}
		}

		public void CopyManufacturer()
		{
			if (ID_LB != null)
			{
				Manufacturer = LBPlatform.Manufacturer;
			}

			if ((Manufacturer == null || Manufacturer.Length < 2) && ID_GDB != null)
			{
				Manufacturer = GDBPlatform.Manufacturer;
			}
		}

		public void CopyData()
		{
			CopyOverview();
			CopyDeveloper();
			//CopyPublisher();
			//CopyGenre();
			//CopyDate();
			//CopyPlayers();
		}

	}
}

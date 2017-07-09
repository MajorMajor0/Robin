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
using System.ComponentModel;
using System.Data.Entity;
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
	public partial class Platform : IDBobject, IDBPlatform, INotifyPropertyChanged
	{

		public OVGPlatform OVGPlatform
		{
			get { return R.Data.OVGPlatforms.FirstOrDefault(x => x.ID == ID); }
		}

		public int MatchedToGamesDB
		{
			get
			{
				return Releases.Where(x => x.ID_GDB != null).Count();
			}
		}

		public int MatchedToGiantBomb
		{
			get
			{
				return Releases.Where(x => x.ID_GB != null).Count();
			}
		}

		public int MatchedToOpenVG
		{
			get
			{
				return Releases.Where(x => x.ID_OVG != null).Count();
			}
		}

		public int MatchedToLaunchBox
		{
			get
			{
				return Releases.Where(x => x.ID_LB != null).Count();
			}
		}

		public int MatchedToSomething
		{
			get
			{
				return Releases.Where(x => ((x.ID_GB != null) || (x.ID_GDB != null) || (x.ID_OVG != null) || (x.ID_LB != null))).Count();
			}
		}

		public int ReleasesWithArt
		{
			get { return Releases.Where(x => x.HasArt).Count(); }
		}

		public int ReleasesIncluded
		{
			get { return Releases.Where(x => x.Included).Count(); }
		}


		public bool Included
		{
			get
			{
				return Releases.Any(x => x.Included) && Emulators.Any(x => x.Included);
			}
		}

		public bool HasArt
		{
			get { return File.Exists(BoxFrontFile); }
		}

		public bool IsCrap { get; set; }

		bool IDBobject.Unlicensed
		{
			get
			{
				throw new NotImplementedException();
			}
		}


		public string RomDirectory
		{
			get { return FileLocation.Roms + FileName + @"\"; }
		}

		public string BoxFrontURL
		{
			get
			{
				return GDBPlatform != null ? GDBPlatform.BoxFrontURL : null;
			}
		}

		public string BoxBackURL
		{
			get
			{
				return GDBPlatform != null ? GDBPlatform.BoxBackURL : null;
			}
		}

		public string BannerURL
		{
			get
			{
				return GDBPlatform != null ? GDBPlatform.BannerURL : null;
			}
		}

		public string ConsoleURL
		{
			get
			{
				return GDBPlatform != null ? GDBPlatform.ConsoleURL : null;
			}
		}

		public string ControllerURL
		{
			get
			{
				return GDBPlatform != null ? GDBPlatform.ControllerURL : null;
			}
		}

		public string BoxFrontFile
		{
			get { return FileLocation.Art.Console + this.ID + "P-BXF.jpg"; }
		}

		public string BoxBackFile
		{
			get { return FileLocation.Art.Console + this.ID + "P-BXB.jpg"; }
		}

		public string BannerFile
		{
			get { return FileLocation.Art.Console + this.ID + "P-BNR.jpg"; }
		}

		public string ConsoleFile
		{
			get { return FileLocation.Art.Console + this.ID + "P-CNSL.jpg"; }
		}

		public string ControllerFile
		{
			get { return FileLocation.Art.Console + this.ID + "P-CTRL.jpg"; }
		}

		IList IDBPlatform.Releases
		{
			get { return Releases as IList; }
		}


		public void GetGames()
		{
			//Games = GamesDB.GetPlatformGames(this);
		}

		//public void Add(RobinDataEntities LDBdata)
		//{
		//	Platform platform = LDBdata.Platforms.FirstOrDefault(x => x.ID == ID);
		//	foreach (Release release in platform.Releases)
		//	{
		//		Releases.Add((Release)release);
		//	}
		//}

		public int ScrapeBoxFront()
		{
			WebClient webclient = new WebClient();
			try
			{
				if (BoxFrontURL != null && !File.Exists(BoxFrontFile))
				{
					webclient.SetStandardHeaders();
					webclient.DownloadFile(BoxFrontURL, BoxFrontFile);
					OnPropertyChanged("BoxFrontFile");
					return 0;
				}
				else
				{
					return 1;
				}
			}
			catch (WebException)
			{
				return 2;
			}
		}

		public void ScrapeArt()
		{
			WebClient webclient = new WebClient();
			Directory.CreateDirectory(FileLocation.Art.Console);
			Stopwatch Watch = new Stopwatch();
			Watch.Start();

			try
			{
				Reporter.Report("  Box front...");
				if (BoxFrontURL != null && !File.Exists(BoxFrontFile))
				{
					webclient.SetStandardHeaders();
					webclient.DownloadFile(BoxFrontURL, BoxFrontFile);
					OnPropertyChanged("BoxFrontFile");
					Reporter.ReportInline(Watch.Elapsed.ToString("ss"));
				}
				else
				{
					Reporter.ReportInline("skipped");
				}
				Watch.Restart();
			}

			catch (WebException)
			{
				Reporter.Report("  Failed getting box front art.");
			}

			try
			{
				Reporter.Report("  Box back...");
				if (BoxBackURL != null && !File.Exists(BoxBackFile))
				{
					webclient.SetStandardHeaders();
					webclient.DownloadFile(BoxBackURL, BoxBackFile);
					OnPropertyChanged("BoxBackFile");
					Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
				}
				else
				{
					Reporter.ReportInline("skipped");
				}
				Watch.Restart();
			}

			catch (WebException)
			{
				Reporter.Report("Failed to get box back art.");
			}

			try
			{
				Reporter.Report("  Banner...");
				if (BannerURL != null && !File.Exists(BannerFile))
				{
					webclient.SetStandardHeaders();
					webclient.DownloadFile(BannerURL, BannerFile);
					OnPropertyChanged("BannerFile");
					Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
				}
				else
				{
					Reporter.ReportInline("skipped");
				}
				Watch.Restart();
			}

			catch (WebException)
			{
				Reporter.Report("  Failed to get banner art.");
			}

			try
			{
				Reporter.Report("  Console...");
				if (ConsoleURL != null && !File.Exists(ConsoleFile))
				{
					webclient.SetStandardHeaders();
					webclient.DownloadFile(ConsoleURL, ConsoleFile);
					OnPropertyChanged("ConsoleFile");
					Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
				}
				else
				{
					Reporter.ReportInline("skipped");
				}
				Watch.Restart();
			}

			catch (WebException)
			{
				Reporter.Report("  Failed getting console art.");
			}

			try
			{
				Reporter.Report("  Controller...");
				if (ControllerURL != null && !File.Exists(ControllerFile))
				{
					webclient.SetStandardHeaders();
					webclient.DownloadFile(ControllerURL, ControllerFile);
					OnPropertyChanged("ControllerFile");
					Reporter.ReportInline(Watch.Elapsed.ToString("ss") + " s");
				}
				else
				{
					Reporter.ReportInline("skipped");
				}
				Watch.Restart();
			}

			catch (WebException)
			{
				Reporter.Report("Failed to get controller art.");
			}
		}

		public async void GetReleaseDirectoryAsync(string[] paths)
		{
			Reporter.Report("Getting " + Title + " files.");
			Directory.CreateDirectory(FileLocation.RomsBackup);

			await Task.Run(() =>
			{
				int[] totals = { 0, 0 };
				string[] deeperPaths = null;
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
			string sha1 = string.Empty;
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
							int count = 0;
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

										string tempFile = "lnxtmp.lyx";
										string tempFile2 = "lnxtmp.lnx";
										string tempPath = Path.GetDirectoryName(FileLocation.HandyConverter) + @"\" + tempFile;
										string tempPath2 = Path.GetDirectoryName(FileLocation.HandyConverter) + @"\" + tempFile2;
										File.Delete(tempPath);
										File.Delete(tempPath2);

										entry.ExtractToFile(tempPath);
										Handy.ConvertLnx(tempFile);
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
			string sha1 = string.Empty;
			Rom matchedRom;

			sha1 = GetHash(foundFilePath, "SHA1", (int)HeaderLength);
			matchedRom = Roms.FirstOrDefault(x => sha1.Equals(x.SHA1, StringComparison.OrdinalIgnoreCase));
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
							if (Handy.ConvertLnx(tempFile))
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
			string hash = "";
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
			byte[] header = new byte[headerlength];
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

				default:
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
			throw new NotImplementedException();
		}

		//public void CopyOverview()
		//{

		//	if (ID_GDB != null)
		//	{
		//		Overview = GDBPlatform.Overview;
		//	}

		//	if ((Overview == null || Overview.Length < 20) && ID_GB != null)
		//	{
		//		Overview = GBPlatform.Deck;
		//	}

		//}

		//public void CopyDeveloper()
		//{
		//	if (ID_LB != null)
		//	{
		//		Developer = LBPlatform.Developer;
		//	}

		//	if ((Developer == null || Developer.Length < 2) && ID_GDB != null)
		//	{
		//		Developer = GDBPlatform.Developer;
		//	}
		//}

		//public void CopyManufacturer()
		//{
		//	if (ID_LB != null)
		//	{
		//		Manufacturer = LBPlatform.Manufacturer;
		//	}

		//	if ((Manufacturer == null || Manufacturer.Length < 2) && ID_GDB != null)
		//	{
		//		Manufacturer = GDBPlatform.Manufacturer;
		//	}
		//}

		//public void CopyData()
		//{
		//	CopyOverview();
		//	CopyDeveloper();
		//	CopyPublisher();
		//	CopyGenre();
		//	CopyDate();
		//	CopyPlayers();
		//}

	}
}

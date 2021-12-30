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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;


namespace Robin
{
	public partial class Platform : IDBobject, IDBPlatform
	{
		[NotMapped]
		public IEnumerable<Game> Games => Releases.Select(x => x.Game).Distinct();
		
		[NotMapped]
		public Platform RPlatform => this;
		
		[NotMapped]
		public Ovgplatform Ovgplatform => R.Data.Ovgplatforms.FirstOrDefault(x => x.Id == Id);
		
		[NotMapped]
		public int MatchedToGamesDB => Releases.Count(x => x.ID_GDB != null);
		
		[NotMapped]
		public int MatchedToGiantBomb => Releases.Count(x => x.ID_GB != null);
		
		[NotMapped]
		public int MatchedToOpenVG => Releases.Count(x => x.ID_OVG != null);
		
		[NotMapped]
		public int MatchedToLaunchBox => Releases.Count(x => x.ID_LB != null);
		
		[NotMapped]
		public int MatchedReleaseCount => Releases.Count(x => x.MatchedToSomething);
		
		[NotMapped]
		public int MatchedGameCount => Games.Count(x => x.MatchedToSomething);
		
		[NotMapped]
		public int ReleasesWithArt => Releases.Count(x => x.HasArt);
		
		[NotMapped]
		public int GamesWithArt => Games.Count(x => x.HasArt);
		
		[NotMapped]
		public int ReleasesIncluded => Releases.Count(x => x.Included);

		[NotMapped]
		public bool Included => HasEmulator && HasRelease;
		
		[NotMapped]
		public bool HasEmulator => Emulators.Any(x => x.Included);
		
		[NotMapped]
		public bool HasRelease => Releases.Any(x => x.Included);

		private List<Rom> roms;
	
		[NotMapped]
		public List<Rom> Roms
		{
			get
			{
				if (roms == null)
				{
					roms = R.Data.Roms.Where(x => x.PlatformId == Id).ToList();
				}
				return roms;
			}
		}

		[NotMapped]
		public string WhyCantIPlay
		{
			get
			{
				if (Included)
				{
					return $"{Title} is ready to play.";
				}
				string and = HasRelease || HasEmulator ? "" : " and ";
				string emulatorTrouble = HasEmulator ? "" : "no emulator appears to be installed for it";
				string releaseTrouble = HasRelease ? "" : "no rom files appear to be available";
				return $"{Title} can't launch because {releaseTrouble} {and} {emulatorTrouble}.";
			}
		}
		
		[NotMapped]
		public bool HasArt => Catalog.Art.Contains(BoxFrontPath);
		
		[NotMapped]
		public bool IsCrap { get; set; }
		
		[NotMapped]
		public bool Unlicensed => false;

		[NotMapped]
		public string MainDisplay => ConsolePath;
		
		[NotMapped]
		public string RomDirectory => FileLocation.Roms + FileName + @"\";
		
		[NotMapped]
		public string BoxFrontUrl => Gdbplatform?.BoxFrontUrl;
		
		[NotMapped]
		public string BoxBackUrl => Gdbplatform?.BoxBackUrl;
		
		[NotMapped]
		public string BannerUrl => Gdbplatform?.BannerUrl;
		
		[NotMapped]
		public string ConsoleUrl => Gdbplatform?.ConsoleUrl;
		
		[NotMapped]
		public string ControllerUrl => Gdbplatform?.ControllerUrl;
		
		[NotMapped]
		public string BoxFrontPath => FileLocation.Art.Console + Id + "P-BXF.jpg";
		
		[NotMapped]
		public string BoxBackPath => FileLocation.Art.Console + Id + "P-BXB.jpg";
		
		[NotMapped]
		public string BannerPath => FileLocation.Art.Console + Id + "P-BNR.jpg";
		
		[NotMapped]
		public string ConsolePath => FileLocation.Art.Console + Id + "P-CNSL.jpg";
		
		[NotMapped]
		public string ControllerPath => FileLocation.Art.Console + Id + "P-CTRL.jpg";

		[NotMapped]
		IList IDBPlatform.Releases => Releases.ToList();

		public void GetGames()
		{
			Debug.Assert(false, "Called GetGames() on Robin.Platform. Don't do that. GetGames is for LocalDB caches.");
		}

		/// <summary>
		/// Scrape art from the selected online database to the built-in file location unique to the release instance. 
		/// </summary>
		/// <param name="artType">The type of art to scrape. Default is all available art.</param>
		/// <param name="localDB">Null for platforms.</param>
		/// <returns>Returns a negative integer to indicate the number of scraping attempts that could be tried again, or 0 if all attempts are successfull.</returns>
		public int ScrapeArt(ArtType artType, LocalDB localDB = 0)
		{
			int returner = 0;
			string url = null;
			string filePath = null;
			string property = null;

			switch (artType)
			{
				case ArtType.All:
					returner += ScrapeArt(ArtType.BoxFront);
					returner += ScrapeArt(ArtType.BoxBack);
					returner += ScrapeArt(ArtType.Banner);
					returner += ScrapeArt(ArtType.Console);
					returner += ScrapeArt(ArtType.Controller);
					break;
				case ArtType.BoxFront:
					url = BoxFrontUrl;
					filePath = BoxFrontPath;
					property = "BoxFrontPath";
					break;
				case ArtType.BoxBack:
					url = BoxBackUrl;
					filePath = BoxBackPath;
					property = "BoxBackPath";
					break;
				case ArtType.Banner:
					url = BannerUrl;
					filePath = BannerPath;
					property = "BannerPath";
					break;
				case ArtType.Console:
					url = ConsoleUrl;
					filePath = ConsolePath;
					property = "ConsolePath";
					break;
				case ArtType.Controller:
					url = ControllerUrl;
					filePath = ControllerPath;
					property = "ControllerPath";
					break;
				default:
					// Not implemented.
					Debug.Assert(false, $"Called Release.ScrapeArt() with the option {artType.Description()}, which is valid only for Releases. Can't see what it's hurting, but don't do that.");
					break;
			}

			return Scrape(url, filePath, property, artType.Description());
		}

		/// <summary>
		/// Sub function to scrape art based on strings computed elsewhere.
		/// </summary>
		/// <param name="url">URL to scrape art from.</param>
		/// <param name="filePath">Path to download art to.</param>
		/// <param name="property">Property of this release to notify PropertyChanged if art is downloaded.</param>
		/// <param name="description">String describing the art to download.</param>
		/// <returns>Returns -1 if artwork to indicate the scrape attempt could be tried again, or 0 if the scrape attempt is successfull.</returns>
		int Scrape(string url, string filePath, string property, string description)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(filePath))
				{
					if (url != null)
					{
						Reporter.Report($"Getting {description} art for {Title}...");

						if (webclient.DownloadFileFromDB(url, filePath))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged(property);
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return -1;
						}
					}
				}
			}

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
			string Sha1;
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

							// TODO Some roms in DB have no Sha1 this is a substantial bug

							Sha1 = Audit.GetHash(memoryStream, HashOption.Sha1, (int)HeaderLength);
							matchedRom = Roms.FirstOrDefault(x => Sha1.Equals(x.Sha1, StringComparison.OrdinalIgnoreCase));

							if (matchedRom == null && HeaderLength > 0)
							{
								Sha1 = Audit.GetHash(memoryStream, HashOption.Sha1, 0);

								matchedRom = Roms.FirstOrDefault(x => Sha1.Equals(x.Sha1, StringComparison.OrdinalIgnoreCase));
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

									if (matchedRom.PlatformId == CONSTANTS.LYNX_PlatformId)
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

			var Sha1 = Audit.GetHash(foundFilePath, HashOption.Sha1, (int)HeaderLength);
			Rom matchedRom = Roms.FirstOrDefault(x => Sha1.Equals(x.Sha1, StringComparison.OrdinalIgnoreCase));
			if (matchedRom == null && HeaderLength > 0)
			{
				Sha1 = Audit.GetHash(foundFilePath, HashOption.Sha1, 0);
				matchedRom = Roms.FirstOrDefault(x => Sha1.Equals(x.Sha1, StringComparison.OrdinalIgnoreCase));
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

						if (matchedRom.PlatformId == CONSTANTS.LYNX_PlatformId)
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

		public void MarkPreferred(Emulator emulator)
		{
			if (emulator != null)
			{
				PreferredEmulatorId = emulator.Id;
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
				Overview = Gdbplatform.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GB != null)
			{
				Overview = Gbplatform.Deck;
			}

		}

		public void CopyDeveloper()
		{
			if (ID_LB != null)
			{
				Developer = Lbplatform.Developer;
			}

			if ((Developer == null || Developer.Length < 2) && ID_GDB != null)
			{
				Developer = Gdbplatform.Developer;
			}
		}

		public void CopyManufacturer()
		{
			if (ID_LB != null)
			{
				Manufacturer = Lbplatform.Manufacturer;
			}

			if ((Manufacturer == null || Manufacturer.Length < 2) && ID_GDB != null)
			{
				Manufacturer = Gdbplatform.Manufacturer;
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
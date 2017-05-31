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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Robin
{
	public partial class Release : IDBobject, INotifyPropertyChanged
	{

		public Release(long platform, string title, long? id_gdb,
			long? id_gb, long? game_id, Region region)
		{
			Platform_ID = platform;
			Title = title;
			ID_GDB = id_gdb;
			ID_GB = id_gb;
			Game_ID = game_id;
			Region = region;
		}

		public Release(long platform)
		{
			Platform_ID = platform;
		}

		public string TitleAndRegion
		{
			get { return Title + " " + Version + " (" + Region.Title + ")"; }
		}

		public string PlatformTitle
		{
			get { return Platform.Title; }
		}

		public string FilePath
		{
			get { return Platform.RomDirectory + FileName; }
		}

		public bool Included
		{
			get { return File.Exists(FilePath); }
		}

		public bool HasArt
		{
			get { return File.Exists(BoxFrontFile); }
		}

		public string BoxFrontURL(LocalDB DB = 0)
		{
			string URL = null;
			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Box - Front"));
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}
					}
					if (string.IsNullOrEmpty(URL) && ID_GB != null)
					{
						URL = GBRelease.BoxURL;
					}
					if (string.IsNullOrEmpty(URL) && ID_OVG != null)
					{
						URL = OVGRelease.BoxFrontURL;
					}
					if (string.IsNullOrEmpty(URL) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID))
					{
						if (ID_LB != null)
						{
							LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => x.Type == "Box - Front");
							if (lbImage != null)
							{
								URL = Launchbox.IMAGESURL + lbImage.FileName;
							}
						}

						if (ID_GDB != null && !string.IsNullOrEmpty(GDBRelease.BoxFrontURL))
						{
							URL = GDBRelease.BoxFrontURL.Replace(@"boxart/", @"boxart/thumb/");
						}
					}
					break;

				case LocalDB.GamesDB:
					if (ID_GDB != null && !string.IsNullOrEmpty(GDBRelease.BoxFrontURL))
					{
						URL = GDBRelease.BoxFrontURL.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;
				case LocalDB.GiantBomb:
					if (ID_GB != null)
					{
						URL = GBRelease.BoxURL;
					}
					break;

				case LocalDB.OpenVGDB:
					if (ID_OVG != null)
					{
						URL = OVGRelease.BoxFrontURL;
					}
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Box - Front"));
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}

						if (string.IsNullOrEmpty(URL) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID))
						{
							LBImage lbImage2 = LBGame.LBImages.FirstOrDefault(x => x.Type == "Box - Front");
							if (lbImage2 != null)
							{
								URL = Launchbox.IMAGESURL + lbImage2.FileName;
							}
						}

					}
					break;
			}

			return string.IsNullOrEmpty(URL) ? null : URL;
		}

		public string BoxBackURL(LocalDB DB = 0)
		{
			string URL = null;
			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Box - Back"));
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}
					}
					if (string.IsNullOrEmpty(URL) && ID_OVG != null)
					{
						URL = OVGRelease.BoxBackURL;
					}

					if (string.IsNullOrEmpty(URL) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID))
					{
						if (ID_LB != null)
						{
							LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Type == "Box - Back"));
							if (lbImage != null)
							{
								URL = Launchbox.IMAGESURL + lbImage.FileName;
							}
						}

						if (string.IsNullOrEmpty(URL) && ID_GDB != null && !string.IsNullOrEmpty(GDBRelease.BoxBackURL))
						{
							URL = GDBRelease.BoxBackURL.Replace(@"boxart/", @"boxart/thumb/");
						}
					}

					break;

				case LocalDB.GamesDB:
					if (ID_GDB != null && !string.IsNullOrEmpty(GDBRelease.BoxBackURL))
					{
						URL = GDBRelease.BoxBackURL.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;
				case LocalDB.GiantBomb:
					break;
				case LocalDB.OpenVGDB:
					if (ID_OVG != null)
					{
						URL = OVGRelease.BoxBackURL;
					}
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Box - Back"));
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}
					}
					break;

				default:
					break;
			}
			return string.IsNullOrEmpty(URL) ? null : URL;
		}

		public string ScreenURL(LocalDB DB = 0)
		{
			string URL = null;
			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Screenshot - Game Title"));
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}
					}
					if (string.IsNullOrEmpty(URL) && ID_GB != null)
					{
						URL = GBRelease.ScreenURL;
					}

					if (string.IsNullOrEmpty(URL) && ID_GDB != null && (Region_ID == 21 || Game.Releases.Count == 1))
					{
						URL = GDBRelease.ScreenURL;
					}
					break;

				case LocalDB.GamesDB:
					if (ID_GDB != null)
					{
						URL = GDBRelease.ScreenURL;
					}
					break;
				case LocalDB.GiantBomb:
					if (ID_GB != null)
					{
						URL = GBRelease.ScreenURL;
					}
					break;
				case LocalDB.OpenVGDB:
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Screenshot - Game Title"));
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}
					}
					break;
			}
			return string.IsNullOrEmpty(URL) ? null : URL;
		}

		public string BannerURL(LocalDB DB = 0)
		{
			string URL = null;

			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						if (ID_LB != null)
						{
							LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Banner"));
							if (lbImage != null)
							{
								URL = Launchbox.IMAGESURL + lbImage.FileName;
							}
						}
					}

					if (string.IsNullOrEmpty(URL) && ID_GDB != null)
					{
						URL = string.IsNullOrEmpty(GDBRelease.BannerURL) ? null : GDBRelease.BannerURL;
					}
					break;
				case LocalDB.GamesDB:
					if (string.IsNullOrEmpty(URL) && ID_GDB != null)
					{
						URL = string.IsNullOrEmpty(GDBRelease.BannerURL) ? null : GDBRelease.BannerURL;
					}
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						if (ID_LB != null)
						{
							LBImage lbImage = LBGame.LBImages.FirstOrDefault(x => (x.Region_ID == Region_ID && x.Type == "Banner"));
							if (lbImage != null)
							{
								URL = Launchbox.IMAGESURL + lbImage.FileName;
							}
						}
					}
					break;
				default:
					break;
			}
			return URL;
		}

		public string LogoURL(LocalDB DB = 0)
		{
			string URL = null;

			switch (DB)
			{
				case LocalDB.Unknown:

					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault();
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}
					}

					if (string.IsNullOrEmpty(URL) && ID_GDB != null)
					{
						URL = string.IsNullOrEmpty(GDBRelease.LogoURL) ? null : GDBRelease.LogoURL;
					}
					break;
				case LocalDB.GamesDB:
					if (string.IsNullOrEmpty(URL) && ID_GDB != null)
					{
						URL = string.IsNullOrEmpty(GDBRelease.LogoURL) ? null : GDBRelease.LogoURL;
					}
					break;
				case LocalDB.GiantBomb:
					break;
				case LocalDB.OpenVGDB:
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						LBImage lbImage = LBGame.LBImages.FirstOrDefault();
						if (lbImage != null)
						{
							URL = Launchbox.IMAGESURL + lbImage.FileName;
						}
					}
					break;

				default:
					break;
			}

			return URL;
		}

		public string BoxFrontFile
		{
			get { return FileLocation.Art.BoxFront + ID + "R-BXF.jpg"; }
		}

		public string BoxFrontThumbFile
		{
			get { return FileLocation.Art.BoxFrontThumbs + ID + "R-BXF.jpg"; }
		}

		public string BoxBackFile
		{
			get { return FileLocation.Art.BoxBack + ID + "R-BXB.jpg"; }
		}

		public string ScreenFile
		{
			get { return FileLocation.Art.Screen + ID + "R-SCR.jpg"; }
		}

		public string BannerFile
		{
			get { return FileLocation.Art.Banner + ID + "R-BNR.jpg"; }
		}

		public string LogoFile
		{
			get { return FileLocation.Art.Logo + ID + "R-LGO.jpg"; }
		}

		public string MarqueeFile
		{
			get { return Platform_ID == 1 ? FileLocation.Marquee + FileName.Replace(".zip", ".png") : null; }
		}

		public void CopyOverview()
		{
			if (ID_LB != null)
			{
				Overview = LBGame.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GDB != null)
			{
				Overview = GDBRelease.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GB != null)
			{
				Overview = GBRelease.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_OVG != null)
			{
				Overview = OVGRelease.Overview;
			}
			if (!string.IsNullOrEmpty(Overview))
			{
				Overview = Overview.Clean();
			}
		}

		public void CopyDeveloper()
		{
			if (ID_LB != null)
			{
				Developer = LBGame.Developer;
			}

			if ((Developer == null || Developer.Length < 2) && ID_GDB != null)
			{
				Developer = GDBRelease.Developer;
			}

			if ((Developer == null || Developer.Length < 2) && ID_OVG != null)
			{
				Developer = OVGRelease.Developer;
			}
		}

		public void CopyPublisher()
		{
			if (ID_LB != null)
			{
				Publisher = LBGame.Publisher;
			}

			if ((Publisher == null || Publisher.Length < 2) && ID_GDB != null)
			{
				Publisher = GDBRelease.Publisher;
			}

			if ((Publisher == null || Publisher.Length < 2) && ID_OVG != null)
			{
				Publisher = OVGRelease.Publisher;
			}
		}

		public void CopyGenre()
		{
			string[] splitSeparators = { ";", "," };
			string joinSeperator = ", ";
			string[] genres;

			if (ID_LB != null && LBGame.Genres != null)
			{
				genres = LBGame.Genres.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();
				Genre = string.Join(joinSeperator, genres);
			}

			if ((Genre == null || Genre.Length < 2) && ID_GDB != null && GDBRelease.Genre != null)
			{
				genres = GDBRelease.Genre.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();

				Genre = string.Join(joinSeperator, genres);
			}

			if ((Genre == null || Genre.Length < 2) && ID_OVG != null && OVGRelease.Genre != null)
			{
				genres = OVGRelease.Genre.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();

				Genre = string.Join(joinSeperator, genres);
			}

		}

		public void CopyDate()
		{
			if (Date == null)
			{
				if (ID_LB != null)
				{
					Date = LBGame.Date;
				}

				if (Date == null && ID_GDB != null)
				{
					Date = GDBRelease.Date;
				}

				if (Date == null && ID_GB != null)
				{
					Date = GBRelease.Date;
				}

				if (Date == null && ID_OVG != null)
				{
					Date = DateTimeRoutines.SafeGetDate(OVGRelease.Date);
				}
			}
		}

		public void CopyPlayers()
		{
			if (ID_LB != null)
			{
				Players = LBGame.Players;
			}

			if (string.IsNullOrEmpty(Players) && ID_GDB != null)
			{
				Players = GDBRelease.Players;
			}

			if (string.IsNullOrEmpty(Players) && ID_GB != null)
			{
				Players = GBRelease.Players;
			}
		}

		public void CopyData()
		{
			CopyOverview();
			CopyDeveloper();
			CopyPublisher();
			CopyGenre();
			CopyDate();
			CopyPlayers();
		}


		public string FileName { get { return Rom.FileName; } }

		public string DatomaticName { get { return Rom.Title; } }

		public string SHA1 { get { return Rom.SHA1; } }

		public string MD5 { get { return Rom.MD5; } }

		public string CRC32 { get { return Rom.CRC32; } }

		public string Size { get { return Rom.Size; } }

		public string Source { get { return Rom.Source; } }


		public int ScrapeBoxFront(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontFile))
				{
					if (BoxFrontURL(DB) != null)
					{
						Reporter.Report("Getting front box art for " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxFrontURL(DB), BoxFrontFile))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("BoxFrontFile");
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

		public int ScrapeBoxBack(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxBackFile))
				{
					if (BoxBackURL(DB) != null)
					{
						Reporter.Report("Getting back box art for " + Title + "...");

						if (webclient.DownloadFileFromDB(BoxBackURL(DB), BoxBackFile))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("BoxBackFile");
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

		public int ScrapeScreen(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(ScreenFile))
				{
					if (ScreenURL(DB) != null)
					{
						Reporter.Report("Getting screen shot for " + Title + "...");
						if (webclient.DownloadFileFromDB(ScreenURL(DB), ScreenFile))
						{
							Reporter.ReportInline("success");
							OnPropertyChanged("ScreenFile");
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

		public int ScrapeBanner(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BannerFile))
				{
					if (BannerURL(DB) != null)
					{
						Reporter.Report("Getting banner for " + Title + "...");

						if (webclient.DownloadFileFromDB(BannerURL(), BannerFile))
						{
							Reporter.ReportInline("success");
							OnPropertyChanged("BannerFile");
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

		public int ScrapeLogo(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(LogoFile))
				{
					if (LogoURL(DB) != null)
					{
						Reporter.Report("Getting logo for " + Title + "...");

						if (webclient.DownloadFileFromDB(LogoURL(), LogoFile))
						{
							Reporter.ReportInline("success");
							OnPropertyChanged("LogoFile");
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

		public void ScrapeArt()
		{
		}

		public int ScrapeArt(LocalDB DB = 0)
		{
			int scrapedCount = 0;

			scrapedCount += ScrapeBoxFront(DB);
			scrapedCount += ScrapeBoxBack(DB);
			scrapedCount += ScrapeScreen(DB);
			scrapedCount += ScrapeBanner(DB);
			scrapedCount += ScrapeLogo(DB);

			return scrapedCount;
		}

		public async void Play(Emulator emulator = null)
		{

			await Task.Run(() =>
			{
				using (Process emulatorProcess = new Process())
				{
					// Choose default emulator if necessary
					if (emulator == null || !Platform.Emulators.Contains(emulator))
					{
						emulator = Platform.Emulator;
					}

					Reporter.Report("Launching " + Title + " using " + emulator.Title);
					emulatorProcess.StartInfo.CreateNoWindow = false;
					emulatorProcess.StartInfo.UseShellExecute = false;
					emulatorProcess.StartInfo.RedirectStandardOutput = true;
					emulatorProcess.StartInfo.RedirectStandardError = true;
					emulatorProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(emulator.FilePath);

					emulatorProcess.StartInfo.FileName = emulator.FilePath;

					if (emulator.ID == CONSTANTS.HIGAN_EMULATOR_ID)
					{
						emulatorProcess.StartInfo.Arguments = @"""" + FileLocation.HiganRoms + Platform.HiganRomFolder + @"\" + Path.GetFileNameWithoutExtension(FileName) + Platform.HiganExtension + @"""";
					}

					// Strip out .xls if system = MAME
					if (emulator.ID == CONSTANTS.MAME_ID)
					{
						if (Platform.ID == CONSTANTS.CHANNELF_PLATFORM_ID)
						{
							emulatorProcess.StartInfo.Arguments = "channelf -cart " + @"""" + FilePath + @"""";// + " -skip_gameinfo -nowindow";
						}
						else
						{
							emulatorProcess.StartInfo.Arguments = Path.GetFileNameWithoutExtension(FileName);
						}
					}

					else
					{
						emulatorProcess.StartInfo.Arguments = FilePath;
					}

					emulatorProcess.Start();

					string output = emulatorProcess.StandardOutput.ReadToEnd();
					string error = emulatorProcess.StandardError.ReadToEnd();
					Reporter.Report(output);
					Reporter.Report(error);
				}
			});
		}

		public async void CreateThumbnail()
		{
			await Task.Run(() =>
			{
				if (File.Exists(BoxFrontFile) && !File.Exists(BoxFrontThumbFile))
				{

					try
					{
						using (System.Drawing.Image image = System.Drawing.Image.FromFile(BoxFrontFile))
						{

							if (image.Width > 255)
							{
								System.Drawing.Image thumb = Thumbs.ResizeImage(image, 255);
								thumb.Save(BoxFrontThumbFile);
							}

							else
							{
								File.Copy(BoxFrontFile, BoxFrontThumbFile);
							}
						}
					}
					catch (OutOfMemoryException)
					{
						Reporter.Report("Bad image file: " + BoxFrontFile + " - " + Title);
					}
					OnPropertyChanged("BoxFrontThumbFile");
					Game.OnPropertyChanged("BoxFrontThumbFile", "");
				}
			});
		}

		public void Play()
		{
			throw new NotImplementedException();
		}
	}
}
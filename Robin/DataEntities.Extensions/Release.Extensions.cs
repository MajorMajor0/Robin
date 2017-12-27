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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Robin
{
	public partial class Release : IDBobject, IDBRelease
	{
		public Release()
		{

		}

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

		 bool _preferred;

		public bool Preferred
		{
			get
			{
				return _preferred;
			}
			set
			{
				if (value == true && Game != null)
				{
					foreach (Release release in Game?.Releases)
					{
						release.Preferred = false;
					}
				}
				_preferred = value;
				OnPropertyChanged("Preferred");
			}
		}

		public bool MatchedToSomething => ID_GB != null || ID_GDB != null || ID_LB != null || ID_OVG != null;

		public bool HasArt => File.Exists(BoxFrontPath);

		public bool Included => HasFile && HasEmulator;

		public bool HasFile => File.Exists(FilePath);

		public bool HasEmulator => Platform.Emulators.Any(x => x.Included);

		List<string> genreList;
		public List<string> GenreList
		{
			get
			{
				if (genreList == null)
				{
					genreList = (Genre ?? "Unknown").Split(',').Select(x => x.Trim()).ToList();
				}
				return genreList;
			}
		}


		public string WhyCantIPlay
		{
			get
			{
				if (Included)
				{
					return Title + " is ready to play.";
				}
				string and = HasFile || HasEmulator ? "" : " and ";
				string emulatorTrouble = HasEmulator ? "" : "no emulator appears installed for " + Platform.Title + ".";
				string releaseTrouble = HasFile ? "" : "no rom file appears to be available";
				return Title + " can't launch because " + releaseTrouble + and + emulatorTrouble + ".";
			}
		}

		public string MainDisplay
		{
			get
			{
				string returner;

				if (Platform_ID == CONSTANTS.ARCADE_PLATFORM_ID)
				{
					returner = LogoPath ?? MarqueePath ?? BoxFrontThumbPath ?? Platform.ControllerPath;
				}

				else
				{
					returner = BoxFrontThumbPath ?? LogoPath ?? MarqueePath ?? Platform.ControllerPath;
				}

				if (returner == LogoPath)
				{
					BorderThickness = 0;
				}
				else
				{
					BorderThickness = 1;
				}
				OnPropertyChanged("BorderThickness");
				return returner;
			}
		}

		public string Year => Date == null ? null : Date.Value.Year.ToString();

		public string TitleAndRegion => Title + " " + Version + " (" + Region.Title + ")";

		public string PlatformTitle => Platform.Title;

		public string RegionTitle => Region.Title;

		public string FilePath => Platform.RomDirectory + Rom.FileName;


		public string BoxFrontURL => GetBoxFrontURL();

		public string BoxBackURL => GetBoxBackURL();

		public string ScreenURL => GetScreenURL();

		public string BannerURL => GetBannerURL();

		public string LogoURL => GetLogoURL();

		public string Box3DURL => LBRelease?.Box3DURL;

		public string MarqueeURL => LBRelease?.MarqueeURL;//TODO Add other DBs

		public string ControlPanelURL => LBRelease?.ControlPanelURL;

		public string ControlInformationURL => LBRelease?.ControlInformationURL;

		public string CartFrontURL => LBRelease?.CartFrontURL;

		public string CartBackURL => LBRelease?.CartBackURL;

		public string Cart3DURL => LBRelease?.Cart3DURL;


		public string GetBoxFrontURL(LocalDB DB = 0)
		{
			string url = null;
			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						url = LBRelease?.BoxFrontURL;
					}
					if (string.IsNullOrEmpty(url) && ID_GB != null)
					{
						url = GBRelease?.BoxURL;
					}
					if (string.IsNullOrEmpty(url) && ID_OVG != null)
					{
						url = OVGRelease?.BoxFrontURL;
					}
					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID))
					{
						if (ID_LB != null)
						{
							url = LBRelease?.BoxFrontURL;
						}

						if (ID_GDB != null && !string.IsNullOrEmpty(GDBRelease?.BoxFrontURL))
						{
							url = GDBRelease?.BoxFrontURL.Replace(@"boxart/", @"boxart/thumb/");
						}
					}
					break;

				case LocalDB.GamesDB:
					if (ID_GDB != null && !string.IsNullOrEmpty(GDBRelease?.BoxFrontURL))
					{
						url = GDBRelease?.BoxFrontURL.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;
				case LocalDB.GiantBomb:
					if (ID_GB != null)
					{
						url = GBRelease?.BoxURL;
					}
					break;

				case LocalDB.OpenVGDB:
					if (ID_OVG != null)
					{
						url = OVGRelease?.BoxFrontURL;
					}
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						url = LBRelease?.BoxFrontURL;

						if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID))
						{
							url = LBRelease?.BoxFrontURL;
						}

					}
					break;
			}

			return string.IsNullOrEmpty(url) ? null : url;
		}

		public string GetBoxBackURL(LocalDB DB = 0)
		{
			string url = null;
			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						url = LBRelease?.BoxBackURL;
					}
					if (string.IsNullOrEmpty(url) && ID_OVG != null)
					{
						url = OVGRelease?.BoxBackURL;
					}

					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID))
					{
						if (ID_LB != null)
						{
							url = LBRelease?.BoxBackURL;
						}

						if (string.IsNullOrEmpty(url) && ID_GDB != null && !string.IsNullOrEmpty(GDBRelease?.BoxBackURL))
						{
							url = GDBRelease?.BoxBackURL.Replace(@"boxart/", @"boxart/thumb/");
						}
					}

					break;

				case LocalDB.GamesDB:
					if (ID_GDB != null && !string.IsNullOrEmpty(GDBRelease?.BoxBackURL))
					{
						url = GDBRelease?.BoxBackURL.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;
				case LocalDB.GiantBomb:
					break;
				case LocalDB.OpenVGDB:
					if (ID_OVG != null)
					{
						url = OVGRelease?.BoxBackURL;
					}
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						url = LBRelease?.BoxBackURL;
					}
					break;
			}
			return string.IsNullOrEmpty(url) ? null : url;
		}

		public string GetScreenURL(LocalDB DB = 0)
		{
			string url = null;
			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						url = LBRelease?.ScreenURL;
					}
					if (string.IsNullOrEmpty(url) && ID_GB != null)
					{
						url = GBRelease?.ScreenURL;
					}

					if (string.IsNullOrEmpty(url) && ID_GDB != null && (Region_ID == 21 || Game.Releases.Count == 1))
					{
						url = GDBRelease?.ScreenURL;
					}
					break;

				case LocalDB.GamesDB:
					if (ID_GDB != null)
					{
						url = GDBRelease?.ScreenURL;
					}
					break;
				case LocalDB.GiantBomb:
					if (ID_GB != null)
					{
						url = GBRelease?.ScreenURL;
					}
					break;
				case LocalDB.OpenVGDB:
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						url = LBRelease?.ScreenURL;
					}
					break;
			}
			return string.IsNullOrEmpty(url) ? null : url;
		}

		public string GetBannerURL(LocalDB DB = 0)
		{
			string url = null;

			switch (DB)
			{
				case LocalDB.Unknown:
					if (ID_LB != null)
					{
						url = LBRelease?.BannerURL;
					}

					if (string.IsNullOrEmpty(url) && ID_GDB != null)
					{
						url = string.IsNullOrEmpty(GDBRelease?.BannerURL) ? null : GDBRelease?.BannerURL;
					}
					break;
				case LocalDB.GamesDB:
					if (ID_GDB != null)
					{
						url = string.IsNullOrEmpty(GDBRelease?.BannerURL) ? null : GDBRelease?.BannerURL;
					}
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						url = LBRelease?.BannerURL;
					}
					break;
			}
			return url;
		}

		public string GetLogoURL(LocalDB DB = 0)
		{
			string url = null;

			switch (DB)
			{
				case LocalDB.Unknown:

					if (ID_LB != null)
					{
						url = LBRelease?.LogoURL;
					}

					if (string.IsNullOrEmpty(url) && ID_GDB != null)
					{
						url = string.IsNullOrEmpty(GDBRelease?.LogoURL) ? null : GDBRelease?.LogoURL;
					}
					break;
				case LocalDB.GamesDB:
					if (ID_GDB != null)
					{
						url = string.IsNullOrEmpty(GDBRelease?.LogoURL) ? null : GDBRelease?.LogoURL;
					}
					break;
				case LocalDB.GiantBomb:
					break;
				case LocalDB.OpenVGDB:
					break;
				case LocalDB.LaunchBox:
					if (ID_LB != null)
					{
						url = LBRelease?.LogoURL;
					}
					break;
			}

			return url;
		}


		public string BoxFrontPath => FileLocation.Art.BoxFront + ID + "R-BXF.jpg";

		public string BoxFrontThumbPath => FileLocation.Art.BoxFrontThumbs + ID + "R-BXF.jpg";

		public string BoxBackPath => FileLocation.Art.BoxBack + ID + "R-BXB.jpg";

		public string ScreenPath => FileLocation.Art.Screen + ID + "R-SCR.jpg";

		public string BannerPath => FileLocation.Art.Banner + ID + "R-BNR.jpg";

		public string LogoPath => FileLocation.Art.Logo + ID + "R-LGO.jpg";

		public string MarqueePath => Platform_ID == 1 ? FileLocation.Marquee + Rom.FileName.Replace(".zip", ".png") : null;

		public string Flag => Region.Flag;


		public int BorderThickness { get; set; } = 1;


		public void CopyOverview()
		{
			if (ID_LB != null)
			{
				Overview = LBGame.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GDB != null)
			{
				Overview = GDBRelease?.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GB != null)
			{
				Overview = GBRelease?.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_OVG != null)
			{
				Overview = OVGRelease?.Overview;
			}
			if (!string.IsNullOrEmpty(Overview))
			{
				Overview = Overview.CleanHtml();
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
				Developer = GDBRelease?.Developer;
			}

			if ((Developer == null || Developer.Length < 2) && ID_OVG != null)
			{
				Developer = OVGRelease?.Developer;
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
				Publisher = GDBRelease?.Publisher;
			}

			if ((Publisher == null || Publisher.Length < 2) && ID_OVG != null)
			{
				Publisher = OVGRelease?.Publisher;
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

			if ((Genre == null || Genre.Length < 2) && ID_GDB != null && GDBRelease?.Genre != null)
			{
				genres = GDBRelease?.Genre.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();

				Genre = string.Join(joinSeperator, genres);
			}

			if ((Genre == null || Genre.Length < 2) && ID_OVG != null && OVGRelease?.Genre != null)
			{
				genres = OVGRelease?.Genre.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();

				Genre = string.Join(joinSeperator, genres);
			}

			Genre = Genre ?? "Unknown";

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
					Date = GDBRelease?.Date;
				}

				if (Date == null && ID_GB != null)
				{
					Date = GBRelease?.Date;
				}

				if (Date == null && ID_OVG != null)
				{
					Date = OVGRelease?.Date;
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
				Players = GDBRelease?.Players;
			}

			if (string.IsNullOrEmpty(Players) && ID_GB != null)
			{
				Players = GBRelease?.Players;
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


		public int ScrapeBoxFront()
		{
			return ScrapeBoxFront(0);
		}

		public int ScrapeBoxFront(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxFrontPath))
				{
					if (GetBoxFrontURL(DB) != null)
					{
						Reporter.Report("Getting front box art for " + Title + "...");

						if (webclient.DownloadFileFromDB(GetBoxFrontURL(DB), BoxFrontPath))
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

		public int ScrapeBoxBack()
		{
			return ScrapeBoxBack(0);
		}

		public int ScrapeBoxBack(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BoxBackPath))
				{
					if (GetBoxBackURL(DB) != null)
					{
						Reporter.Report("Getting back box art for " + Title + "...");

						if (webclient.DownloadFileFromDB(GetBoxBackURL(DB), BoxBackPath))
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

		public int ScrapeScreen()
		{
			return ScrapeScreen(0);
		}

		public int ScrapeScreen(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(ScreenPath))
				{
					if (GetScreenURL(DB) != null)
					{
						Reporter.Report("Getting screen shot for " + Title + "...");
						if (webclient.DownloadFileFromDB(GetScreenURL(DB), ScreenPath))
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

		public int ScrapeBanner()
		{
			return ScrapeBanner(0);
		}

		public int ScrapeBanner(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(BannerPath))
				{
					if (GetBannerURL(DB) != null)
					{
						Reporter.Report("Getting banner for " + Title + "...");

						if (webclient.DownloadFileFromDB(GetBannerURL(), BannerPath))
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

		public int ScrapeLogo()
		{
			return ScrapeLogo(0);
		}

		public int ScrapeLogo(LocalDB DB = 0)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(LogoPath))
				{
					if (GetLogoURL(DB) != null)
					{
						Reporter.Report("Getting logo for " + Title + "...");

						if (webclient.DownloadFileFromDB(GetLogoURL(), LogoPath))
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

		public int ScrapeMarquee()
		{
			// TODO: Implement
			throw new NotImplementedException();
		}

		public int ScrapeBox3D()
		{
			// TODO: Implement
			throw new NotImplementedException();
		}

		public int ScrapeCartFront()
		{
			// TODO: Implement
			throw new NotImplementedException();
		}

		public int ScrapeCart3D()
		{
			// TODO: Implement
			throw new NotImplementedException();
		}

		public int ScrapeControlPanel()
		{
			// TODO: Implement
			throw new NotImplementedException();
		}

		public void ScrapeArt()
		{
			ScrapeArt(0);
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

		public void Play()
		{
			Play(null);
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
						emulatorProcess.StartInfo.Arguments = @"""" + FileLocation.HiganRoms + Platform.HiganRomFolder + @"\" + Path.GetFileNameWithoutExtension(Rom.FileName) + Platform.HiganExtension + @"""";
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
							emulatorProcess.StartInfo.Arguments = Path.GetFileNameWithoutExtension(Rom.FileName);
						}
					}

					else
					{
						emulatorProcess.StartInfo.Arguments = FilePath;
					}

					try
					{
						emulatorProcess.Start();
						PlayCount++;
					}
					catch (Exception)
					{
						// TODO: report something usefull here if the process fails to start
					}


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
				if (File.Exists(BoxFrontPath) && !File.Exists(BoxFrontThumbPath))
				{

					try
					{
						using (System.Drawing.Image image = System.Drawing.Image.FromFile(BoxFrontPath))
						{

							if (image.Width > 255)
							{
								System.Drawing.Image thumb = Thumbs.ResizeImage(image, 255);
								thumb.Save(BoxFrontThumbPath);
							}

							else
							{
								File.Copy(BoxFrontPath, BoxFrontThumbPath);
							}
						}
					}
					catch (OutOfMemoryException)
					{
						File.Delete(BoxFrontPath);
						Reporter.Report("Bad image file: " + BoxFrontPath + " - " + Title + ", file deleted.");
					}
					OnPropertyChanged("BoxFrontThumbFile");
					Game.OnPropertyChanged("BoxFrontThumbFile", "");
				}
			});
		}

		[Conditional("DEBUG")]
		public void SetLBReleaseMatch()
		{
			if (LBGame != null)
			{
				LBRelease = LBGame.LBReleases.FirstOrDefault(x => x.Region == Region);
			}
		}
	}
}
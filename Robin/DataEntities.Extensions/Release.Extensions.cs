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

		public bool IsBeaten
		{
			get { return Game.IsBeaten; }
			set { Game.IsBeaten = value; }
		}

		public bool IsGame
		{
			get { return Game.IsGame; }
			set { Game.IsGame = value; }
		}

		public bool IsCrap
		{
			get { return Game.IsCrap; }
			set { Game.IsCrap = value; }
		}

		public bool IsAdult
		{
			get { return Game.IsAdult; }
			set { Game.IsAdult = value; }
		}

		public bool IsMess
		{
			get { return Game.IsMess; }
			set { Game.IsMess = value; }
		}

		public bool Unlicensed
		{
			get { return Game.Unlicensed; }
			set { Game.Unlicensed = value; }
		}

		public LocalDB LocalDB => LocalDB.Robin;

		public string Overview
		{
			get { return Game.Overview; }
			set { Game.Overview = value; }
		}

		public string Developer
		{
			get { return Game.Developer; }
			set { Game.Developer = value; }
		}

		public string Publisher
		{
			get { return Game.Publisher; }
			set { Game.Publisher = value; }
		}

		public string VideoFormat
		{
			get { return Game.VideoFormat; }
			set { Game.VideoFormat = value; }
		}

		public string Players
		{
			get { return Game.Players; }
			set { Game.Players = value; }
		}

		public string Genre
		{
			get { return Game.Genre; }
			set { Game.Genre = value; }
		}


		public decimal? Rating
		{
			get { return Game.Rating; }
			set { Game.Rating = value; }
		}


		bool preferred;
		public bool Preferred
		{
			get
			{
				return preferred;
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
				preferred = value;
				OnPropertyChanged("Preferred");
			}
		}

		public bool MatchedToSomething => ID_GB != null || ID_GDB != null || ID_LB != null || ID_OVG != null;

		public bool HasArt => File.Exists(BoxFrontThumbPath) || File.Exists(LogoPath) || File.Exists(MarqueePath);

		public bool Included => HasFile && HasEmulator;

		public bool HasFile => File.Exists(FilePath);

		public bool HasEmulator => Platform.Emulators.Any(x => x.Included);

		public List<string> GenreList => Game.GenreList;

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
				if (Platform_ID == CONSTANTS.ARCADE_PLATFORM_ID)
				{
					if (File.Exists(LogoPath)) { BorderThickness = 0; OnPropertyChanged("BorderThickness"); return LogoPath; }
					if (File.Exists(MarqueePath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return MarqueePath; }
					if (File.Exists(BoxFrontThumbPath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return BoxFrontThumbPath; }
				}

				else
				{
					if (File.Exists(BoxFrontThumbPath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return BoxFrontThumbPath; }
					if (File.Exists(LogoPath)) { BorderThickness = 0; OnPropertyChanged("BorderThickness"); return LogoPath; }
					if (File.Exists(MarqueePath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return MarqueePath; }
				}

				BorderThickness = 0;
				OnPropertyChanged("BorderThickness");
				return Platform.ControllerPath;
			}
		}

		public string Year => Date?.Year.ToString();

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

					url = LBRelease?.BoxFrontURL;

					if (string.IsNullOrEmpty(url))
					{
						url = GBRelease?.BoxURL;
					}

					if (string.IsNullOrEmpty(url))
					{
						url = OVGRelease?.BoxFrontURL;
					}

					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID) && !string.IsNullOrEmpty(GDBRelease?.BoxFrontURL))
					{
						url = GDBRelease?.BoxFrontURL.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;

				case LocalDB.GamesDB:
					url = GDBRelease?.BoxFrontURL?.Replace(@"boxart/", @"boxart/thumb/");
					break;

				case LocalDB.GiantBomb:
					url = GBRelease?.BoxURL;
					break;

				case LocalDB.OpenVGDB:
					url = OVGRelease?.BoxFrontURL;
					break;

				case LocalDB.LaunchBox:
					url = LBRelease?.BoxFrontURL;
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

					url = LBRelease?.BoxBackURL;

					if (string.IsNullOrEmpty(url))
					{
						url = OVGRelease?.BoxBackURL;
					}

					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID) && !string.IsNullOrEmpty(GDBRelease?.BoxBackURL))
					{
						url = GDBRelease?.BoxBackURL.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;

				case LocalDB.GamesDB:
					if (!string.IsNullOrEmpty(GDBRelease?.BoxBackURL))
					{
						url = GDBRelease.BoxBackURL.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;

				case LocalDB.OpenVGDB:
					url = OVGRelease?.BoxBackURL;
					break;

				case LocalDB.LaunchBox:
					url = LBRelease?.BoxBackURL;
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

					url = LBRelease?.ScreenURL;

					if (string.IsNullOrEmpty(url))
					{
						url = GBRelease?.ScreenURL;
					}

					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || Region_ID == CONSTANTS.UNKNOWN_REGION_ID) && !string.IsNullOrEmpty(GDBRelease?.ScreenURL))
					{
						url = GDBRelease?.ScreenURL;
					}
					break;

				case LocalDB.GamesDB:
					url = GDBRelease?.ScreenURL;
					break;

				case LocalDB.GiantBomb:
					url = GBRelease?.ScreenURL;
					break;

				case LocalDB.LaunchBox:
					url = LBRelease?.ScreenURL;
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

					url = LBRelease?.BannerURL;

					if (string.IsNullOrEmpty(url))
					{
						url = string.IsNullOrEmpty(GDBRelease?.BannerURL) ? null : GDBRelease?.BannerURL;
					}
					break;

				case LocalDB.GamesDB:
					url = string.IsNullOrEmpty(GDBRelease?.BannerURL) ? null : GDBRelease?.BannerURL;
					break;

				case LocalDB.LaunchBox:
					url = LBRelease?.BannerURL;
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

					url = LBRelease?.LogoURL;

					if (string.IsNullOrEmpty(url))
					{
						url = string.IsNullOrEmpty(GDBRelease?.LogoURL) ? null : GDBRelease?.LogoURL;
					}
					break;

				case LocalDB.GamesDB:
					url = string.IsNullOrEmpty(GDBRelease?.LogoURL) ? null : GDBRelease?.LogoURL;
					break;

				case LocalDB.LaunchBox:
					url = LBRelease?.LogoURL;
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
				Overview = LBRelease?.Overview;
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
				Developer = LBRelease.Developer;
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
				Publisher = LBRelease.Publisher;
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

			if (ID_LB != null && LBRelease.LBGame.Genres != null)
			{
				genres = LBRelease.LBGame.Genres.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();
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
					Date = LBRelease.Date;
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
				Players = LBRelease.Players;
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

		/// <summary>
		/// IDBRelease.ScrapeBoxFront
		/// </summary>
		/// <returns>Returns -1 if artwork to indicate the scrape attempt could be tried again, or 0 if the scrape attempt is successfull.</returns>
		public int ScrapeBoxFront()
		{
			return ScrapeArt(ArtType.All, 0);
		}

		/// <summary>
		/// Scrape art from the selected online database to the built-in file location unique to the release instance. 
		/// </summary>
		/// <param name="artType">The type of art to scrape. Default is all available art.</param>
		/// <param name="localDB">The database to scrape from. Default is unknown, which allows Robin to cycle through available databases in preferred order.</param>
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
					returner += ScrapeArt(ArtType.BoxFront, localDB);
					returner += ScrapeArt(ArtType.BoxBack, localDB);
					returner += ScrapeArt(ArtType.Banner, localDB);
					returner += ScrapeArt(ArtType.Screen, localDB);
					returner += ScrapeArt(ArtType.Logo, localDB);
					break;
				case ArtType.BoxFront:
					url = GetBoxFrontURL(localDB);
					filePath = BoxFrontPath;
					property = "BoxFrontPath";
					break;
				case ArtType.BoxBack:
					url = GetBoxBackURL(localDB);
					filePath = BoxBackPath;
					property = "BoxBackPath";
					break;
				case ArtType.Banner:
					url = GetBannerURL(localDB);
					filePath = BannerPath;
					property = "BannerPath";
					break;
				case ArtType.Screen:
					url = GetScreenURL(localDB);
					filePath = ScreenPath;
					property = "ScreenPath";
					break;
				case ArtType.Logo:
					url = GetLogoURL(localDB);
					filePath = LogoPath;
					property = "LogoPath";
					break;
				case ArtType.Box3D:
					// Needs URL implemented
					break;
				case ArtType.Marquee:
					// Needs URL implemented
					break;
				case ArtType.ControlPanel:
					// Needs URL implemented
					break;
				case ArtType.ControlInformation:
					// Needs URL implemented
					break;
				case ArtType.CartFront:
					// Needs URL implemented
					break;
				case ArtType.CartBack:
					// Needs URL implemented
					break;
				case ArtType.Cart3D:
					// Needs URL implemented
					break;
				default:
					// Not implemented.
					Debug.Assert(false, $"Called Release.ScrapeArt() with the option {artType.Description()}, which is valid only for Platforms. Can't see what it's hurting, but don't do that.");
					break;
			}

			return Scrape(url, filePath, property, artType.Description());
		}

		/// <summary>
		/// SUb function to scrape art based on strings computed elsewhere.
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
							if (property == "BoxFrontPath")
							{
								CreateThumbnail();
							}
							OnPropertyChanged(property);
							Game.OnPropertyChanged2(property);
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
				if (File.Exists(BoxFrontPath))
				{
					try
					{
						using (System.Drawing.Image image = System.Drawing.Image.FromFile(BoxFrontPath))
						{
#if DEBUG
							Stopwatch Watch = Stopwatch.StartNew();
#endif
							File.Delete(BoxFrontThumbPath);
#if DEBUG
							Debug.WriteLine("Delete" + Watch.ElapsedMilliseconds);
							Watch.Restart();
#endif
							if (image.Width > 255)
							{
								System.Drawing.Image thumb = Thumbs.ResizeImage(image, 255);
								thumb.Save(BoxFrontThumbPath);
							}

							else
							{
								File.Copy(BoxFrontPath, BoxFrontThumbPath);
							}
#if DEBUG
							Debug.WriteLine("Create" + Watch.ElapsedMilliseconds);
							Watch.Restart();
#endif
						}
					}
					catch (OutOfMemoryException)
					{
						File.Delete(BoxFrontPath);
						Reporter.Report("Bad image file: " + BoxFrontPath + " - " + Title + ", file deleted.");
					}
					OnPropertyChanged("BoxFrontThumbFile");
					OnPropertyChanged("MainDisplay");
					Game.OnPropertyChanged2("BoxFrontThumbFile");
					Game.OnPropertyChanged2("MainDisplay");
				}
			});
		}

	}
}
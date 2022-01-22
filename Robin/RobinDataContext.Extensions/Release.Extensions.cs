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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Robin
{
	public partial class Release : IDbObject, IDbRelease
	{
		public Release(long platform, string title, long? ID_gdb,
			long? ID_gb, long? game_id, Region region)
		{
			PlatformId = platform;
			Title = title;
			ID_GDB = ID_gdb;
			ID_GB = ID_gb;
			GameId = game_id;
			Region = region;
		}

		[NotMapped]
		public bool IsBeaten
		{
			get { return Game.IsBeaten; }
			set { Game.IsBeaten = value; }
		}

		[NotMapped]
		public bool IsGame
		{
			get { return Game.IsGame; }
			set { Game.IsGame = value; }
		}

		[NotMapped]
		public bool IsCrap
		{
			get { return Game.IsCrap; }
			set { Game.IsCrap = value; }
		}

		[NotMapped]
		public bool IsAdult
		{
			get { return Game.IsAdult; }
			set { Game.IsAdult = value; }
		}

		[NotMapped]
		public bool IsMess
		{
			get { return Game.IsMess; }
			set { Game.IsMess = value; }
		}

		[NotMapped]
		public bool Unlicensed
		{
			get { return Game.Unlicensed; }
			set { Game.Unlicensed = value; }
		}

		[NotMapped]
		public LocalDB LocalDB => LocalDB.Robin;

		[NotMapped]
		public string Overview
		{
			get { return Game.Overview; }
			set { Game.Overview = value; }
		}

		[NotMapped]
		public string Developer
		{
			get { return Game.Developer; }
			set { Game.Developer = value; }
		}

		[NotMapped]
		public string Publisher
		{
			get { return Game.Publisher; }
			set { Game.Publisher = value; }
		}

		[NotMapped]
		public string VideoFormat
		{
			get { return Game.VideoFormat; }
			set { Game.VideoFormat = value; }
		}

		[NotMapped]
		public string Players
		{
			get { return Game.Players; }
			set { Game.Players = value; }
		}

		[NotMapped]
		public string Genre
		{
			get { return Game.Genre; }
			set { Game.Genre = value; }
		}

		[NotMapped]
		public double? Rating
		{
			get { return Game.Rating; }
			set { Game.Rating = value; }
		}


		bool preferred;

		[NotMapped]
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

		[NotMapped]
		public bool MatchedToSomething => ID_GB != null || ID_GDB != null || ID_LB != null || ID_OVG != null;

		[NotMapped]
		public bool HasArt => Catalog.Art.Contains(BoxFrontThumbPath)
			|| Catalog.Art.Contains(LogoPath)
			|| Catalog.Art.Contains(MarqueePath);

		[NotMapped]
		public bool Included => HasFile && HasEmulator;

		[NotMapped]
		public bool HasFile => Catalog.Roms.Contains(FilePath);

		[NotMapped]
		public bool HasEmulator => Platform.Emulators.Any(x => x.Included);

		[NotMapped]
		public List<string> GenreList => Game.GenreList;

		[NotMapped]
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

		[NotMapped]
		public string MainDisplay
		{
			get
			{
				if (PlatformId == CONSTANTS.PlatformId.Arcade)
				{
					if (Catalog.Art.Contains(LogoPath)) { BorderThickness = 0; OnPropertyChanged("BorderThickness"); return LogoPath; }
					if (Catalog.Art.Contains(MarqueePath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return MarqueePath; }
					if (Catalog.Art.Contains(BoxFrontThumbPath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return BoxFrontThumbPath; }
				}

				else
				{
					if (Catalog.Art.Contains(BoxFrontThumbPath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return BoxFrontThumbPath; }
					if (Catalog.Art.Contains(LogoPath)) { BorderThickness = 0; OnPropertyChanged("BorderThickness"); return LogoPath; }
					if (Catalog.Art.Contains(MarqueePath)) { BorderThickness = 1; OnPropertyChanged("BorderThickness"); return MarqueePath; }
				}

				BorderThickness = 0;
				OnPropertyChanged("BorderThickness");
				return Platform.ControllerPath;
			}
		}

		[NotMapped]
		public string Year => Date?.Year.ToString();

		[NotMapped]
		public string TitleAndRegion => $"{Title} {Version} ({Region.Title})";

		[NotMapped]
		public string PlatformTitle => Platform.Title;

		[NotMapped]
		public string RegionTitle => Region.Title;

		[NotMapped]
		public string FilePath => Platform.RomDirectory + Rom.FileName;

		[NotMapped]
		public string BoxFrontUrl => GetBoxFrontUrl();

		[NotMapped]
		public string BoxBackUrl => GetBoxBackUrl();

		[NotMapped]
		public string ScreenUrl => GetScreenUrl();

		[NotMapped]
		public string BannerUrl => GetBannerUrl();

		[NotMapped]
		public string LogoUrl => GetLogoUrl();

		[NotMapped]
		public string Box3DURL => Lbrelease?.Box3DUrl;

		[NotMapped]
		public string MarqueeURL => Lbrelease?.MarqueeUrl;//TODO Add other DBs

		[NotMapped]
		public string ControlPanelURL => Lbrelease?.ControlPanelUrl;

		[NotMapped]
		public string ControlInformationURL => Lbrelease?.ControlInformationUrl;

		[NotMapped]
		public string CartFrontURL => Lbrelease?.CartFrontUrl;

		[NotMapped]
		public string CartBackURL => Lbrelease?.CartBackUrl;

		[NotMapped]
		public string Cart3DURL => Lbrelease?.Cart3DUrl;


		public string GetBoxFrontUrl(LocalDB DB = 0)
		{
			string url = null;
			switch (DB)
			{
				case LocalDB.Unknown:

					url = Lbrelease?.BoxFrontUrl;

					if (string.IsNullOrEmpty(url))
					{
						url = Gbrelease?.BoxUrl;
					}

					if (string.IsNullOrEmpty(url))
					{
						url = Ovgrelease?.BoxFrontUrl;
					}

					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || RegionId == CONSTANTS.RegionId.Unk) && !string.IsNullOrEmpty(Gdbrelease?.BoxFrontUrl))
					{
						url = Gdbrelease?.BoxFrontUrl.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;

				case LocalDB.GamesDB:
					url = Gdbrelease?.BoxFrontUrl?.Replace(@"boxart/", @"boxart/thumb/");
					break;

				case LocalDB.GiantBomb:
					url = Gbrelease?.BoxUrl;
					break;

				case LocalDB.OpenVGDB:
					url = Ovgrelease?.BoxFrontUrl;
					break;

				case LocalDB.LaunchBox:
					url = Lbrelease?.BoxFrontUrl;
					break;
			}

			return string.IsNullOrEmpty(url) ? null : url;
		}

		public string GetBoxBackUrl(LocalDB DB = 0)
		{
			string url = null;
			switch (DB)
			{
				case LocalDB.Unknown:

					url = Lbrelease?.BoxBackUrl;

					if (string.IsNullOrEmpty(url))
					{
						url = Ovgrelease?.BoxBackUrl;
					}

					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || RegionId == CONSTANTS.RegionId.Unk) && !string.IsNullOrEmpty(Gdbrelease?.BoxBackUrl))
					{
						url = Gdbrelease?.BoxBackUrl.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;

				case LocalDB.GamesDB:
					if (!string.IsNullOrEmpty(Gdbrelease?.BoxBackUrl))
					{
						url = Gdbrelease.BoxBackUrl.Replace(@"boxart/", @"boxart/thumb/");
					}
					break;

				case LocalDB.OpenVGDB:
					url = Ovgrelease?.BoxBackUrl;
					break;

				case LocalDB.LaunchBox:
					url = Lbrelease?.BoxBackUrl;
					break;
			}
			return string.IsNullOrEmpty(url) ? null : url;
		}

		public string GetScreenUrl(LocalDB DB = 0)
		{
			string url = null;
			switch (DB)
			{
				case LocalDB.Unknown:

					url = Lbrelease?.ScreenUrl;

					if (string.IsNullOrEmpty(url))
					{
						url = Gbrelease?.ScreenUrl;
					}

					if (string.IsNullOrEmpty(url) && (Game.Releases.Count == 1 || RegionId == CONSTANTS.RegionId.Unk) && !string.IsNullOrEmpty(Gdbrelease?.ScreenUrl))
					{
						url = Gdbrelease?.ScreenUrl;
					}
					break;

				case LocalDB.GamesDB:
					url = Gdbrelease?.ScreenUrl;
					break;

				case LocalDB.GiantBomb:
					url = Gbrelease?.ScreenUrl;
					break;

				case LocalDB.LaunchBox:
					url = Lbrelease?.ScreenUrl;
					break;
			}
			return string.IsNullOrEmpty(url) ? null : url;
		}

		public string GetBannerUrl(LocalDB DB = 0)
		{
			string url = null;

			switch (DB)
			{
				case LocalDB.Unknown:

					url = Lbrelease?.BannerUrl;

					if (string.IsNullOrEmpty(url))
					{
						url = string.IsNullOrEmpty(Gdbrelease?.BannerUrl) ? null : Gdbrelease?.BannerUrl;
					}
					break;

				case LocalDB.GamesDB:
					url = string.IsNullOrEmpty(Gdbrelease?.BannerUrl) ? null : Gdbrelease?.BannerUrl;
					break;

				case LocalDB.LaunchBox:
					url = Lbrelease?.BannerUrl;
					break;
			}
			return url;
		}

		public string GetLogoUrl(LocalDB DB = 0)
		{
			string url = null;

			switch (DB)
			{
				case LocalDB.Unknown:

					url = Lbrelease?.LogoUrl;

					if (string.IsNullOrEmpty(url))
					{
						url = string.IsNullOrEmpty(Gdbrelease?.LogoUrl) ? null : Gdbrelease?.LogoUrl;
					}
					break;

				case LocalDB.GamesDB:
					url = string.IsNullOrEmpty(Gdbrelease?.LogoUrl) ? null : Gdbrelease?.LogoUrl;
					break;

				case LocalDB.LaunchBox:
					url = Lbrelease?.LogoUrl;
					break;
			}
			return url;
		}


		[NotMapped]
		public string BoxFrontPath => $"{FileLocation.Art.BoxFront}{Id}R-BXF.jpg";

		[NotMapped]
		public string BoxFrontThumbPath => $"{FileLocation.Art.BoxFrontThumbs}{Id}R-BXF.jpg";

		[NotMapped]
		public string BoxBackPath => $"{FileLocation.Art.BoxBack}{Id}R-BXB.jpg";

		[NotMapped]
		public string ScreenPath => $"{FileLocation.Art.Screen}{Id}R-SCR.jpg";

		[NotMapped]
		public string BannerPath => $"{FileLocation.Art.Banner}{Id}R-BNR.jpg";

		[NotMapped]
		public string LogoPath => $"{FileLocation.Art.Logo}{Id}R-LGO.jpg";

		[NotMapped]
		public string MarqueePath => PlatformId == 1 ? $"{FileLocation.Marquee}{Rom.FileName.Replace(".zip", ".png")}" : null;

		[NotMapped]
		public string Flag => Region.Flag;


		[NotMapped]
		public int BorderThickness { get; set; } = 1;


		public void CopyOverview()
		{
			if (ID_LB != null)
			{
				Overview = Lbrelease?.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GDB != null)
			{
				Overview = Gdbrelease?.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_GB != null)
			{
				Overview = Gbrelease?.Overview;
			}

			if ((Overview == null || Overview.Length < 20) && ID_OVG != null)
			{
				Overview = Ovgrelease?.Overview;
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
				Developer = Lbrelease.Developer;
			}

			if ((Developer == null || Developer.Length < 2) && ID_GDB != null)
			{
				Developer = Gdbrelease?.Developer;
			}

			if ((Developer == null || Developer.Length < 2) && ID_OVG != null)
			{
				Developer = Ovgrelease?.Developer;
			}
		}

		public void CopyPublisher()
		{
			if (ID_LB != null)
			{
				Publisher = Lbrelease.Publisher;
			}

			if ((Publisher == null || Publisher.Length < 2) && ID_GDB != null)
			{
				Publisher = Gdbrelease?.Publisher;
			}

			if ((Publisher == null || Publisher.Length < 2) && ID_OVG != null)
			{
				Publisher = Ovgrelease?.Publisher;
			}
		}

		public void CopyGenre()
		{
			string[] splitSeparators = { ";", "," };
			string joinSeperator = ", ";
			string[] genres;

			if (ID_LB != null && Lbrelease.Lbgame.Genres != null)
			{
				genres = Lbrelease.Lbgame.Genres.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();
				Genre = string.Join(joinSeperator, genres);
			}

			if ((Genre == null || Genre.Length < 2) && ID_GDB != null && Gdbrelease?.Genre != null)
			{
				genres = Gdbrelease?.Genre.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();

				Genre = string.Join(joinSeperator, genres);
			}

			if ((Genre == null || Genre.Length < 2) && ID_OVG != null && Ovgrelease?.Genre != null)
			{
				genres = Ovgrelease?.Genre.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).OrderBy(x => x).ToArray();

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
					Date = Lbrelease.Date;
				}

				if (Date == null && ID_GDB != null)
				{
					Date = Gdbrelease?.Date;
				}

				if (Date == null && ID_GB != null)
				{
					Date = Gbrelease?.Date;
				}

				if (Date == null && ID_OVG != null)
				{
					Date = Ovgrelease?.Date;
				}
			}
		}

		public void CopyPlayers()
		{
			if (ID_LB != null)
			{
				Players = Lbrelease.Players;
			}

			if (string.IsNullOrEmpty(Players) && ID_GDB != null)
			{
				Players = Gdbrelease?.Players;
			}

			if (string.IsNullOrEmpty(Players) && ID_GB != null)
			{
				Players = Gbrelease?.Players;
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
					url = GetBoxFrontUrl(localDB);
					filePath = BoxFrontPath;
					property = "BoxFrontPath";
					break;
				case ArtType.BoxBack:
					url = GetBoxBackUrl(localDB);
					filePath = BoxBackPath;
					property = "BoxBackPath";
					break;
				case ArtType.Banner:
					url = GetBannerUrl(localDB);
					filePath = BannerPath;
					property = "BannerPath";
					break;
				case ArtType.Screen:
					url = GetScreenUrl(localDB);
					filePath = ScreenPath;
					property = "ScreenPath";
					break;
				case ArtType.Logo:
					url = GetLogoUrl(localDB);
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
							Game.OnPropertyChanged(property);
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
				using Process emulatorProcess = new Process();
				// Choose default emulator if necessary
				if (emulator == null || !Platform.Emulators.Contains(emulator))
				{
					emulator = Platform.PreferredEmulator;
				}

				Reporter.Report($"Launching {Title} using {emulator.Title}");

				emulatorProcess.StartInfo.CreateNoWindow = false;
				emulatorProcess.StartInfo.UseShellExecute = false;
				emulatorProcess.StartInfo.RedirectStandardOutput = true;
				emulatorProcess.StartInfo.RedirectStandardError = true;
				emulatorProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(emulator.FilePath);

				emulatorProcess.StartInfo.FileName = emulator.FilePath;

				switch (emulator.Id)
				{
					case CONSTANTS.EmulatorId.Higan:
						emulatorProcess.StartInfo.Arguments = $"\"{FileLocation.HiganRoms}{Platform.HiganRomFolder}\\{Path.GetFileNameWithoutExtension(Rom.FileName)}{Platform.HiganExtension}\"";
						break;

					case CONSTANTS.EmulatorId.Mame:
						if (Platform.Id == CONSTANTS.PlatformId.ChannelF)
						{
							emulatorProcess.StartInfo.Arguments = $"channelf -cart \"{FilePath}\" -skip_gameinfo -nowindow";
						}
						else
						{
							// Strip out .xls if system = MAME
							emulatorProcess.StartInfo.Arguments = Path.GetFileNameWithoutExtension(Rom.FileName);
						}
						break;
					case CONSTANTS.EmulatorId.Retroarch:
						string coreFile = Platform.Cores[0].FilePath;

						emulatorProcess.StartInfo.Arguments = $"{emulator.Parameters} \"{coreFile}\" \"{FilePath}\"";
						break;


					default:
						emulatorProcess.StartInfo.Arguments = $"{emulator.Parameters} {FilePath}";
						break;
				}

				try
				{
					emulatorProcess.Start();
					emulatorProcess.PriorityClass = ProcessPriorityClass.High;

					if (!IsAdult && IsGame)
					{
						PlayCount++;
					}
				}
				catch (Exception)
				{
					// TODO: report something usefull here if the process fails to start
				}

				string output = emulatorProcess.StandardOutput.ReadToEnd();
				string error = emulatorProcess.StandardError.ReadToEnd();
				Reporter.Report(output);
				Reporter.Report(error);
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
					Game.OnPropertyChanged("BoxFrontThumbFile");
					Game.OnPropertyChanged("MainDisplay");
				}
			});
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

	}
}
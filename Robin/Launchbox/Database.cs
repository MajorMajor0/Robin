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
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Robin.Launchbox
{
	class Database: IDB
	{
		bool platformsCached;

		bool disposed;

		List<XElement> gameElements;

		List<XElement> releaseElements;

		List<XElement> imageElements;


		public string Title => "LaunchBox";

		public LocalDB DB => LocalDB.LaunchBox;

		public DbSet Platforms => R.Data.LBPlatforms;

		public DbSet Releases => R.Data.LBReleases;

		public bool HasRegions => true;

		XDocument launchboxFile;

		static string LaunchBoxDataZipFile = FileLocation.Data + "LBdata.zip";


		public Launchbox()
		{
			Reporter.Tic("Loading LaunchBox local cache...", out int tic1);

			R.Data.LBPlatforms.Load();
			R.Data.LBImages.Load();
			R.Data.LBReleases.Load();
			R.Data.LBGames.Load();

			platformsCached = false;

			Reporter.Toc(tic1);
		}

		public void GetLaunchBoxFile()
		{
			using (WebClient webclient = new WebClient())
			{
				if (File.Exists(LaunchBoxDataZipFile) && (File.GetLastWriteTime(LaunchBoxDataZipFile).Date == DateTime.Today))
				{
					Reporter.Report("Found up to date LaunchBox zip file.");
				}
				else
				{
					Reporter.Tic("Updating LaunchBox zip file...", out int tic1);

					webclient.DownloadFileFromDB(@"http://gamesdb.launchbox-app.com/Metadata.zip", LaunchBoxDataZipFile);
					Reporter.Toc(tic1);
				}
			}

			Reporter.Tic("Extracting info from zip file...", out int tic2);
			try
			{
				using (ZipArchive archive = ZipFile.Open(LaunchBoxDataZipFile, ZipArchiveMode.Read))
				using (var dattext = archive.GetEntry("Metadata.xml").Open())
				{
					launchboxFile = XDocument.Load(dattext);
				}
			}

			catch (InvalidDataException)
			{
				Reporter.Report("Error in LaunchBox Metadata file.");
				File.Delete(LaunchBoxDataZipFile);
				launchboxFile = null;
			}

			gameElements = launchboxFile.Root.Elements("Game").ToList();
			releaseElements = launchboxFile.Root.Elements("GameAlternateName").ToList();
			imageElements = launchboxFile.Root.Elements("GameImage").ToList();

			Reporter.Toc(tic2);
		}

		public void CachePlatformData(Platform platform)
		{
			if (!platformsCached)
			{
				 Reporter.Tic("Caching data for all platforms to save time.", out int tic1);
				if (launchboxFile == null)
				{
					GetLaunchBoxFile();
				}

				List<XElement> platformElements = launchboxFile.Root.Elements("Platform").ToList();

				foreach (XElement platformElement in platformElements)
				{
					string tempTitle = platformElement.SafeGetA("Name");

					// Fuck Gamewave
					if (Regex.IsMatch(tempTitle, @"Game*.Wave", RegexOptions.IgnoreCase))
					{
						continue;
					}

					LBPlatform lbPlatform = R.Data.LBPlatforms.Local.FirstOrDefault(x => x.Title == tempTitle);
					if (lbPlatform == null)
					{
						lbPlatform = new LBPlatform();

						App.Current.Dispatcher.Invoke(() =>
						{
							MergeWindow mergeWindow = new MergeWindow(tempTitle);

							switch (mergeWindow.ShowDialog())
							{
								case true: // Merge the new platform with existing
									lbPlatform = mergeWindow.SelectedLBPlatform;
									break;

								case false: // Add the new platform
									R.Data.LBPlatforms.Local.Add(lbPlatform);
									break;

								default:
									throw new ArgumentOutOfRangeException();
							}
						});
					}

					lbPlatform.Title = tempTitle;
					lbPlatform.Date = DateTimeRoutines.SafeGetDate(platformElement.SafeGetA("Date"));
					lbPlatform.Developer = platformElement.SafeGetA("Developer");
					lbPlatform.Manufacturer = platformElement.SafeGetA("Manufacturer");
					lbPlatform.Cpu = platformElement.SafeGetA("Cpu");
					lbPlatform.Memory = platformElement.SafeGetA("Memory");
					lbPlatform.Graphics = platformElement.SafeGetA("Graphics");
					lbPlatform.Sound = platformElement.SafeGetA("Sound");
					lbPlatform.Display = platformElement.SafeGetA("Display");
					lbPlatform.Media = platformElement.SafeGetA("Media");
					lbPlatform.Display = platformElement.SafeGetA("Display");
					lbPlatform.Controllers = platformElement.SafeGetA("MaxControllers");
					lbPlatform.Category = platformElement.SafeGetA("Category");
				}
				Reporter.Toc(tic1);

				platformsCached = true;
			}

			else
			{
				Reporter.Report("Platforms recently cached--no need to cache again");
			}
		}


		public void CachePlatformGames(Platform platform)
		{
			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			List<XElement> platformGameElements = launchboxFile.Root.Elements("Game").Where(x => x.Element("Platform").Value == platform.LBPlatform.Title).ToList();

			int gameCount = platformGameElements.Count;
			Reporter.Report("Found " + gameCount + " " + platform.LBPlatform.Title + " games in LaunchBox file.");
			int j = 0;

			foreach (XElement gameElement in platformGameElements)
			{
				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report("  Working " + j + " / " + gameCount + " " + platform.LBPlatform.Title + " games in the LaunchBox database.");
				}

				string title = gameElement.Element("Name")?.Value;

				// Don't create this game if the title or database ID is null
				if (string.IsNullOrEmpty(title) || !int.TryParse(gameElement.SafeGetA("DatabaseID"), out int id))
				{
					continue;
				}

				// Check if the game alredy exists in the local cache
				LBGame lbGame = R.Data.LBGames.Local.FirstOrDefault(x => x.ID == id);
				if (lbGame == null)
				{
					lbGame = new LBGame { ID = id };
					platform.LBPlatform.LBGames.Add(lbGame);
					Debug.WriteLine("New game: " + lbGame.Title);
				}

				// If a game has changed platforms, catch it and zero out match
				if (lbGame.LBPlatform_ID != platform.LBPlatform.ID)
				{
					lbGame.LBPlatform = platform.LBPlatform;
					Release release = R.Data.Releases.Local.FirstOrDefault(x => x.ID_LB == lbGame.ID);
					if (release != null)
					{
						release.ID_LB = null;
					}
				}

				// Set or overwrite game properties
				lbGame.Title = title;
				lbGame.Date = DateTimeRoutines.SafeGetDateTime(gameElement.SafeGetA("ReleaseDate") ?? gameElement.SafeGetA("ReleaseYear") + @"-01-01 00:00:00");

				lbGame.Overview = gameElement.Element("Overview")?.Value;
				lbGame.Genres = gameElement.Element("Genres")?.Value;
				lbGame.Developer = gameElement.Element("Developer")?.Value;

				lbGame.Publisher = gameElement.Element("Publisher")?.Value;
				lbGame.VideoURL = gameElement.Element("VideoURL")?.Value;
				lbGame.WikiURL = gameElement.Element("WikipediaURL")?.Value;
				lbGame.Players = gameElement.Element("MaxPlayers")?.Value;
			}
		}

		public void CachePlatformReleases(Platform platform)
		{
			CachePlatformGames(platform);

			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			Reporter.Report("Caching " + platform.LBPlatform.Title + " releasses.");

			List<XElement> gameReleaseElements;
			int j = 0;
			int gameCount = platform.LBPlatform.LBGames.Count;

			foreach (LBGame lbGame in platform.LBPlatform.LBGames)
			{
				long regionID;
				string regionText;

				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report("  Working " + j + " / " + gameCount + " " + platform.LBPlatform.Title + " games.");
				}

				gameReleaseElements = releaseElements.Where(x => x.Element("DatabaseID").Value == lbGame.ID.ToString()).ToList();

				// Cache releases for this game from the launchbox file
				foreach (XElement releaseElement in gameReleaseElements)
				{
					regionText = releaseElement.Element("Region")?.Value;

					if (regionText == null)
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
					}

					else if (!RegionDictionary.TryGetValue(regionText, out regionID))
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
						Reporter.Report("Couldn't find " + regionText + " in LB image dictionary.");
					}

					LBRelease lbRelease = lbGame.LBReleases.FirstOrDefault(x => x.Region_ID == regionID);

					if (lbRelease == null)
					{
						lbRelease = new LBRelease();
						lbGame.LBReleases.Add(lbRelease);
						lbRelease.Region_ID = regionID;
					}

					lbRelease.Title = releaseElement.Element("AlternateName").Value;
				}
			}
			CachePlatformImages(platform);
		}

		public void CachePlatformImages(Platform platform)
		{
			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			Reporter.Report("Caching " + platform.LBPlatform.Title + " images.");
			List<XElement> gameImageElements;
			int j = 0;
			int gameCount = platform.LBPlatform.LBGames.Count;

			foreach (LBGame lbGame in platform.LBPlatform.LBGames)
			{
				long regionID;
				string regionText;
				string fileName;

				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report("  Working " + j + " / " + gameCount + " " + platform.LBPlatform.Title + " games in the local cache.");
				}

				gameImageElements = imageElements.Where(x => x.Element("DatabaseID").Value == lbGame.ID.ToString()).ToList();

				// Cache images for this game from the launchbox file
				foreach (XElement imageElement in gameImageElements)
				{

					fileName = imageElement.Element("FileName").Value;

					// Check if image already exists in the local cache
					LBImage lbImage = R.Data.LBImages.Local.FirstOrDefault(x => x.FileName == fileName);

					if (lbImage == null)
					{
						lbImage = new LBImage { FileName = fileName };
					}

					lbImage.Type = imageElement.Element("Type")?.Value;

					regionText = imageElement.Element("Region")?.Value;
					if (regionText == null)
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
					}

					else if (!RegionDictionary.TryGetValue(regionText, out regionID))
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
						Reporter.Report("Couldn't find " + regionText + " in LB image dictionary.");
					}


					// Create a release to hold the image or attach it to it
					LBRelease lbRelease = lbGame.LBReleases.FirstOrDefault(x => x.Region_ID == regionID);
					if (lbRelease == null)
					{
						lbRelease = new LBRelease();
						lbGame.LBReleases.Add(lbRelease);
						lbRelease.Region_ID = regionID;
						lbRelease.Title = lbGame.Title;
					}

					lbRelease.LBImages.Add(lbImage);
				}
			}
		}

		public void CachePlatforms()
		{
			CachePlatformData(null);
		}

		public void ReportUpdates(bool detect)
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			if (detect)
			{
				R.Data.ChangeTracker.DetectChanges();
#if DEBUG
				Debug.WriteLine("Detect changes: " + Watch.ElapsedMilliseconds);
				Watch.Restart();
#endif
			}
			var lbReleaseEntries = R.Data.ChangeTracker.Entries<LBRelease>();
			var lbImageEntries = R.Data.ChangeTracker.Entries<LBImage>();
			var lbGameEntries = R.Data.ChangeTracker.Entries<LBGame>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int lbReleaseAddCount = lbReleaseEntries.Count(x => x.State == EntityState.Added);

			int lbImageAddCount = lbImageEntries.Count(x => x.State == EntityState.Added);
			int lbGameAddCount = lbGameEntries.Count(x => x.State == EntityState.Added);

			int lbReleaseModCount = lbReleaseEntries.Count(x => x.State == EntityState.Modified);
			int lbImageModCount = lbImageEntries.Count(x => x.State == EntityState.Modified);
			int lbGameModCount = lbGameEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("LBReleases added: " + lbReleaseAddCount + ", LBReleases updated: " + lbReleaseModCount);
			Reporter.Report("LBImages added: " + lbImageAddCount + ", LBImages updated: " + lbImageModCount);
			Reporter.Report("LBGames added: " + lbGameAddCount + ", LBgames updated: " + lbGameModCount);
		}

		public static Dictionary<string, long> RegionDictionary = new Dictionary<string, long>
		{
			{ "Asia", 1 },
			{ "Australia", 2 },
			{ "Brazil", 3 },
			{ "Canada", 4 },
			{ "China", 5 },
			{ "Denmark", 6 },
			{ "Europe", 7 },
			{ "Finland", 8 },
			{ "France", 9 },
			{ "Germany", 10 },
			{ "Hong Kong", 11 },
			{ "Italy", 12 },
			{ "Japan", 13 },
			{ "Korea", 14 },
			{ "The Netherlands", 15 },
			{ "Russia", 16 },
			{ "Spain", 17 },
			{ "Sweden", 18 },
			{ "United States", 21 },
			{ "World", 22 },
			{ "Norway", 25 },
			{ "United Kingdom", 28 },
			{ "North America", 21 },
			{ "Oceania", 2 },
			{ "South America", 3 }

		};

		public const string IMAGESURL = @"http://images.launchbox-app.com/";

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Launchbox()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{
				// free other managed objects that implement
				// IDisposable only
				//OVdata.Dispose();
			}

			// release any unmanaged objects
			// set the object references to null

			//OVdata = null;

			disposed = true;
		}
	}
}

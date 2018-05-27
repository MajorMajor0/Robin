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

namespace Robin
{
	class Launchbox : IDB
	{
		bool disposed;

		ILookup<string, XElement> gameElementLookupByPlatform;

		ILookup<string, XElement> releaseElementLookupByGameID;

		ILookup<string, XElement> imageElementLookupByGameID;

		List<XElement> platformElements;


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

			Reporter.Toc(tic1);
		}

		/// <summary>
		/// Get the xml file containing hte entire Launchbox database. Use existing downloaded file if it was modified today. Load the xml file to memory as property launchboxFile.
		/// </summary>
		void GetLaunchBoxFile()
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

			gameElementLookupByPlatform = launchboxFile.Root.Elements("Game").ToLookup(x => x.Element("Platform").Value);
			releaseElementLookupByGameID = launchboxFile.Root.Elements("GameAlternateName").ToLookup(x => x.Element("DatabaseID").Value);
			imageElementLookupByGameID = launchboxFile.Root.Elements("GameImage").ToLookup(x => x.Element("DatabaseID").Value);
			platformElements = launchboxFile.Root.Elements("Platform").ToList();

			Reporter.Toc(tic2);
		}

		/// <summary>
		/// Cache data for a selected LBPlatforms in the local database from an xml file downloaded from Launchbox.
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the LBPlatform to cache.</param>
		public void CachePlatformData(Platform platform)
		{
			Reporter.Tic($"Caching data for {platform.Title}...", out int tic1);
			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			// Create a dictionary of existing LBPlatforms to speed lookups
			Dictionary<string, LBPlatform> platformDictionary = R.Data.LBPlatforms.ToDictionary(x => x.Title);

			// Create a Hashset of LBPlatforms to store any new LBPlatforms that we discover
			HashSet<LBPlatform> newLBPlatforms = new HashSet<LBPlatform>();

			foreach (XElement platformElement in platformElements)
			{
				string tempTitle = platformElement.Element("Name").Value;

				// If a bad titl is found or the title is gamewave, just bail
				if (string.IsNullOrEmpty(tempTitle) || Regex.IsMatch(tempTitle, @"Game*.Wave", RegexOptions.IgnoreCase))
				{
					continue;
				}

#if DEBUG
				Stopwatch watch1 = Stopwatch.StartNew();
#endif
				// Check whether the LBPlatform exists before trying to add it. LBPlatforms have no ID, so check by title. If the title isn't found, this might be because the LBPlatform is new or because the title has been changed. The merge window lets the user decide.
				if (!platformDictionary.TryGetValue(tempTitle, out LBPlatform lbPlatform))
				{
					lbPlatform = new LBPlatform();

					Application.Current.Dispatcher.Invoke(() =>
					{
						MergeWindow mergeWindow = new MergeWindow(tempTitle);

						switch (mergeWindow.ShowDialog())
						{
							case true: // Merge the new platform with existing
								lbPlatform = mergeWindow.SelectedLBPlatform;
								break;

							case false: // Add the new platform
								newLBPlatforms.Add(lbPlatform);
								break;

							default:
								throw new ArgumentOutOfRangeException();
						}
					});
				}
#if DEBUG
				Debug.WriteLine("PB: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
				// Whether the LBPlatorm is new or old, overwrite everything with the newest data
				lbPlatform.Title = tempTitle;
				lbPlatform.Date = DateTimeRoutines.SafeGetDate(platformElement.SafeGetA("Date"));
				lbPlatform.Developer = platformElement.Element("Developer")?.Value ?? lbPlatform.Developer;
				lbPlatform.Manufacturer = platformElement.Element("Manufacturer")?.Value ?? lbPlatform.Manufacturer;
				lbPlatform.Cpu = platformElement.Element("Cpu")?.Value ?? lbPlatform.Cpu;
				lbPlatform.Memory = platformElement.Element("Memory")?.Value ?? lbPlatform.Memory;
				lbPlatform.Graphics = platformElement.Element("Graphics")?.Value ?? lbPlatform.Graphics;
				lbPlatform.Sound = platformElement.Element("Sound")?.Value ?? lbPlatform.Sound;
				lbPlatform.Display = platformElement.Element("Display")?.Value ?? lbPlatform.Display;
				lbPlatform.Media = platformElement.Element("Media")?.Value ?? lbPlatform.Media;
				lbPlatform.Display = platformElement.Element("Display")?.Value ?? lbPlatform.Display;
				lbPlatform.Controllers = platformElement.Element("MaxControllers")?.Value ?? lbPlatform.Controllers;
				lbPlatform.Category = platformElement.Element("Category")?.Value ?? lbPlatform.Category;
#if DEBUG
				Debug.WriteLine("PC: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
			}

			R.Data.LBPlatforms.AddRange(newLBPlatforms);
			Reporter.Toc(tic1, "all platforms cached.");
		}

		public void CachePlatformGames(Platform platform)
		{
			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			// Create a dictionary of existing Games to speed lookups
			Dictionary<long, LBGame> existingLBGameDict = R.Data.LBGames.ToDictionary(x => x.ID);

			// Create a Hashset of LBGames to store any new LBGames that we discover
			HashSet<LBGame> newLBGames = new HashSet<LBGame>();

			List<XElement> platformGameElements = gameElementLookupByPlatform[platform.LBPlatform.Title].ToList();
			int gameCount = platformGameElements.Count;
			Reporter.Report($"Found {gameCount} {platform.LBPlatform.Title} games in LaunchBox zip file.");
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
				if (string.IsNullOrEmpty(title) || !long.TryParse(gameElement.SafeGetA("DatabaseID"), out long id))
				{
					continue;
				}

				// Check if the game alredy exists in the local cache before trying to add it
				if (!existingLBGameDict.TryGetValue(id, out LBGame lbGame))
				{
					lbGame = new LBGame { ID = id };
					newLBGames.Add(lbGame);
					Debug.WriteLine("New game: " + lbGame.Title);
				}

				// If a game has changed platforms, catch it and zero out match
				if (lbGame.LBPlatform_ID != platform.LBPlatform.ID)
				{
					lbGame.LBPlatform = platform.LBPlatform;
					Release release = R.Data.Releases.FirstOrDefault(x => x.ID_LB == lbGame.ID);
					if (release != null)
					{
						release.ID_LB = null;
					}
				}

				// Set or overwrite game properties
				lbGame.Title = title;
				lbGame.Date = DateTimeRoutines.SafeGetDateTime(gameElement.SafeGetA("ReleaseDate") ?? gameElement.SafeGetA("ReleaseYear") + @"-01-01 00:00:00");

				lbGame.Overview = gameElement.Element("Overview")?.Value ?? lbGame.Overview;
				lbGame.Genres = gameElement.Element("Genres")?.Value ?? lbGame.Genres;
				lbGame.Developer = gameElement.Element("Developer")?.Value ?? lbGame.Developer;
				lbGame.Publisher = gameElement.Element("Publisher")?.Value ?? lbGame.Publisher;
				lbGame.VideoURL = gameElement.Element("VideoURL")?.Value ?? lbGame.VideoURL;
				lbGame.WikiURL = gameElement.Element("WikipediaURL")?.Value ?? lbGame.WikiURL;
				lbGame.Players = gameElement.Element("MaxPlayers")?.Value ?? lbGame.Players;
			}
			R.Data.LBGames.AddRange(newLBGames);
		}

		public void CachePlatformReleases(Platform platform)
		{
			Reporter.Tic($"Cache {platform.LBPlatform.Title} releases begun...", out int tic1);
			CachePlatformGames(platform);

			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			// Create a dictionary of existing LBReleases to speed lookups
			Dictionary<long, LBRelease> existingLBReleaseDict = R.Data.LBReleases.ToDictionary(x => x.ID);

			// Create a Hashset of LBReleases to store any new LBReleases that we discover
			HashSet<LBRelease> newLBReleases = new HashSet<LBRelease>();

			int gameCount = platform.LBPlatform.LBGames.Count;
			int j = 0;

			foreach (LBGame lbGame in platform.LBPlatform.LBGames)
			{
				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report($"  Working {j} / {gameCount} {platform.LBPlatform.Title} games.");
				}

				var gameReleaseElements = releaseElementLookupByGameID[lbGame.ID.ToString()];

				// Cache releases for this game from the launchbox file
				foreach (XElement releaseElement in gameReleaseElements)
				{
					string regionText = releaseElement.Element("Region")?.Value;
					long regionID;

					if (regionText == null)
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
					}

					else if (!RegionDictionary.TryGetValue(regionText, out regionID))
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
						Reporter.Report($"Couldn't find {regionText} in LB image dictionary.");
					}
#if DEBUG
					Stopwatch watch1 = Stopwatch.StartNew();
#endif
					LBRelease lbRelease = lbGame.LBReleases.FirstOrDefault(x => x.Region_ID == regionID);
#if DEBUG
					Debug.WriteLine($"RA: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
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
			Reporter.Toc(tic1, $"Cache {platform.LBPlatform.Title} releases finished.");
		}

		public void CachePlatformImages(Platform platform)
		{
			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}
#if DEBUG
			int i = 0;
#endif
			Dictionary<string, LBImage> existingLbImageDict = R.Data.LBImages.ToDictionary(x => x.FileName);

			Reporter.Report("Caching " + platform.LBPlatform.Title + " images.");
			int j = 0;
			int gameCount = platform.LBPlatform.LBGames.Count;

			foreach (LBGame lbGame in platform.LBPlatform.LBGames)
			{
				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report($"  Working {j} / {gameCount} {platform.LBPlatform.Title} games in the local cache.");
				}
#if DEBUG
				Stopwatch watch1 = Stopwatch.StartNew();
#endif
				var gameImageElements = imageElementLookupByGameID[lbGame.ID.ToString()];
#if DEBUG
				Debug.WriteLine("Game: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
				// Cache images for this game from the launchbox file
				foreach (XElement imageElement in gameImageElements)
				{
					string fileName = imageElement.Element("FileName")?.Value;

					// Check if image already exists in the local cache before creating a new one. Whether new or old, overwrite properties.
					if (!existingLbImageDict.TryGetValue(fileName, out LBImage lbImage))
					{
						lbImage = new LBImage { FileName = fileName };
					}

					lbImage.Type = imageElement.Element("Type")?.Value ?? lbImage.Type;

					string regionText = imageElement.Element("Region")?.Value;
					long regionID;
					if (regionText == null)
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
					}

					else if (!RegionDictionary.TryGetValue(regionText, out regionID))
					{
						regionID = CONSTANTS.UNKNOWN_REGION_ID;
						Reporter.Report("Couldn't find {regionText} in the region dictionary.");
					}
#if DEBUG
					Debug.WriteLine("IB: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
					// Create a release to hold the image or attach it to it
					LBRelease lbRelease = lbGame.LBReleases.FirstOrDefault(x => x.Region_ID == regionID);
					if (lbRelease == null)
					{
						lbRelease = new LBRelease();
						lbGame.LBReleases.Add(lbRelease);
						lbRelease.Region_ID = regionID;
						lbRelease.Title = lbGame.Title;
					}
#if DEBUG
					Debug.WriteLine("IC: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
					// This is a hack to avoid trying to add the image to multiple releases, which will bonk
					// because the foreign key relation is 1 or 0. The correct answer is to make this many-to-many,
					// but that seems like a pain in the ass since there are very few images related to more than one release.
					if (lbImage.LBRelease == null)
					{
						lbRelease.LBImages.Add(lbImage);
					}

#if DEBUG
					Debug.WriteLine("ID: " + watch1.ElapsedMilliseconds); watch1.Restart();
					Debug.WriteLine($"Image #: {i++}.");
#endif
				}
			}
		}

		public void CachePlatforms()
		{
			CachePlatformData(null);
		}

		/// <summary>
		/// Implements IDB.ReportUpdates(). Report to the UI how many database entries and of what type have been updated or added since the last save changes for a local DB cache.
		/// </summary>
		/// <param name="detect">Whether to detect changes prior to reporting. Detecting changes takes about 4 seconds. This can be set to false if no changes have been made since the last detect changes. Detecting changes is only necessary for updates, it is not necessary to detect additions.</param>
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

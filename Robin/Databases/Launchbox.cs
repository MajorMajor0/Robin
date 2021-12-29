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
//using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

using Microsoft.EntityFrameworkCore;

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

		public IEnumerable<IDBPlatform> Platforms => R.Data.Lbplatforms;

		public IEnumerable<IDBRelease> Releases => R.Data.Lbreleases;

		public bool HasRegions => true;

		XDocument launchboxFile;

		static string LaunchBoxDataZipFile = FileLocation.Data + "LBdata.zip";


		public Launchbox()
		{
			Reporter.Tic("Loading LaunchBox local cache...", out int tic1);

			R.Data.Lbplatforms.Load();
			R.Data.Lbimages.Load();
			R.Data.Lbreleases.Load();
			R.Data.Lbgames.Load();

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
		/// Cache data for a selected Lbplatforms in the local database from an xml file downloaded from Launchbox.
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the Lbplatform to cache.</param>
		public void CachePlatformData(Platform platform)
		{
			Reporter.Tic($"Caching data for {platform.Title}...", out int tic1);
			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			// Create a dictionary of existing Lbplatforms to speed lookups
			Dictionary<string, Lbplatform> platformDictionary = R.Data.Lbplatforms.ToDictionary(x => x.Title);

			// Create a Hashset of Lbplatforms to store any new Lbplatforms that we discover
			HashSet<Lbplatform> newLbplatforms = new HashSet<Lbplatform>();

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
				// Check whether the Lbplatform exists before trying to add it. Lbplatforms have no ID, so check by title. If the title isn't found, this might be because the Lbplatform is new or because the title has been changed. The merge window lets the user decide.
				if (!platformDictionary.TryGetValue(tempTitle, out Lbplatform Lbplatform))
				{
					Lbplatform = new Lbplatform();

					Application.Current.Dispatcher.Invoke(() =>
					{
						MergeWindow mergeWindow = new MergeWindow(tempTitle);

						switch (mergeWindow.ShowDialog())
						{
							case true: // Merge the new platform with existing
								Lbplatform = mergeWindow.SelectedLbplatform;
								break;

							case false: // Add the new platform
								newLbplatforms.Add(Lbplatform);
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
				Lbplatform.Title = tempTitle;
				Lbplatform.Date = DateTimeRoutines.SafeGetDate(platformElement.SafeGetA("Date"));
				Lbplatform.Developer = platformElement.Element("Developer")?.Value ?? Lbplatform.Developer;
				Lbplatform.Manufacturer = platformElement.Element("Manufacturer")?.Value ?? Lbplatform.Manufacturer;
				Lbplatform.Cpu = platformElement.Element("Cpu")?.Value ?? Lbplatform.Cpu;
				Lbplatform.Memory = platformElement.Element("Memory")?.Value ?? Lbplatform.Memory;
				Lbplatform.Graphics = platformElement.Element("Graphics")?.Value ?? Lbplatform.Graphics;
				Lbplatform.Sound = platformElement.Element("Sound")?.Value ?? Lbplatform.Sound;
				Lbplatform.Display = platformElement.Element("Display")?.Value ?? Lbplatform.Display;
				Lbplatform.Media = platformElement.Element("Media")?.Value ?? Lbplatform.Media;
				Lbplatform.Display = platformElement.Element("Display")?.Value ?? Lbplatform.Display;
				Lbplatform.Controllers = platformElement.Element("MaxControllers")?.Value ?? Lbplatform.Controllers;
				Lbplatform.Category = platformElement.Element("Category")?.Value ?? Lbplatform.Category;
#if DEBUG
				Debug.WriteLine("PC: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
			}

			R.Data.Lbplatforms.AddRange(newLbplatforms);
			Reporter.Toc(tic1, "all platforms cached.");
		}

		public void CachePlatformGames(Platform platform)
		{
			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			// Create a dictionary of existing Games to speed lookups
			Dictionary<long, Lbgame> existingLbgameDict = R.Data.Lbgames.ToDictionary(x => x.Id);

			// Create a Hashset of Lbgames to store any new Lbgames that we discover
			HashSet<Lbgame> newLbgames = new HashSet<Lbgame>();

			List<XElement> platformGameElements = gameElementLookupByPlatform[platform.Lbplatform.Title].ToList();
			int gameCount = platformGameElements.Count;
			Reporter.Report($"Found {gameCount} {platform.Lbplatform.Title} games in LaunchBox zip file.");
			int j = 0;

			foreach (XElement gameElement in platformGameElements)
			{
				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report("  Working " + j + " / " + gameCount + " " + platform.Lbplatform.Title + " games in the LaunchBox database.");
				}

				string title = gameElement.Element("Name")?.Value;

				// Don't create this game if the title or database ID is null
				if (string.IsNullOrEmpty(title) || !long.TryParse(gameElement.SafeGetA("DatabaseID"), out long id))
				{
					continue;
				}

				// Check if the game alredy exists in the local cache before trying to add it
				if (!existingLbgameDict.TryGetValue(id, out Lbgame Lbgame))
				{
					Lbgame = new Lbgame { Id = id };
					newLbgames.Add(Lbgame);
					Debug.WriteLine("New game: " + Lbgame.Title);
				}

				// If a game has changed platforms, catch it and zero out match
				if (Lbgame.LbplatformId != platform.Lbplatform.Id)
				{
					Lbgame.Lbplatform = platform.Lbplatform;
					Release release = R.Data.Releases.FirstOrDefault(x => x.ID_LB == Lbgame.Id);
					if (release != null)
					{
						release.ID_LB = null;
					}
				}

				// Set or overwrite game properties
				Lbgame.Title = title;
				Lbgame.Date = DateTimeRoutines.SafeGetDateTime(gameElement.SafeGetA("ReleaseDate") ?? gameElement.SafeGetA("ReleaseYear") + @"-01-01 00:00:00");

				Lbgame.Overview = gameElement.Element("Overview")?.Value ?? Lbgame.Overview;
				Lbgame.Genres = gameElement.Element("Genres")?.Value ?? Lbgame.Genres;
				Lbgame.Developer = gameElement.Element("Developer")?.Value ?? Lbgame.Developer;
				Lbgame.Publisher = gameElement.Element("Publisher")?.Value ?? Lbgame.Publisher;
				Lbgame.VideoUrl = gameElement.Element("VideoUrl")?.Value ?? Lbgame.VideoUrl;
				Lbgame.WikiUrl = gameElement.Element("WikipediaURL")?.Value ?? Lbgame.WikiUrl;
				Lbgame.Players = gameElement.Element("MaxPlayers")?.Value ?? Lbgame.Players;
			}
			R.Data.Lbgames.AddRange(newLbgames);
		}

		public void CachePlatformReleases(Platform platform)
		{
			Reporter.Tic($"Cache {platform.Lbplatform.Title} releases begun...", out int tic1);
			CachePlatformGames(platform);

			if (launchboxFile == null)
			{
				GetLaunchBoxFile();
			}

			// Create a dictionary of existing Lbreleases to speed lookups
			//Dictionary<long, Lbrelease> existingLbreleaseDict = R.Data.Lbreleases.ToDictionary(x => x.Id);

			// Create a Hashset of Lbreleases to store any new Lbreleases that we discover
			//HashSet<Lbrelease> newLbreleases = new HashSet<Lbrelease>();

			int gameCount = platform.Lbplatform.Lbgames.Count;
			int j = 0;

			foreach (Lbgame Lbgame in platform.Lbplatform.Lbgames)
			{
				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report($"  Working {j} / {gameCount} {platform.Lbplatform.Title} games.");
				}

				var gameReleaseElements = releaseElementLookupByGameID[Lbgame.Id.ToString()];

				// Cache releases for this game from the launchbox file
				foreach (XElement releaseElement in gameReleaseElements)
				{
					string regionText = releaseElement.Element("Region")?.Value;
					long regionID;

					if (regionText == null)
					{
						regionID = CONSTANTS.UNKNOWN_RegionId;
					}

					else if (!RegionDictionary.TryGetValue(regionText, out regionID))
					{
						regionID = CONSTANTS.UNKNOWN_RegionId;
						Reporter.Report($"Couldn't find {regionText} in LB image dictionary.");
					}
#if DEBUG
					Stopwatch watch1 = Stopwatch.StartNew();
#endif
					Lbrelease Lbrelease = Lbgame.Lbreleases.FirstOrDefault(x => x.RegionId == regionID);
#if DEBUG
					Debug.WriteLine($"RA: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
					if (Lbrelease == null)
					{
						Lbrelease = new Lbrelease();
						Lbgame.Lbreleases.Add(Lbrelease);
						Lbrelease.RegionId = regionID;
					}

					Lbrelease.Title = releaseElement.Element("AlternateName").Value;
				}
			}
			CachePlatformImages(platform);
			Reporter.Toc(tic1, $"Cache {platform.Lbplatform.Title} releases finished.");
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
			Dictionary<string, Lbimage> existingLbimageDict = R.Data.Lbimages.ToDictionary(x => x.FileName);

			Reporter.Report("Caching " + platform.Lbplatform.Title + " images.");
			int j = 0;
			int gameCount = platform.Lbplatform.Lbgames.Count;

			foreach (Lbgame Lbgame in platform.Lbplatform.Lbgames)
			{
				// Reporting only
				if ((gameCount / 10) != 0 && ++j % (gameCount / 10) == 0)
				{
					Reporter.Report($"  Working {j} / {gameCount} {platform.Lbplatform.Title} games in the local cache.");
				}
#if DEBUG
				Stopwatch watch1 = Stopwatch.StartNew();
#endif
				var gameImageElements = imageElementLookupByGameID[Lbgame.Id.ToString()];
#if DEBUG
				Debug.WriteLine("Game: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
				// Cache images for this game from the launchbox file
				foreach (XElement imageElement in gameImageElements)
				{
					string fileName = imageElement.Element("FileName")?.Value;

					// Check if image already exists in the local cache before creating a new one. Whether new or old, overwrite properties.
					if (!existingLbimageDict.TryGetValue(fileName, out Lbimage Lbimage))
					{
						Lbimage = new Lbimage { FileName = fileName };
					}

					Lbimage.Type = imageElement.Element("Type")?.Value ?? Lbimage.Type;

					string regionText = imageElement.Element("Region")?.Value;
					long regionID;
					if (regionText == null)
					{
						regionID = CONSTANTS.UNKNOWN_RegionId;
					}

					else if (!RegionDictionary.TryGetValue(regionText, out regionID))
					{
						regionID = CONSTANTS.UNKNOWN_RegionId;
						Reporter.Report("Couldn't find {regionText} in the region dictionary.");
					}
#if DEBUG
					Debug.WriteLine("IB: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
					// Create a release to hold the image or attach it to it
					Lbrelease Lbrelease = Lbgame.Lbreleases.FirstOrDefault(x => x.RegionId == regionID);
					if (Lbrelease == null)
					{
						Lbrelease = new Lbrelease();
						Lbgame.Lbreleases.Add(Lbrelease);
						Lbrelease.RegionId = regionID;
						Lbrelease.Title = Lbgame.Title;
					}
#if DEBUG
					Debug.WriteLine("IC: " + watch1.ElapsedMilliseconds); watch1.Restart();
#endif
					// This is a hack to avoid trying to add the image to multiple releases, which will bonk
					// because the foreign key relation is 1 or 0. The correct answer is to make this many-to-many,
					// but that seems like a pain in the ass since there are very few images related to more than one release.
					if (Lbimage.Lbrelease == null)
					{
						Lbrelease.Lbimages.Add(Lbimage);
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
			var LbreleaseEntries = R.Data.ChangeTracker.Entries<Lbrelease>();
			var LbimageEntries = R.Data.ChangeTracker.Entries<Lbimage>();
			var LbgameEntries = R.Data.ChangeTracker.Entries<Lbgame>();
#if DEBUG
			Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
			int LbreleaseAddCount = LbreleaseEntries.Count(x => x.State == EntityState.Added);

			int LbimageAddCount = LbimageEntries.Count(x => x.State == EntityState.Added);
			int LbgameAddCount = LbgameEntries.Count(x => x.State == EntityState.Added);

			int LbreleaseModCount = LbreleaseEntries.Count(x => x.State == EntityState.Modified);
			int LbimageModCount = LbimageEntries.Count(x => x.State == EntityState.Modified);
			int LbgameModCount = LbgameEntries.Count(x => x.State == EntityState.Modified);

			Reporter.Report("Lbreleases added: " + LbreleaseAddCount + ", Lbreleases updated: " + LbreleaseModCount);
			Reporter.Report("Lbimages added: " + LbimageAddCount + ", Lbimages updated: " + LbimageModCount);
			Reporter.Report("Lbgames added: " + LbgameAddCount + ", Lbgames updated: " + LbgameModCount);
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

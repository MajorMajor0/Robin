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

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Robin;

public class Datomatic : IDB
{
	public string Title => "Datomatic (Robin)";

	public LocalDB DB => LocalDB.Datomatic;

	public IEnumerable<IDbPlatform> Platforms =>
		R.Data.Platforms.Local.ToObservableCollection();

	public IEnumerable<IDbRelease> Releases =>
		R.Data.Releases.Local.ToObservableCollection();

	public Datomatic()
	{
		R.Data.Languages.Load();
	}

	public bool HasRegions => true;

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

		var releaseEntries = R.Data.ChangeTracker.Entries<Release>();
		var gameEntries = R.Data.ChangeTracker.Entries<Game>();
		var romEntries = R.Data.ChangeTracker.Entries<Rom>();
#if DEBUG
		Debug.WriteLine("Get entries: " + Watch.ElapsedMilliseconds);
#endif
		int gameAddCount = gameEntries.Count(x => x.State == EntityState.Added);
		int gameModCount = gameEntries.Count(x => x.State == EntityState.Modified);

		int releaseAddCount = releaseEntries.Count(x => x.State == EntityState.Added);
		int releaseModCount = releaseEntries.Count(x => x.State == EntityState.Modified);

		int romAddCount = romEntries.Count(x => x.State == EntityState.Added);
		int romModCount = romEntries.Count(x => x.State == EntityState.Modified);

		Reporter.Report("Releases added: " + releaseAddCount + ", Releases updated: " + releaseModCount);
		Reporter.Report("Games added: " + gameAddCount + ", Games updated: " + gameModCount);
		Reporter.Report("Roms added: " + romAddCount + ", Roms updated: " + romModCount);
	}

	public void CachePlatforms()
	{
		Reporter.Report("It is not possible to cache platforms for Robin/Datomatic");
	}

	public void CachePlatformGamesAsync(Platform platform)
	{
		Reporter.Report("It is not possible to cache games for Robin/Datomatic, only releases");
	}

	/// <summary>
	/// Implements IDB.CachePlatformdata() Update the local DB cache of platform associated metadata. Not fully implemented for Datomatic
	/// </summary>
	/// <param name="platform">Robin.Platform associated with the DBPlatorm to update.</param>
	public void CachePlatformData(Platform platform)
	{
		// TODO: make this copy data from DBs to Platforms
		Reporter.Report("It is not possible to cache platform data for Robin/Datomatic");
	}

	bool disposed;

	const string UNL = @".*\(Unl\).*";

	public void CachePlatformReleases(Platform platform)
	{
		if (platform.ID == CONSTANTS.Platform_ID.Arcade)
		{
			Mame.Database mame = new();
			mame.CacheReleases();
		}
		else
		{
			CacheDatomaticReleases(platform);
		}
	}

	void CacheDatomaticReleases(Platform platform)
	{
		//TODO: customize dialog window to explain what is going on here--i.e., that you have to find a datomatic file
		// Configure open file dialog box
		Microsoft.Win32.OpenFileDialog Dialog = new();
		Dialog.FileName = "Document"; // Default file name
		Dialog.DefaultExt = ".dat"; // Default file extension
		Dialog.Filter = "DAT files (.xml; .txt; .dat)|*.xml;*.txt;*.dat|ZIP files (.zip)|*.zip|All files (*.*)|*.*"; // Filter files by extension

		// Show open file dialog box
		bool? result = Dialog.ShowDialog() ?? false;

		// Process XML file

		if (result == true && File.Exists(Dialog.FileName))
		{
			Stopwatch Watch1 = Stopwatch.StartNew();
#if DEBUG
			Stopwatch Watch2 = Stopwatch.StartNew();
#endif
			XDocument DatomaticFile;

			// Unpack Datomatic from zip file if it is zipped
			if (Path.GetExtension(Dialog.FileName) == ".zip")
			{
				using ZipArchive archive = ZipFile.Open(Dialog.FileName, ZipArchiveMode.Read);
				string zipentryname = Path.GetFileNameWithoutExtension(Dialog.FileName) + ".dat";
				using var dattext = archive.GetEntry(zipentryname).Open();
				DatomaticFile = XDocument.Load(dattext);
			}

			else
			{
				DatomaticFile = XDocument.Load(Dialog.FileName);
			}

			List<Region> datomaticRegions = R.Data.Regions.Where(x => x.Datomatic != null).ToList();

			// Add release where required to make sure xelements have standardized info
			foreach (XElement gameElement in DatomaticFile.Root.Elements("game"))
			{
				if (!gameElement.Descendants("release").Any())
				{
					string elementName = gameElement.SafeGetA(attribute: "name") ?? "Unk";
					string regionName = "UNK";

					// Get the region using regex from parenthesis
					foreach (Region region in datomaticRegions)
					{
						if (Regex.IsMatch(elementName, @"\([^)]*" + region.Datomatic + @"[^)]*\)"))
						{
							regionName = region.Datomatic;
							break;
						}
					}
					if (regionName == "UNK")
					{
						foreach (Region region in datomaticRegions)
						{
							if (Regex.IsMatch(elementName, @"\([^)]*" + region.Title + @"[^)]*\)"))
							{
								regionName = region.Datomatic;
								break;
							}
						}
					}

					XElement ReleaseNode = new("release");
					ReleaseNode.Add(new XAttribute("name", elementName));
					ReleaseNode.Add(new XAttribute("region", regionName));
					gameElement.Add(ReleaseNode);
				}
			}

			try
			{
				List<XElement> parentElements = DatomaticFile.Root.Descendants("game").Where(x => x.SafeGetA(attribute: "cloneof") == null).ToList();
				List<XElement> childElements = DatomaticFile.Root.Descendants("game").Where(x => x.SafeGetA(attribute: "cloneof") != null).ToList();
#if DEBUG
				Watch2.Restart();
				Debug.WriteLine("00: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
#endif
				int j = 0;
				int parentCount = parentElements.Count;
				int releaseCount = DatomaticFile.Root.Descendants("release").Count();
				int romCount = DatomaticFile.Root.Descendants("rom").Count();

				Reporter.Report("Found " + parentCount + " games, " + romCount + " ROMs and " + releaseCount + " releases.");

				//Extract releases from the datomatic file
				foreach (XElement parentElement in parentElements)
				{
					// For reporting only
					if (j++ % (parentCount / 10) == 0)
					{
						Reporter.Report("Getting " + j + @" / " + parentCount + " " + Watch1.Elapsed.ToString(@"m\:ss") + " elapsed.");
						Watch1.Restart();
					}

					Game game = null;
					string parentTitle = parentElement.SafeGetA(attribute: "name");
#if DEBUG
					Watch2.Restart();
#endif
					// Collect all child roms
					List<XElement> romElements = childElements.Where(x => x.SafeGetA(attribute: "cloneof") == parentTitle).ToList();
					romElements.Insert(0, parentElement);
#if DEBUG
					Debug.WriteLine("A: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
#endif

					// Check if game exists
					foreach (XElement romElement in romElements)
					{
						string romElementSHA1 = romElement.SafeGetA(element1: "rom", attribute: "SHA1");
#if DEBUG
						Watch2.Restart();
#endif
						Release release = platform.Releases.FirstOrDefault(x => x.Rom.SHA1 == romElementSHA1);
						if (release != null)
						{
							game = release.Game;
							break; // Game exists--no need to keep looking
						}
#if DEBUG
						Debug.WriteLine("AA: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
#endif
					}
#if DEBUG
					Debug.WriteLine("B: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
#endif
					// If the game wasn't found, create a new one and add it
					if (game == null)
					{
						game = new Game();
						R.Data.Games.Add(game);
					}

					// Check if each rom exists
					foreach (XElement romElement in romElements)
					{
						string romElementSHA1 = romElement.SafeGetA(element1: "rom", attribute: "SHA1");
						if (romElementSHA1 == null)
						{
							continue; // Malformed element
						}

						var rom = R.Data.Roms.FirstOrDefault(x => x.SHA1 == romElementSHA1);

						// Not found, create a new one
						if (rom == null)
						{
							rom = new Rom();// { Platform_ID = platform.ID };
							R.Data.Roms.Add(rom);
						}
#if DEBUG
						Watch2.Restart();
#endif
						// Whether existing or new, overwrite properties with new data
						ParseElementToRom(romElement, rom);
#if DEBUG
						Debug.WriteLine("C: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
#endif
						// Get the releases from the rom element
						foreach (XElement releaseElement in romElement.Descendants("release"))
						{
							//string releaseTitle = releaseElement.SafeGetA(attribute: "name");
							string releaseRegionTitle = releaseElement.SafeGetA(attribute: "region");

							if (releaseRegionTitle == null)
							{
								Reporter.Report("Skipped release (SHA1: " + romElementSHA1 + ") because the datomatic file lists it with no region");
								continue;
							}

							long? regionID = R.Data.Regions.FirstOrDefault(x => (x.Datomatic == releaseRegionTitle) || (x.Title == releaseRegionTitle)).ID;

							if (regionID == null)
							{
								Reporter.Report("Skipped Datomatic release (SHA1: " + romElementSHA1 + ", Region: " + releaseRegionTitle + ") because the region wasn't recognized. Consider adding this region to the database");
								continue;
							}
#if DEBUG
							Watch2.Restart();
#endif
							Release release = platform.Releases.FirstOrDefault(x => x.Rom_ID == rom.ID && x.Region_ID == regionID);
							if (release == null)
							{
								release = new Release
								{
									Game = game,
									Rom = rom,
									Region_ID = (long)regionID
								};
								platform.Releases.Add(release);
							}
#if DEBUG
							Debug.WriteLine("D: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
#endif
							ParseElementToRelease(releaseElement, release);
#if DEBUG
							Debug.WriteLine("E: " + Watch2.ElapsedMilliseconds); Watch2.Restart();
#endif
						}
					}
				}

				// Update platform cache date
				string dateString = DatomaticFile.SafeGetB("header", "date");
				DateTime cacheDate;
				if (dateString != null)
				{
					CultureInfo enUS = new("en-US");
					if (DateTime.TryParseExact(dateString, "yyyyMMdd-hhmmss", enUS,
						 DateTimeStyles.None, out cacheDate))
					{
						platform.CacheDate = cacheDate;
					}
					else
					{
						platform.CacheDate = DateTime.Now;
					}
				}

				Reporter.Report("Finished.");
			}

			catch (NullReferenceException)
			{
				// Ignore
			}

		}
		else
		{
			Reporter.Report("File not found.");
		}
	}

	void ParseElementToRelease(XElement releaseElement, Release release)
	{
		// Get release properties from release node
		release.Title = releaseElement.SafeGetA(attribute: "name");

		// Check if release is unlicensed
		release.Unlicensed = Regex.IsMatch(release.Title, UNL);

		// Check if release is a BIOS
		if (release.Title.Contains(@"[BIOS]"))
		{
			release.IsGame = false;
		}

		else
		{
			release.IsGame = true;
		}

		// Get languages from datomatic using regex from parenthesis
		release.Language = "";

		foreach (Language language in R.Data.Languages)
		{
			if (Regex.IsMatch(release.Title, @"\([^)]*" + language.Abbreviation + @"[^)]*\)"))
			{
				release.Language += language.Abbreviation + ",";
			}
		}
		release.Language = release.Language.TrimEnd(',');

		// Get special info using regex from parenthesis
		release.Special = "";

		MatchCollection matchList = Regex.Matches(release.Title, @"\([^)]*\)");
		var Matches = matchList.Cast<System.Text.RegularExpressions.Match>().Select(match => match.Value).ToList();

		Matches.RemoveAll(x => Regex.IsMatch(x, @".*Rev.*|.*v.*|.*Beta.*|.*Proto.*|.*NTSC.*|.*PAL.*|.*Unl.*|.*Japan.*|.*USA.*"));

		for (int i = 0; i < Matches.Count; i++)
		{
			Matches[i] = Matches[i].Replace("(", "").Replace(")", "");
		}

		foreach (Region region in R.Data.Regions)
		{
			Matches.Remove(region.Title);
		}

		Matches.Remove(release.Language);
		string languages = @"\w{2},\w{2},.*";
		Matches.RemoveAll(x => Regex.IsMatch(x, languages));

		if (Matches.Any())
		{
			release.Special = string.Join(", ", Matches);
		}

		// Get version from datomatic using regex from parenthesis
		release.Version = Regex.Match(release.Title, @"(?<=\()Rev [^)]*(?=\))|(?<=\()v[^)]*(?=\))|(?<=\()Beta[^)]*(?=\))|(?<=\()Proto[^)]*(?=\))").ToString();

		// Get the video format from datomatic using regex from parenthesis
		release.VideoFormat = Regex.Match(release.Title, @"(?<=\()NTSC(?=\))|(?<=\()PAL(?=\))").ToString();

		// Remove all parenthetical info from title
		release.Title = Regex.Replace(release.Title, @"\([^)]*\)", string.Empty);
		release.Title = release.Title.TrimEnd(' ');



		// Move "the" to front of title
		if (release.Title.Length > 3 && release.Title.Substring(release.Title.Length - 3, 3).ToLower() == "the")
		{
			release.Title = "The " + release.Title[0..^5];
		}
	}

	static void ParseElementToRom(XElement romElement, Rom rom)
	{
		// Get rom properties from parent node
		rom.Source = "Datomatic";
		rom.CRC32 = romElement.SafeGetA(element1: "rom", attribute: "crc");
		rom.MD5 = romElement.SafeGetA(element1: "rom", attribute: "MD5");
		rom.SHA1 = romElement.SafeGetA(element1: "rom", attribute: "SHA1");
		rom.Size = romElement.SafeGetA(element1: "rom", attribute: "size");
		rom.Title = romElement.SafeGetA(element1: "rom", attribute: "name");
		rom.Title = Path.GetFileNameWithoutExtension(rom.Title);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~Datomatic()
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
			// R.Data.GBPlatforms.Dispose();
		}

		// release any unmanaged objects
		// set the object references to null

		//R.Data = null;

		disposed = true;
	}
}

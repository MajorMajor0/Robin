﻿/*This file is part of Robin.
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Robin.Mame;

public class Database
{
	readonly Platform arcadePlatform;
	//readonly List<Region> notNullRegions;

	//public Entities MEntities { get; set; }

	public Database()
	{
		arcadePlatform = R.Data.Platforms.FirstOrDefault(x => x.ID == CONSTANTS.Platform_ID.Arcade);
		//notNullRegions = R.Data.Regions.Where(x => x.Priority != null).OrderByDescending(x => x.Priority).ToList();
	}

	/// <summary>
	/// Legacy function to cache releases from direct MAME output. Caches only working releases. To be deprecated.
	/// </summary>
	public void CacheReleases()
	{
		Stopwatch Watch = Stopwatch.StartNew();
		Stopwatch Watch1 = Stopwatch.StartNew();

		List<XElement> machineElements = new();

		Reporter.Report("Getting xml file from MAME...");

		// Scan through xml file from MAME and pick out working games
		XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Parse };
		using (Process process = MAMEexe(@"-lx"))
		using (XmlReader reader = XmlReader.Create(process.StandardOutput, settings))
		{
			int machineCount = 0;
			while (reader.Read())
			{
				if (reader.Name == "machine")
				{
					XElement machineElement = XNode.ReadFrom(reader) as XElement;
					string emulationStatus = machineElement.SafeGetA(element1: "driver", attribute: "emulation");
					string publisher = machineElement.SafeGetA("manufacturer");

					if ((emulationStatus == "good") && !Regex.IsMatch(publisher, @"hack|bootleg"))
					{
						machineElements.Add(machineElement);

						if (++machineCount % 100 == 0)
						{
							Reporter.Report(machineCount + " machines");
						}
					}
				}
			}
		}

		List<XElement> parentElements = machineElements.Where(x => x.SafeGetA(attribute: "cloneof") == null).ToList();
		List<XElement> childElements = machineElements.Where(x => x.SafeGetA(attribute: "cloneof") != null).ToList();

		int parentCount = parentElements.Count;
		int childCount = childElements.Count;
		int j = 0;
		Reporter.Report("Found " + parentCount + " parent machines and " + childCount + " child machines.");
		foreach (XElement parentElement in parentElements)
		{
			if (++j % (parentCount / 10) == 0)
			{
				Reporter.Report("Working " + j + " / " + parentCount + " parent machines.");
			}

			Game game = null;
			string parentName = parentElement.SafeGetA(attribute: "name");
			List<XElement> romElements = childElements.Where(x => x.SafeGetA(attribute: "cloneof") == parentName).ToList();
			romElements.Insert(0, parentElement);

			// Check if game exists
			foreach (XElement romElement in romElements)
			{
				string romElementName = romElement.SafeGetA(attribute: "name") + ".zip";
				Release tRelease = arcadePlatform.Releases.FirstOrDefault(x => x.Rom.FileName == romElementName);
				if (tRelease != null)
				{
					game = tRelease.Game;
					break; // Game exists--no need to keep looking
				}
			}

			if (game == null)
			{
				game = new Game();
				R.Data.Games.Add(game);
			}

			// Check if each rom exists and each release exists
			foreach (XElement romElement in romElements)
			{
				string romElementName = romElement.SafeGetA(attribute: "name") + ".zip";
				var rom = arcadePlatform.Roms.FirstOrDefault(x => x.FileName == romElementName);

				// Not found, create a new one
				if (rom == null)
				{
					rom = new Robin.Rom();
					arcadePlatform.Roms.Add(rom);
				}

				rom.Title = romElement.SafeGetA("description");
				rom.FileName = romElementName;
				rom.Source = "MAME";

				var release = arcadePlatform.Releases.FirstOrDefault(x => x.Rom.FileName == romElementName);
				if (release == null)
				{
					release = new Release { Rom = rom };
					arcadePlatform.Releases.Add(release);
				}

				release.Title = rom.Title;
				release.Game = game;
				ParseReleaseFromElement(romElement, release);
			}
		}
		List<Release> orphanReleases = arcadePlatform.Releases.Where(x => x.Game == null).ToList();
		int orphanCount = orphanReleases.Count;

		if (orphanCount > 0)
		{
			// Get list of clones from MAME to help clean up orphans
			Reporter.Report("Getting list of clones...");
			Watch1.Restart();

			List<string[]> cloneList = CloneList();

			j = 0;

			Reporter.ReportInline(Watch1.Elapsed.ToString(@"m\:ss"));
			Reporter.Report("Cleaning up " + orphanCount + " orphans...");
			Watch1.Restart();

			foreach (Release orphanRelease in orphanReleases)
			{
				if (++j % (parentCount / 10) == 0)
				{
					Reporter.Report("Working " + j + " / " + orphanCount + " orphans.");
				}

				string[] clonePair = cloneList.FirstOrDefault(x => x[0] + @".zip" == orphanRelease.Rom.FileName);

				// First look through arcadePlatform to see if the parent is there
				if (arcadePlatform.Releases.Any(x => x.Rom.FileName == clonePair[1] + @".zip"))
				{
					Release parentRelease = arcadePlatform.Releases.FirstOrDefault(x => x.Rom.FileName == clonePair[1] + @".zip");
					orphanRelease.Game = parentRelease.Game;
				}

				// If the parent is not in arcadePlatform, go back to mame to get the parent
				else
				{
					using Process process1 = MAMEexe(@"-lx " + clonePair[1]);
					using StreamReader MameOutput = process1.StandardOutput;
					string text = MameOutput.ReadToEnd();
					XElement machineElement = XElement.Parse(text).Element("machine");

					Release parentRelease = new();
					Game parentGame = new();

					ParseReleaseFromElement(machineElement, parentRelease);

					parentRelease.Game = parentGame;
					orphanRelease.Game = parentGame;
					R.Data.Games.Add(parentGame);
					arcadePlatform.Releases.Add(parentRelease);

					Debug.WriteLine(machineElement.SafeGetA(element1: "driver", attribute: "status"));
				}
			}
		}
		else
		{
			Reporter.Report("No orphans found.");
		}

		Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
	}

	/// <summary>
	/// Cache releases from local MAME cache to Robin database. Caches only working releases. Sets parents and samples.
	/// </summary>
	//public void CacheReleases2()
	//{
	//	Dictionary<string, Machine> newMachines = M.Data.Machines.Where(x => x.Status == "good" && !Regex.IsMatch(x.Manufacturer, @"hack|bootleg")).ToDictionary(x => x.Name);
	//	HashSet<Release> newReleases = new HashSet<Release>();
	//	Dictionary<string, Release> existingReleases = arcadePlatform.Releases.ToDictionary(x => x.Rom.FileName.Replace(".zip", ""));

	//	// Add new machines to newReleases
	//	foreach (Machine machine in newMachines.Values)
	//	{
	//		if (!existingReleases.TryGetValue(machine.Name, out Release release))
	//		{
	//			release = new Release();
	//			Robin.Rom rom = new Robin.Rom();
	//			rom.FileName = $"{machine.Name}.zip";
	//			rom.Source = "MAME";
	//			release.Rom = rom;
	//			newReleases.Add(release);
	//		}

	//		release.Publisher = machine.Manufacturer;
	//		release.Players = machine.Players;

	//		if (DateTime.TryParseExact(machine.Year, "yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date))
	//		{
	//			release.Date = date;
	//		}

	//		string parenthesisText = machine.ParenthesisText;

	//		// Get version, remove region and store remaining text as special
	//		string versionFind = @"V\s(\d|\w|\.)*";
	//		string revisionFind = @"Rev\s(\d+|\w|\.)*";

	//		release.Version = Regex.Match(parenthesisText, revisionFind).Value ?? Regex.Match(parenthesisText, versionFind).Value;

	//		release.Special = parenthesisText.Replace(@"(", "").Replace(@")", "");
	//		release.Special = Regex.Replace(release.Special, revisionFind, "");
	//		release.Special = Regex.Replace(release.Special, versionFind, "");
	//		release.Special = Regex.Replace(release.Special, @"(^(\s|,)+)", "");
	//		release.Special = Regex.Replace(release.Special, @"\s+", " ");

	//		// Remove all prenthesis text and store title
	//		release.Title = Regex.Replace(release.Title, @"\s\(.*\)", "");
	//	}

	//	// Add new Releases to database and refresh dictionary
	//	arcadePlatform.Releases.AddRange(newReleases);
	//	existingReleases = arcadePlatform.Releases.ToDictionary(x => x.Rom.FileName.Replace(".zip", ""));

	//	// Assign parents
	//	Reporter.Tic("Validating parents...", out int tic5);
	//	int i = 0;
	//	ILookup<string, Machine> childMachines = newMachines.Values.Where(x => x.Parent_Name != null).ToLookup(x => x.Parent_Name);

	//	List<Release> orphanReleases = existingReleases.Values.Where(x => x.Game == null).ToList();

	//	foreach(Release orphanRelease in orphanReleases)
	//	{
	//		string machineName = orphanRelease.Rom.FileName.Replace(".zip", "");
	//		string possibleParentName = newMachines[machineName].Parent_Name;

	//		if (existingReleases.TryGetValue(possibleParentName, out Release brother))
	//		{
	//			orphanRelease.Game = brother.Game;
	//		}

	//		else
	//		{
	//			Game game = new();
	//		}



	//	}


	//	foreach (Machine child in childMachines)
	//	{
	//		if (existingReleases.TryGetValue(child.Parent_Name, out Release brother))
	//		{
	//			existingReleases[child.Name].Game = brother.Game;
	//		}

	//		if (!machines.ContainsKey(child.Parent_Name))
	//		{
	//			foreach (Machine brother in childMachines[child.Parent_Name])
	//			{
	//				brother.Parent_Name = child.Parent_Name;
	//			}
	//			child.Parent_Name = null;
	//			Debug.WriteLine($"-Parent assigned {i++}");
	//		}
	//	}
	//	Reporter.Toc(tic5);

	//	//Reporter.Tic("Validating samples...", out int tic6);
	//	//i = 0;

	//	//childMachines = machines.Values.Where(x => x.Sample_Name != null).ToLookup(x => x.Sample_Name);

	//	//foreach (Machine child in childMachines)
	//	//{
	//	//	Debug.WriteLine($"{child.Name}, Clone: = {child.Name}");
	//	//	if (!machines.ContainsKey(child.Sample_Name))
	//	//	{
	//	//		foreach (Machine brother in childMachines[child.Sample_Name])
	//	//		{
	//	//			brother.Sample_Name = child.Sample_Name;
	//	//		}
	//	//		child.Sample_Name = null;
	//	//		Debug.WriteLine($"-Parent assigned {i++}");
	//	//	}
	//	//}
	//	//Reporter.Toc(tic6);

	//}

	/// <summary>
	/// Cache direct MAME output to local DB cache MAMe.db
	/// </summary>
	//public void CacheDataBase3()
	//{
	//	Stopwatch Watch = Stopwatch.StartNew();
	//	Stopwatch Watch1 = Stopwatch.StartNew();

	//	XmlReaderSettings settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };

	//	// Wipe tables
	//	Reporter.Tic("Wiping tables.", out int tic1);
	//	string query = @"DELETE FROM Machine_Disk;
	//					DELETE FROM Machine_Rom;
	//					DELETE FROM Machine;
	//					DELETE FROM Disk;
	//					DELETE FROM Rom;
	//					DELETE FROM sqlite_sequence;";

	//	M.Data.Database.ExecuteSqlRaw(query);
	//	Reporter.Toc(tic1);

	//	//Reload M.Data so it knows about the wipe
	//	M.Refresh(false);
	//	M.Data.ChangeTracker.AutoDetectChangesEnabled = false;

	//	int machineCount = 0;
	//	int romCount = 0;
	//	int diskCount = 0;

	//	Dictionary<string, Machine> machines = new Dictionary<string, Machine>();
	//	Dictionary<string, Rom> roms = new Dictionary<string, Rom>();
	//	Dictionary<string, Disk> disks = new Dictionary<string, Disk>();

	//	// Scan through xml file from MAME and store machines
	//	using (Process process = MAMEexe(@"-lx"))
	//	using (XmlReader reader = XmlReader.Create(process.StandardOutput, settings))
	//	{
	//		Reporter.Tic("Getting xml file from MAME...", out int tic2);
	//		while (reader.Read())
	//		{
	//			if (reader.Name == "machine")
	//			{
	//				XElement machineElement = XNode.ReadFrom(reader) as XElement;
	//				Machine machine = new Machine(machineElement);
	//				machines.Add(machine.Name, machine);
	//				//machine.ID = machineCount;

	//				foreach (XElement romElement in machineElement.Elements("rom"))
	//				{
	//					string crc = romElement.Attribute("crc")?.Value ?? "NONE" + romCount;
	//					if (!roms.TryGetValue(crc, out Rom rom))
	//					{
	//						rom = new Rom(romElement);
	//						roms.Add(crc, rom);
	//					}
	//					machine.Roms.Add(rom);
	//					rom.ID = ++romCount;
	//				}

	//				foreach (XElement diskElement in machineElement.Elements("disk"))
	//				{
	//					string SHA1 = diskElement.Attribute("SHA1")?.Value ?? "NONE" + diskCount;

	//					if (!disks.TryGetValue(SHA1, out Disk disk))
	//					{
	//						disk = new Disk(diskElement);
	//						disks.Add(SHA1, disk);
	//					}
	//					machine.Disks.Add(disk);
	//					disk.ID = ++diskCount;
	//				}

	//				if (++machineCount % 5000 == 0)
	//				{
	//					Reporter.Report(machineCount + " machines, " + Watch1.Elapsed.TotalSeconds + " s.");
	//					Watch1.Restart();
	//				}
	//			}
	//		}
	//		Reporter.Toc(tic2);
	//	}

	//	// Add machines to local db Cache
	//	Reporter.Tic("Adding machines...", out int tic3);
	//	M.Data.Machines.AddRange(machines.Select(x => x.Value));
	//	Reporter.Toc(tic3);

	//	Reporter.Tic("Saving changes...", out int tic4);
	//	M.Data.Save(false);
	//	Reporter.Toc(tic4);

	//	//Reporter.Tic("Validating parents...", out int tic5);
	//	//int i = 0;

	//	//ILookup<string, Machine> childMachines = machines.Values.Where(x => x.Parent_Name != null).ToLookup(x => x.Parent_Name);

	//	//foreach (Machine child in childMachines)
	//	//{
	//	//	Debug.WriteLine($"{child.Name}, Clone: = {child.Name}");
	//	//	if (!machines.ContainsKey(child.Parent_Name))
	//	//	{
	//	//		foreach (Machine brother in childMachines[child.Parent_Name])
	//	//		{
	//	//			brother.Parent_Name = child.Parent_Name;
	//	//		}
	//	//		child.Parent_Name = null;
	//	//		Debug.WriteLine($"-Parent assigned {i++}");
	//	//	}
	//	//}
	//	//Reporter.Toc(tic5);

	//	//Reporter.Tic("Validating samples...", out int tic6);
	//	//i = 0;

	//	//childMachines = machines.Values.Where(x => x.Sample_Name != null).ToLookup(x => x.Sample_Name);

	//	//foreach (Machine child in childMachines)
	//	//{
	//	//	Debug.WriteLine($"{child.Name}, Clone: = {child.Name}");
	//	//	if (!machines.ContainsKey(child.Sample_Name))
	//	//	{
	//	//		foreach (Machine brother in childMachines[child.Sample_Name])
	//	//		{
	//	//			brother.Sample_Name = child.Sample_Name;
	//	//		}
	//	//		child.Sample_Name = null;
	//	//		Debug.WriteLine($"-Parent assigned {i++}");
	//	//	}
	//	//}
	//	//Reporter.Toc(tic6);

	//	M.Data.Save(true);

	//	Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
	//}

	private static ProcessStartInfo MAMEProcess(string arguments = null)
	{
		ProcessStartInfo MAMEexe = new()
		{
			FileName = FileLocation.MAME,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			WindowStyle = ProcessWindowStyle.Hidden,
			CreateNoWindow = true,
			Arguments = arguments
		};

		return MAMEexe;
	}

	private static List<string[]> CloneList()
	{
		ProcessStartInfo MAMEexe = MAMEProcess(@"-lc");

		List<string[]> cloneList = new();
		string line;
		string[] separators = { " " };

		using (Process process = Process.Start(MAMEexe))
		using (StreamReader streamReader = process.StandardOutput)
		{
			while ((line = streamReader.ReadLine()) != null)
			{
				cloneList.Add(line.Split(separators, StringSplitOptions.RemoveEmptyEntries));
			}
		}
		return cloneList;
	}

	public static Process MAMEexe(string arguments)
	{
		ProcessStartInfo mameexe = MAMEProcess(arguments);
		return Process.Start(mameexe);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xelement"></param>
	/// <param name="release"></param>
	void ParseReleaseFromElement(XElement xelement, Release release)
	{
		release.Publisher = xelement.SafeGetA("manufacturer");
		release.Players = xelement.SafeGetA(element1: "input", attribute: "players");

		// Get date
		if (DateTime.TryParseExact(xelement.SafeGetA("year"), "yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime datecatcher))
		{
			release.Date = datecatcher;
		}

		// Standardize version/revision text within parenthesis
		//Regex Rparenthesis = new Regex(@"\(.*\)");
		string parenthesisText = Machine.GetParenthesisText(release.Title);

		// Get region from text in parenthesis
		foreach (Region region in R.Data.Regions)
		{
			if (parenthesisText != null && (parenthesisText.Contains(region.Title) || parenthesisText.Contains(region.Datomatic ?? "XXX") || parenthesisText.Contains(region.UNCode ?? "XXX")))
			{
				release.Region_ID = region.ID;

				parenthesisText = Regex.Replace(parenthesisText, region.Title + @"|" + region.Datomatic + @"|" + region.UNCode, string.Empty);

				break;
			}
		}

		// Get version, remove region and store remaining text as special
		string versionFind = @"V\s(\d|\w|\.)*";
		string revisionFind = @"Rev\s(\d+|\w|\.)*";

		release.Version = Regex.Match(parenthesisText, revisionFind).Value ?? Regex.Match(parenthesisText, versionFind).Value;

		release.Special = parenthesisText.Replace(@"(", "").Replace(@")", "");
		release.Special = Regex.Replace(release.Special, revisionFind, "");
		release.Special = Regex.Replace(release.Special, versionFind, "");
		release.Special = Regex.Replace(release.Special, @"(^(\s|,)+)", "");
		release.Special = Regex.Replace(release.Special, @"\s+", " ");

		release.Title = Regex.Replace(release.Title, @"\s\(.*\)", "");
	}

	//private static void ParseParenthesis(Release release, string text)
	//{

	//}

	/// <summary>
	/// Audit MAME ROMs currently used by Robin using MAME.exe -verifyroms command line switch
	/// </summary>
	public static TitledCollection<Audit.Result> AuditRoms()
	{
		Reporter.Report("Auditing MAME ROMs");
		Emulator mame = R.Data.Emulators.FirstOrDefault(x => x.ID == CONSTANTS.Emulator_ID.Mame);

		List<string> arguments = GetListOfCurrentRoms();

		List<string> resultStrings = new();

		int k = 0;
		int N = arguments.Count;
		foreach (string argument in arguments)
		{
			using Process emulatorProcess = new();
			emulatorProcess.StartInfo.CreateNoWindow = true;
			emulatorProcess.StartInfo.UseShellExecute = false;
			emulatorProcess.StartInfo.RedirectStandardOutput = true;
			emulatorProcess.StartInfo.RedirectStandardError = true;
			emulatorProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(mame.FilePath);

			emulatorProcess.StartInfo.FileName = mame.FilePath;
			emulatorProcess.StartInfo.Arguments = argument;
			Reporter.Tic("Getting batch " + ++k + " / " + N + " from MAME...", out int tic1);
			try
			{
				emulatorProcess.Start();
			}
			catch (Exception)
			{
				Reporter.Report("MAME process failed to start.");
			}

			string output = emulatorProcess.StandardOutput.ReadToEnd();
			Reporter.Toc(tic1);
			string error = emulatorProcess.StandardError.ReadToEnd();

			Reporter.Tic("Listing results...", out int tic2);
			List<string> lines = output.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			resultStrings.AddRange(lines);
			Reporter.Toc(tic2);
		}

		TitledCollection<Audit.Result> auditResults = new("Arcade");
		foreach (string line in resultStrings)
		{
			if (!line.Contains(": "))
			{
				auditResults.Add(Audit.GetResultFromMameLine(line));
			}
		}

		return auditResults;
	}

	/// <summary>
	/// Return a list of roms currently in Robin and return them formatted for use in MAME verify ROMs
	/// Arguments are broke up into 32k long chunks for use in command line
	/// </summary>
	/// <returns></returns>
	static List<string> GetListOfCurrentRoms()
	{
		Reporter.Report("Getting roms from Robin...");
		List<string> returner = new();
		Platform arcadePlatform = R.Data.Platforms.FirstOrDefault(x => x.ID == CONSTANTS.Platform_ID.Arcade);
		string[] arcadeRoms = arcadePlatform.Releases.Select(x => x.Rom.FileName.Replace(@".zip", "")).OrderBy(x => x).ToArray();

		int i = 0;
		int j = 0;
		while (j < arcadeRoms.Length)
		{
			StringBuilder argument = new("-verifyroms ");

			// Arguments have to be less than about 32k or windows bonks the command line
			while (argument.Length < 32000 && j < arcadeRoms.Length)
			{
				argument.Append(' ').Append(arcadeRoms[j++]);
			}
			returner.Add(argument.ToString());

			Reporter.ReportInline(i++ + " ");
		}
		Reporter.Report("Finished getting ROMs from Robin.");
		return returner;
	}

	//string MachineDictionaryToSql(Machine machine)
	//{
	//	StringBuilder returner = new StringBuilder("INSERT INTO Machine (Name, Description, Year, Manufacturer, Status, Emulation, Players, Display, Control, Parent_Name, Sample_NAme, IsMechanical, IsDevice, IsBios, IsRunnable) VALUES (");
	//	returner.Append(machine.Name).Append(",").
	//		Append("'").Append(machine.Name).Append("','").
	//		Append(machine.Description.Replace("'", "''")).Append("','").
	//		Append(machine.Year ?? "xkcd").Append("','").
	//		Append((machine.Manufacturer ?? "xkcd").Replace("'", "''")).Append("','").
	//		Append(machine.Status).Append("','").
	//		Append(machine.Emulation).Append("','").
	//		Append(machine.Players ?? "xkcd").Append("','").
	//		Append(machine.Display ?? "xkcd").Append("','").
	//		Append(machine.Control ?? "xkcd").Append("',").
	//		Append(machine.Parent_Name == null ? "null" : machine.Parent_Name).Append(",").
	//		Append(machine.Sample_Name == null ? "null" : machine.Parent_Name).Append(",").
	//		Append(machine.IsMechanical ? "1" : "0").Append(",").
	//		Append(machine.IsDevice ? "1" : "0").Append(",").
	//		Append(machine.IsBios ? "1" : "0").Append(",").
	//		Append(machine.IsRunnable ? "1" : "0").Append(");").Replace("'xkcd'", "null");
	//	return returner.ToString();
	//}

	//string RomDictionaryToSql(Rom rom)
	//{
	//	StringBuilder returner = new StringBuilder("INSERT INTO Rom (ID, Name, Region, CRC, Size, Status, Optional) VALUES (");
	//	returner.Append(rom.ID).Append(",").
	//		Append("'").Append(rom.Name.Replace("'", "''")).Append("','").
	//		Append((rom.Region ?? "xkcd").Replace("'", "''")).Append("','").
	//		Append(rom.CRC ?? "xkcd").Append("',").
	//		Append(rom.Size == null ? "null" : rom.Size.ToString()).Append(",'").
	//		Append(rom.Status).Append("',").
	//		Append(rom.Optional ? "1" : "0").Append(");").Replace("'xkcd'", "null");
	//	return returner.ToString();
	//}

	///// <summary>
	///// Add a zipfile potentially containing roms to the database for later analysis
	///// </summary>
	//public static void GetFromZipFile()
	//{
	//	// Wipe tables
	//	Reporter.Tic("Wiping tables...", out int tic1);
	//	string query = @"DELETE FROM RomFile_Rom;
	//					DELETE FROM RomFile;
	//					DELETE FROM sqlite_sequence WHERE NAME = 'RomFile';";
	//	M.Data.Database.ExecuteSqlCommand(query);
	//	Reporter.Toc(tic1);

	//	Reporter.Tic("Refreshing DB...", out int tic2);
	//	M.Refresh(true);
	//	Reporter.Toc(tic2);

	//	RobinDataEntities RData = new RobinDataEntities();
	//	Platform arcade = RData.Platforms.FirstOrDefault(x => x.ID == CONSTANTS.ARCADE_Platform_ID);

	//	string[] files = Directory.GetFiles(arcade.RomDirectory);

	//	//try
	//	//{
	//	Reporter.Tic("Creating dictionary...", out int tic3);
	//	//ConcurrentDictionary<string, RomFile> romFiles = new ConcurrentDictionary<string, RomFile>();
	//	Dictionary<string, RomFile> romFiles = new Dictionary<string, RomFile>();
	//	Dictionary<string, Rom> roms = M.Data.Roms.ToDictionary(x => x.CRCN, y => y);
	//	Dictionary<string, Machine> machines = M.Data.Machines.ToDictionary(x => x.Name, y => y);
	//	Reporter.Toc(tic3);

	//	Stopwatch Watch = Stopwatch.StartNew();
	//	Stopwatch Watch1 = Stopwatch.StartNew();

	//	int i = 0;

	//	foreach (string file in files.Where(x => x.EndsWith(".zip")))
	//	{
	//		if (!romFiles.ContainsKey(file))
	//		{
	//			RomFile romFile = new RomFile
	//			{
	//				FilePath = file
	//			};

	//			romFiles.Add(file, romFile);

	//			using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(file))
	//			{
	//				foreach (ZipEntry entry in zipFile)
	//				{
	//					if (roms.TryGetValue(entry.Crc.ToString("X"), out Rom rom))
	//					{
	//						romFile.Roms.Add(rom);
	//					}

	//					else
	//					{
	//						rom = new Rom
	//						{
	//							Name = entry.FileName,
	//							CRC = entry.Crc.ToString("X"),
	//							Size = entry.UncompressedSize,
	//							Unknown = true
	//						};
	//					}
	//				}
	//			}
	//		}

	//		if (++i % 1000 == 0)
	//		{
	//			Reporter.Report(i + " zipfiles scanned." + Watch.Elapsed.TotalSeconds.ToString("#.0"));
	//		}
	//	}

	//	//var zipFiles = files.Where(x => x.EndsWith(".zip"));

	//	//ParallelOptions opt = new ParallelOptions();
	//	//opt.MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount / 2, 1);
	//	//Parallel.ForEach(zipFiles, opt, (file) =>
	//	//{
	//	//	if (!romFiles.ContainsKey(file))
	//	//	{
	//	//		RomFile romFile = new RomFile
	//	//		{
	//	//			FilePath = file
	//	//		};

	//	//		romFiles.TryAdd(file, romFile);

	//	//		string name = Path.GetFileNameWithoutExtension(file);
	//	//		if (machines.TryGetValue(name, out Machine machine))
	//	//		{
	//	//			romFile.Machine = machine;
	//	//		}

	//	//		using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(file))
	//	//		{
	//	//			foreach (ZipEntry entry in zipFile)
	//	//			{
	//	//				Rom rom;
	//	//				if (roms.TryGetValue(entry.Crc.ToString("X"), out rom))
	//	//				{
	//	//					romFile.Roms.Add(rom);
	//	//				}

	//	//				else
	//	//				{
	//	//					rom = new Rom
	//	//					{
	//	//						Name = entry.FileName,
	//	//						CRC = entry.Crc.ToString("X"),
	//	//						Size = entry.UncompressedSize,
	//	//						Unknown = true
	//	//					};
	//	//				}
	//	//			}
	//	//		}
	//	//	}
	//	//	if (++i % 1000 == 0)
	//	//	{
	//	//		Reporter.Report(i + " zipfiles scanned." + Watch.Elapsed.TotalSeconds.ToString("#.0"));
	//	//	}
	//	//});

	//	Reporter.Tic("Adding romfiles", out int tic4);
	//	M.Data.RomFiles.AddRange(romFiles.Values);
	//	Reporter.Toc(tic4);

	//	M.Data.Save();

	//	Debug.WriteLine("Finished: " + Watch1.ElapsedMilliseconds);

	//	//}

	//	//catch (Exception ex)
	//	//{
	//	//	int u = 0;
	//	//}
	//}
}


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
using System.Data.Entity;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Robin.Mame
{
	public class MAME
	{
		Platform arcadePlatform;

		List<Region> notNullRegions;

		//public Entities MEntities { get; set; }

		public MAME()
		{
			arcadePlatform = R.Data.Platforms.Local.FirstOrDefault(x => x.ID == CONSTANTS.ARCADE_PLATFORM_ID);
			notNullRegions = R.Data.Regions.Local.Where(x => x.Priority != null).OrderByDescending(x => x.Priority).ToList();
		}


		public void CacheDataBase()
		{
			Stopwatch Watch = Stopwatch.StartNew();
			Stopwatch Watch1 = Stopwatch.StartNew();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Parse;

			int machineCount = 0;
			List<XElement> machineElements = new List<XElement>();

			Reporter.ReportInline(Watch1.Elapsed.ToString(@"m\:ss"));
			Watch1.Restart();
			Reporter.Report("Getting xml file from MAME...");

			// Scan through xml file from MAME and pick out working games

			using (Process process = MAMEexe(@"-lx"))
			using (XmlReader reader = XmlReader.Create(process.StandardOutput, settings))
			{
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
					R.Data.Games.Local.Add(game);
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
						release = new Release();
						release.Rom = rom;
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
						using (Process process1 = MAMEexe(@"-lx " + clonePair[1]))
						using (StreamReader MameOutput = process1.StandardOutput)
						{
							string text = MameOutput.ReadToEnd();
							XElement machineElement = XElement.Parse(text).Element("machine");

							Release parentRelease = new Release();
							Game parentGame = new Game();

							ParseReleaseFromElement(machineElement, parentRelease);

							parentRelease.Game = parentGame;
							orphanRelease.Game = parentGame;
							R.Data.Games.Local.Add(parentGame);
							arcadePlatform.Releases.Add(parentRelease);

							Debug.WriteLine(machineElement.SafeGetA(element1: "driver", attribute: "status"));
						}
					}
				}
			}
			else
			{
				Reporter.Report("No orphans found.");
			}

			Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
		}

		public void CacheDataBase2()
		{
			Stopwatch Watch = Stopwatch.StartNew();

			List<string> goodMachines = new List<string>();
			List<string> goodRoms = new List<string>();
			List<string> goodDisks = new List<string>();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Parse;

			// Wipe tables
			Reporter.Tic("Wiping tables.");
			//string query = @"DELETE FROM Machine_Disk;
			//				DELETE FROM Machine_Rom;
			//				DELETE FROM Machine;
			//				DELETE FROM Disk;
			//				DELETE FROM Rom;
			//				DELETE FROM sqlite_sequence;";

			//M.Data.Database.ExecuteSqlCommand(query);
			M.Data.Machines.Local.Clear();
			M.Data.Roms.Local.Clear();
			M.Data.Disks.Local.Clear();
			M.Data.SaveChanges();
			Reporter.Toc();

			int machineCount = 0;

			Reporter.Tic("Getting xml file from MAME...");

			List<Machine> machines = new List<Machine>();
			//List<Rom> roms = new List<Rom>();
			//List<Disk> disks = new List<Disk>();

			// Scan through xml file from MAME and pick out working games
			using (Process process = MAMEexe(@"-lx"))
			using (XmlReader reader = XmlReader.Create(process.StandardOutput, settings))
			{

				while (reader.Read())
				{
					if (reader.Name == "machine")
					{
						XElement machineElement = XNode.ReadFrom(reader) as XElement;

						// Check if machine exists, if not add it to database
						//string machineName = machineElement.SafeGetA(attribute: "name");
						//goodMachines.Add(machineName);

						//Machine machine = M.Data.Machines.Local.FirstOrDefault(x => x.Name == machineName);

						//if (machine == null)
						//{
						Machine machine = new Machine(machineElement);
						machines.Add(machine);
						//}

						//else
						//{
						//	machine.GetPropsFromXElement(machineElement);
						//}

						// Check if roms exist, if not add them to database
						foreach (XElement romElement in machineElement.Elements("rom"))
						{
							//string romName = romElement.SafeGetA(attribute: "name");
							//goodRoms.Add(romName);

							//Rom rom = M.Data.Roms.Local.FirstOrDefault(x => x.Name == romName);

							//if (rom == null)
							//{
							//Rom rom = new Rom(romElement);
							//roms.Add(rom);
							//}

							//else
							//{
							//	rom.GetPropsFromXElement(romElement);
							//}
							//rom.Machines.Add(machine);
							machine.Roms.Add(new Rom(romElement));

						}

						// Check if disks exist, if not add them to database
						foreach (XElement diskElement in machineElement.Elements("disk"))
						{
							//string diskName = diskElement.SafeGetA(attribute: "name");
							//goodDisks.Add(diskName);

							//Disk disk = M.Data.Disks.Local.FirstOrDefault(x => x.Name == diskName);

							//if (disk == null)
							//{
							//Disk disk = new Disk(diskElement);
							//disks.Add(disk);
							//}

							//else
							//{
							//	disk.GetPropsFromXElement(diskElement);
							//}
							//disk.Machines.Add(machine);
							machine.Disks.Add(new Disk(diskElement));
						}

						if (++machineCount % 100 == 0)
						{
							Reporter.Report(machineCount + " machines");
						}
					}
				}
			}

			Reporter.Toc();


			// Remove items no longer in the latest database
			//Reporter.Tic("Cleaning up database.");
			//M.Data.Machines.Local.RemoveAll(x => !goodMachines.Contains(x.Name));
			//M.Data.Roms.Local.RemoveAll(x => !goodRoms.Contains(x.Name));
			//M.Data.Disks.Local.RemoveAll(x => !goodDisks.Contains(x.Name));
			//Reporter.Toc();

			// Find parent based on cloneof stored as temp value
			Reporter.Tic("Storing clones.");
			foreach (Machine machine in M.Data.Machines.Local.Where(x => x.CloneOf != null))
			{
				machine.Parent = M.Data.Machines.Local.FirstOrDefault(x => x.Name == machine.CloneOf);
			}
			Reporter.Toc();

			// Find sample based on sampleof stored as temp value
			Reporter.Tic("Storing samples.");
			foreach (Machine machine in M.Data.Machines.Local.Where(x => x.CloneOf != null))
			{
				machine.Sample = M.Data.Machines.Local.FirstOrDefault(x => x.Name == machine.SampleOf);
			}
			Reporter.Toc();

			Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
		}

		public void CacheDataBase3()
		{
			Stopwatch Watch = Stopwatch.StartNew();
			Stopwatch Watch1 = Stopwatch.StartNew();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Parse;

			// Wipe tables
			Reporter.Tic("Wiping tables.");
			string query = @"DELETE FROM Machine_Disk;
							DELETE FROM Machine_Rom;
							DELETE FROM Machine;
							DELETE FROM Disk;
							DELETE FROM Rom;
							DELETE FROM sqlite_sequence;";

			M.Data.Database.ExecuteSqlCommand(query);
			Reporter.Toc();

			//Reload M.Data so it knows about the wipe
			M.Refresh(false);
			M.Data.Configuration.ValidateOnSaveEnabled = false;

			int machineCount = 0;
			int romCount = 0;
			int diskCount = 0;

			Dictionary<string, Machine> machines = new Dictionary<string, Machine>();
			Dictionary<string, Rom> roms = new Dictionary<string, Rom>();
			Dictionary<string, Disk> disks = new Dictionary<string, Disk>();

			// Scan through xml file from MAME and store machines
			using (Process process = MAMEexe(@"-lx"))
			using (XmlReader reader = XmlReader.Create(process.StandardOutput, settings))
			{
				Reporter.Tic("Getting xml file from MAME...");
				while (reader.Read())
				{
					if (reader.Name == "machine")
					{
						XElement machineElement = XNode.ReadFrom(reader) as XElement;
						Machine machine = new Machine(machineElement);
						machines.Add(machine.Name, machine);
						machine.ID = machineCount;

						foreach (XElement romElement in machineElement.Elements("rom"))
						{
							string crc = romElement.Attribute("crc")?.Value ?? "NONE" + romCount;
							Rom rom;
							if (!roms.TryGetValue(crc, out rom))
							{
								rom = new Rom(romElement);
								roms.Add(crc, rom);
							}
							machine.Roms.Add(rom);
							rom.ID = ++romCount;
						}

						foreach (XElement diskElement in machineElement.Elements("disk"))
						{
							string sha1 = diskElement.Attribute("sha1")?.Value ?? "NONE" + diskCount;
							Disk disk;

							if (!disks.TryGetValue(sha1, out disk))
							{
								disk = new Disk(diskElement);
								disks.Add(sha1, disk);
							}
							machine.Disks.Add(disk);
							disk.ID = ++diskCount;
						}

						if (++machineCount % 5000 == 0)
						{
							Reporter.Report(machineCount + " machines, " + Watch1.Elapsed.TotalSeconds + " s.");
							Watch1.Restart();
						}
					}
				}
				Reporter.Toc();
			}

			// Add machines
			Reporter.Tic("Adding machines...");
			M.Data.Machines.AddRange(machines.Select(x => x.Value));
			Reporter.Toc();

			Reporter.Tic("Saving changes...");
			M.Data.Save(false);
			Reporter.Toc();

			M.Refresh(true);
			Reporter.Tic("Storing clones...");
			foreach (Machine machine in M.Data.Machines.Local.Where(x => x.CloneOf != null))
			{
				if (machines.TryGetValue(machine.CloneOf, out Machine parent))
				{
					machine.Parent = parent;
				}
			}
			Reporter.Toc();

			// Find sample based on sampleof stored as temp value
			Reporter.Tic("Storing samples...");
			foreach (Machine machine in M.Data.Machines.Local.Where(x => x.SampleOf != null))
			{
				if (machines.TryGetValue(machine.SampleOf, out Machine sampleof))
					machine.Sample = sampleof;
			}
			Reporter.Toc();

			M.Data.Save(true);

			Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
		}

		//public void CacheDataFast()
		//{
		//	Stopwatch Watch = Stopwatch.StartNew();
		//	Stopwatch Watch1 = Stopwatch.StartNew();

		//	XmlReaderSettings settings = new XmlReaderSettings();
		//	settings.DtdProcessing = DtdProcessing.Parse;

		//	// Wipe tables
		//	Reporter.Tic("Wiping tables.");
		//	string query = @"DELETE FROM Machine_Disk;
		//					DELETE FROM Machine_Rom;
		//					DELETE FROM Machine;
		//					DELETE FROM Disk;
		//					DELETE FROM Rom;
		//					DELETE FROM sqlite_sequence;";

		//	M.Data.Database.ExecuteSqlCommand(query);
		//	Reporter.Toc();

		//	//Reload M.Data so it knows about the wipe
		//	M.Refresh(false);
		//	M.Data.Configuration.ValidateOnSaveEnabled = false;

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
		//		Reporter.Tic("Getting xml file from MAME...");
		//		while (reader.Read())
		//		{
		//			if (reader.Name == "machine")
		//			{
		//				XElement machineElement = XNode.ReadFrom(reader) as XElement;
		//				Machine machine = new Machine(machineElement);
		//				machines.Add(machine.Name, machine);
		//				machine.ID = machineCount;

		//				foreach (XElement romElement in machineElement.Elements("rom"))
		//				{
		//					string crc = romElement.Attribute("crc")?.Value ?? "NONE" + romCount;
		//					Rom rom;
		//					if (!roms.TryGetValue(crc, out rom))
		//					{
		//						rom = new Rom(romElement);
		//						roms.Add(crc, rom);
		//					}
		//					machine.Roms.Add(rom);
		//					rom.ID = ++romCount;
		//				}

		//				foreach (XElement diskElement in machineElement.Elements("disk"))
		//				{
		//					string sha1 = diskElement.Attribute("sha1")?.Value ?? "NONE" + diskCount;
		//					Disk disk;

		//					if (!disks.TryGetValue(sha1, out disk))
		//					{
		//						disk = new Disk(diskElement);
		//						disks.Add(sha1, disk);
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
		//		Reporter.Toc();
		//	}

		//	//// Add machines
		//	//Reporter.Tic("Adding machines...");
		//	//M.Data.Machines.AddRange(machines.Select(x => x.Value));
		//	//Reporter.Toc();

		//	Reporter.Tic("Saving changes...");
		//	using (var connection = new SQLiteConnection(M.Data.ConnectionString))
		//	{
		//		connection.Open();
		//		Stopwatch Watch2 = Stopwatch.StartNew();

		//		using (var command = new SQLiteCommand(connection))
		//		{
		//			using (var transaction = connection.BeginTransaction())
		//			{
		//				foreach (Machine machine in machines.Values)
		//				{
		//					command.CommandText = MachineDictionaryToSql(machine);
		//					command.ExecuteNonQuery();
		//				}

		//				foreach (Rom rom in roms.Values)
		//				{
		//					command.CommandText = RomDictionaryToSql(rom);
		//					command.ExecuteNonQuery();
		//				}

		//				transaction.Commit();
		//			}
		//		}
		//		connection.Close();
		//	}
		//	//M.Data.Save(false);
		//	Reporter.Toc();

		//	M.Refresh(true);
		//	Reporter.Tic("Storing clones...");
		//	foreach (Machine machine in M.Data.Machines.Local.Where(x => x.CloneOf != null))
		//	{
		//		if (machines.TryGetValue(machine.CloneOf, out Machine parent))
		//		{
		//			machine.Parent = parent;
		//		}
		//	}
		//	Reporter.Toc();

		//	// Find sample based on sampleof stored as temp value
		//	Reporter.Tic("Storing samples...");
		//	foreach (Machine machine in M.Data.Machines.Local.Where(x => x.SampleOf != null))
		//	{
		//		if (machines.TryGetValue(machine.SampleOf, out Machine sampleof))
		//			machine.Sample = sampleof;
		//	}
		//	Reporter.Toc();

		//	M.Data.Save(true);

		//	Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
		//}

		static ProcessStartInfo MAMEProcess(string arguments = null)
		{
			ProcessStartInfo MAMEexe = new ProcessStartInfo();
			MAMEexe.FileName = FileLocation.MAME;
			MAMEexe.UseShellExecute = false;
			MAMEexe.RedirectStandardOutput = true;
			MAMEexe.WindowStyle = ProcessWindowStyle.Hidden;
			MAMEexe.CreateNoWindow = true;
			MAMEexe.Arguments = arguments;

			return MAMEexe;
		}

		static List<string[]> CloneList()
		{
			ProcessStartInfo MAMEexe = MAMEProcess(@"-lc");

			List<string[]> cloneList = new List<string[]>();
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
			Regex Rparenthesis = new Regex(@"\(.*\)");
			string parenthesisText = Machine.GetParenthesisText(release.Title);

			// Get region from text in parenthesis
			foreach (Region region in R.Data.Regions.Local)
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

		/// <summary>
		/// Audit MAME ROMs currently used by Robin using MAME verifyroms command line switch
		/// </summary>
		public static List<AuditResult> AuditRoms()
		{
			Reporter.Report("Auditing MAME ROMs");
			Emulator mame = R.Data.Emulators.Local.FirstOrDefault(x => x.ID == CONSTANTS.MAME_ID);

			List<string> arguments = GetListOfCurrentRoms();

			List<string> resultStrings = new List<string>();

			int k = 0;
			int N = arguments.Count;
			foreach (string argument in arguments)
			{
				using (Process emulatorProcess = new Process())
				{
					emulatorProcess.StartInfo.CreateNoWindow = false;
					emulatorProcess.StartInfo.UseShellExecute = false;
					emulatorProcess.StartInfo.RedirectStandardOutput = true;
					emulatorProcess.StartInfo.RedirectStandardError = true;
					emulatorProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(mame.FilePath);

					emulatorProcess.StartInfo.FileName = mame.FilePath;
					emulatorProcess.StartInfo.Arguments = argument;

					try
					{
						emulatorProcess.Start();
						Reporter.Tic("Getting batch " + ++k + " / " + N + " from MAME...");
					}
					catch (Exception)
					{
						// TODO: report something usefull here if the process fails to start
					}

					string output = emulatorProcess.StandardOutput.ReadToEnd();
					Reporter.Toc();
					string error = emulatorProcess.StandardError.ReadToEnd();

					Reporter.Tic("Listing results...");
					List<string> lines = output.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("romset ", "").Replace(" is", "").Replace(" available", "")).ToList();
					resultStrings.AddRange(lines);
					Reporter.Toc();
				}
			}

			//List<string> goodResults = resultStrings.Where(x => x.Contains("is good")).ToList();
			//List<string> badResults = resultStrings.Where(x => x.Contains("is bad")).ToList();
			//List<string> bestAvailableResults = resultStrings.Where(x => x.Contains("is best available")).ToList();

			List<AuditResult> auditResults = new List<AuditResult>();
			foreach (string line in resultStrings)
			{
				if (!line.Contains(": "))
				{
					auditResults.Add(new AuditResult(line));
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
			List<string> returner = new List<string>();
			Platform arcadePlatform = R.Data.Platforms.Local.FirstOrDefault(x => x.ID == CONSTANTS.ARCADE_PLATFORM_ID);
			string[] arcadeRoms = arcadePlatform.Releases.Select(x => x.Rom.FileName.Replace(@".zip", "")).OrderBy(x => x).ToArray();

			int i = 0;
			int j = 0;
			while (j < arcadeRoms.Length)
			{
				StringBuilder argument = new StringBuilder("-verifyroms ");

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

		string MachineDictionaryToSql(Machine machine)
		{
			StringBuilder returner = new StringBuilder("INSERT INTO Machine (ID, Name, Description, Year, Manufacturer, Status, Emulation, Players, Display, Control, Parent_ID, Sample_ID, IsMechanical, IsDevice, IsBios, IsRunnable) VALUES (");
			returner.Append(machine.ID).Append(",").
				Append("'").Append(machine.Name).Append("','").
				Append(machine.Description.Replace("'", "''")).Append("','").
				Append(machine.Year ?? "xkcd").Append("','").
				Append((machine.Manufacturer ?? "xkcd").Replace("'", "''")).Append("','").
				Append(machine.Status).Append("','").
				Append(machine.Emulation).Append("','").
				Append(machine.Players ?? "xkcd").Append("','").
				Append(machine.Display ?? "xkcd").Append("','").
				Append(machine.Control ?? "xkcd").Append("',").
				Append(machine.Parent_ID == null ? "null" : machine.Parent_ID.ToString()).Append(",").
				Append(machine.Sample_ID == null ? "null" : machine.Parent_ID.ToString()).Append(",").
				Append(machine.IsMechanical ? "1" : "0").Append(",").
				Append(machine.IsDevice ? "1" : "0").Append(",").
				Append(machine.IsBios ? "1" : "0").Append(",").
				Append(machine.IsRunnable ? "1" : "0").Append(");").Replace("'xkcd'", "null");
			return returner.ToString();
		}

		string RomDictionaryToSql(Rom rom)
		{
			StringBuilder returner = new StringBuilder("INSERT INTO Rom (ID, Name, Region, CRC, Size, Status, Optional) VALUES (");
			returner.Append(rom.ID).Append(",").
				Append("'").Append(rom.Name.Replace("'", "''")).Append("','").
				Append((rom.Region ?? "xkcd").Replace("'", "''")).Append("','").
				Append(rom.CRC ?? "xkcd").Append("',").
				Append(rom.Size == null ? "null" : rom.Size.ToString()).Append(",'").
				Append(rom.Status).Append("',").
				Append(rom.Optional ? "1" : "0").Append(");").Replace("'xkcd'", "null");
			return returner.ToString();
		}

		public class AuditResult
		{
			public Robin.Rom Rom { get; set; }
			public Robin.Rom Parent { get; set; }
			public string Result { get; set; }

			public AuditResult(string line)
			{
				string[] liner = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				if (liner.Length == 2)
				{
					Rom = R.Data.Roms.Local.FirstOrDefault(x => x.FileName == liner[0] + ".zip");
					Result = liner[1];
				}

				if (liner.Length == 3)
				{
					Rom = R.Data.Roms.Local.FirstOrDefault(x => x.FileName == liner[0] + ".zip");
					Parent = R.Data.Roms.Local.FirstOrDefault(x => x.FileName == liner[1].Replace("[", "").Replace("]", "") + ".zip");
					Result = liner[2];
				}
			}
		}

		public void GetFromZipFile(string filename)
		{
			try
			{
				using (ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Read))
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						
					}
				}
			}

			catch (Exception)
			{
			}
		}
	}
}


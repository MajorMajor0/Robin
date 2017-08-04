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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Robin
{
	public class MAME
	{
		Platform arcadePlatform;
		List<Region> regions;
		List<Region> notNullRegions;

		public MAME()
		{

			arcadePlatform = new Platform();
			regions = new List<Region>();
			R.Data = new RobinDataEntities();

			R.Data.Configuration.AutoDetectChangesEnabled = false;
			R.Data.Configuration.LazyLoadingEnabled = false;
			R.Data.Releases.Load();
			R.Data.Releases.Include(x => x.Rom).Load();
			R.Data.Regions.Load();
			R.Data.Games.Load();
			R.Data.Platforms.Load();
			R.Data.Platforms.Include(x => x.Roms).Load();
			notNullRegions = R.Data.Regions.Where(x => x.Priority != null).OrderByDescending(x => x.Priority).ToList();
		}

		public async void GetDataBase()
		{

			Stopwatch Watch = new Stopwatch();
			Stopwatch Watch1 = new Stopwatch();
			Watch.Start();
			Watch1.Start();

			XmlReaderSettings settings = new XmlReaderSettings();

			await Task.Run(() =>
			{
				int machineCount = 0;
				List<XElement> machineElements = new List<XElement>();
				XDocument xdoc = new XDocument();

				settings.DtdProcessing = DtdProcessing.Parse;

				Reporter.Report("Opening databases...");
				Watch1.Restart();

				regions = R.Data.Regions.ToList();
				arcadePlatform = R.Data.Platforms.FirstOrDefault(x => x.Title.Contains("Arcade"));

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
							string driverStatus = machineElement.SafeGetA(element1: "driver", attribute: "status");
							string publisher = machineElement.SafeGetA("manufacturer");

							if ((driverStatus == "good") && !Regex.IsMatch(publisher, @"hack|bootleg"))
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

				int parentCount = parentElements.Count();
				int childCount = childElements.Count();
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
						Rom rom = null;
						string romElementName = romElement.SafeGetA(attribute: "name") + ".zip";
						rom = arcadePlatform.Roms.FirstOrDefault(x => x.FileName == romElementName);

						// Not found, create a new one
						if (rom == null)
						{
							rom = new Rom();
							arcadePlatform.Roms.Add(rom);
						}

						rom.Title = romElement.SafeGetA("description");
						rom.FileName = romElementName;
						rom.Source = "MAME";

						Release release = null;
						release = arcadePlatform.Releases.FirstOrDefault(x => x.Rom.FileName == romElementName);
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
				int orphanCount = orphanReleases.Count();

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

						string[] clonePair = cloneList.FirstOrDefault(x => x[0] + @".zip" == orphanRelease.FileName);

						// First look through arcadePlatform to see if the parent is there
						if (arcadePlatform.Releases.Any(x => x.FileName == clonePair[1] + @".zip"))
						{
							Release parentRelease = arcadePlatform.Releases.FirstOrDefault(x => x.FileName == clonePair[1] + @".zip");
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
								R.Data.Games.Add(parentGame);
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

				R.Data.Save();
                // TODO Report total removed
			});
			Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
		}

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

		static List<string[]> BrotherList()
		{
			ProcessStartInfo MAMEexe = MAMEProcess(@"-lb");

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

		static Process MAMEexe(string arguments)
		{
			ProcessStartInfo mameexe = MAMEProcess(arguments);
			return Process.Start(mameexe);
		}

		void ParseReleaseFromElement(XElement xelement, Release release)
		{
			DateTime datecatcher;
			release.Publisher = xelement.SafeGetA("manufacturer");
			release.Players = xelement.SafeGetA(element1: "input", attribute: "players");
			release.IsGame = true;

			// Get date
			if (DateTime.TryParseExact(xelement.SafeGetA("year"), "yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out datecatcher))
			{
				release.Date = datecatcher;
			}

			// Standardize version/revision text within parenthesis
			#region
			Regex Rparenthesis = new Regex(@"\(.*\)");
			string parenthesisText = Rparenthesis.Match(release.Title).Value;
			string revisionFindText = @"rev[ison\.]*\s?";

			if (parenthesisText != null)
			{
				parenthesisText = parenthesisText.Replace("US", "USA");

				if (Regex.IsMatch(parenthesisText, revisionFindText, RegexOptions.IgnoreCase))
				{
					parenthesisText = Regex.Replace(parenthesisText, revisionFindText, "Rev ", RegexOptions.IgnoreCase);
				}
				else
				{
					parenthesisText = Regex.Replace(parenthesisText, @"v[ersion\.]*\s?", "V ", RegexOptions.IgnoreCase);
				}
			}
			#endregion

			// Get region from text in parenthesis
			#region

			foreach (Region region in regions)
			{
				if (parenthesisText != null && (parenthesisText.Contains(region.Title) || parenthesisText.Contains(region.Datomatic ?? "XXX") || parenthesisText.Contains(region.UNCode ?? "XXX")))
				{
					release.Region_ID = region.ID;
					parenthesisText = Regex.Replace(parenthesisText, region.Title + @"|" + region.Datomatic + @"|" + region.UNCode, string.Empty);
				}
			}

			if (release.Region_ID == null)
			{
				release.Region_ID = CONSTANTS.UNKNOWN_REGION_ID;
			}
			#endregion

			// Get version, remove region and store remaining text as special
			#region
			string versionFind = @"V\s(\d|\w|\.)*";
			string revisionFind = @"Rev\s(\d+|\w|\.)*";

			release.Version = Regex.Match(parenthesisText, revisionFind).Value ?? Regex.Match(parenthesisText, versionFind).Value;

			release.Special = parenthesisText.Replace(@"(", "").Replace(@")", "");
			release.Special = Regex.Replace(release.Special, revisionFind, "");
			release.Special = Regex.Replace(release.Special, versionFind, "");
			release.Special = Regex.Replace(release.Special, @"(^(\s|,)+)", "");
			release.Special = Regex.Replace(release.Special, @"\s+", " ");

			release.Title = Regex.Replace(release.Title, @"\s\(.*\)", "");
			#endregion

		}
	}
}


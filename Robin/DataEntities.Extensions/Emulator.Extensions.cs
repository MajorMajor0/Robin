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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Robin
{
	public partial class Emulator : IDBobject
	{
		public string FilePath
		{
			get { return FileLocation.Emulators + FileName; }
		}

		public bool HasArt
		{
			get { return File.Exists(ImageFile); }
		}

		public bool Included
		{
			get { return File.Exists(FilePath); }
		}

		public bool Preferred
		{
			set { }
			get { return true; }
		}

		public string ImageFile
		{
			get { return FileLocation.Images + Image; }
		}

		public string MainDisplay => ImageFile;

		public string WhyCantIPlay
		{
			get
			{
				if(Included)
				{
					return Title + " is ready to play.";
				}
				return Title + " can't launch because the exe file is missing. Place it in the correct folder or drag it to the emulator icon.";
			}
		}

		public bool Unlicensed => false;

		public void MarkPreferred(Platform platform)
		{
			if (platform != null)
			{
				platform.PreferredEmulator_ID = ID;
			}
		}

		public void Play()
		{
			Play(null);
		}

		public async void Play(Release release = null)
		{
			await Task.Run(() =>
			{
				using (Process emulatorProcess = new Process())
				{
					if (release == null)
					{
						Reporter.Report("Launching " + Title + ".");
					}
					else
					{
						Reporter.Report("Launching " + release.Title + " using " + Title + ".");
					}

					emulatorProcess.StartInfo.CreateNoWindow = false;
					emulatorProcess.StartInfo.UseShellExecute = false;
					emulatorProcess.StartInfo.RedirectStandardOutput = true;
					emulatorProcess.StartInfo.RedirectStandardError = true;

					emulatorProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(FilePath);
					emulatorProcess.StartInfo.FileName = FilePath;

					if (release != null)
					{
						if (ID == CONSTANTS.HIGAN_EMULATOR_ID)
						{
							emulatorProcess.StartInfo.Arguments = @"""" + FileLocation.HiganRoms + release.Platform.HiganRomFolder + @"\" + Path.GetFileNameWithoutExtension(release.Rom.FileName) + release.Platform.HiganExtension + @"""";
						}

						// Strip out .xls if system = MAME
						if (ID == CONSTANTS.MAME_ID)
						{
							if (release.Platform.ID == CONSTANTS.CHANNELF_PLATFORM_ID)
							{
								emulatorProcess.StartInfo.Arguments = "channelf -cart " + @"""" + release.FilePath + @"""";// + " -skip_gameinfo -nowindow";
							}
							else
							{
								emulatorProcess.StartInfo.Arguments = Path.GetFileNameWithoutExtension(release.Rom.FileName);
							}
						}

						else
						{
							emulatorProcess.StartInfo.Arguments = release.FilePath;
						}
					}

					try
					{
						emulatorProcess.Start();

						if (release != null)
						{
							release.PlayCount++;
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
				}
			});
		}

		public int ScrapeArt(ArtType artType, LocalDB localDB)
		{
			Debug.Assert(false, "Trying to scrape art for an emulator. Don't do that.");
			return 0;
		}

		public void Add(string sourceFilePath)
		{
			if (Path.GetExtension(sourceFilePath) == ".exe")
			{
				try
				{
					Reporter.Report("Adding " + Title);

					string destinationDirectory = Path.GetDirectoryName(FilePath);
					string sourceDirectory = Path.GetDirectoryName(sourceFilePath);
					string sourceFile = Path.GetFileName(sourceFilePath);
					string sourceFileAfterMove = destinationDirectory + @"\" + sourceFile;

					Directory.CreateDirectory(destinationDirectory);
					Filex.CopyDirectory(sourceDirectory, destinationDirectory);
					File.Move(sourceFileAfterMove, FilePath);
					OnPropertyChanged("Included");
				}
				catch
				{
					Reporter.Warn("Problem creating directory of moving file.");
				}
			}
			else
			{
				Reporter.Warn("The dropped file doesn't look like an executable.");
			}
		}
	}
}

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
using System.IO;

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
            throw new NotImplementedException();
        }

        public int ScrapeArt(LocalDB localDB)
        {
            Reporter.Report("Scraping art not available for emulators");
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

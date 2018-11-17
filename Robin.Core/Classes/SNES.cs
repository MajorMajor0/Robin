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


//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Diagnostics;
//using System.Text;
//using System.Threading.Tasks;

//namespace Robin
//{
//    class SNES
//    {

//        public static void GetROMS(Platform platform, string folder)
//        {
//            List<string> filelist = Directory.GetFiles(folder).ToList<string>();
//            int HeaderLength = 512;
//            int total = filelist.Count();

//            string CurrentDir = FileLocation.Roms + platform.FileName + @"\" ;
//            Directory.CreateDirectory(CurrentDir);

//            foreach (string file in filelist)
//            {
//                // Get SHA1
//                FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);

//                if (stream.Length < HeaderLength)
//                {
//                    stream.Close();
//                    continue;
//                }
//                // Read header
//                byte[] header = new byte[HeaderLength];
//                stream.Read(header, 0, HeaderLength);

//                // Read SHA1 buffer
//                byte[] buffer = new byte[stream.Length - HeaderLength];
//                stream.Read(buffer, 0, (int)(stream.Length - HeaderLength));

//                // SHA1
//                string SHA1 = "";
//                SHA1Managed managedSHA1 = new SHA1Managed();
//                byte[] shaBuffer = managedSHA1.ComputeHash(buffer);
//                foreach (byte b in shaBuffer)
//                {
//                    SHA1 += b.ToString("x2").ToUpper();
//                }
//                var LinqResult = platform.Releases.Where(x => Equals(x.SHA1, SHA1));

//                if (LinqResult.Any())
//                {
//                    Release release = LinqResult.FirstOrDefault();
//                    if (release.FileName==null || release.FileName=="")
//                    {
//                        string DestinationFilename = Path.GetFileNameWithoutExtension(file).Wash();
//                        string DestinationFilenameN;
//                        string Ext = Path.GetExtension(file);


//                        release.FileName = FileLocation.Roms + platform.FileName + @"\" + DestinationFilename;

//                        DestinationFilename = CurrentDir + DestinationFilename + Ext;
//                        DestinationFilenameN = DestinationFilename;
//                        int i = 2;
//                        while (File.Exists(DestinationFilenameN))
//                        {
//                            DestinationFilenameN = DestinationFilename + i.ToString();
//                        }
//                        File.Copy(file, DestinationFilename);

//                    }

//                }
//                stream.Close();
//            }
//        }
//    }
//}

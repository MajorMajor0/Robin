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

namespace Robin.Core
{
	class Handy
	{
		public static bool ConvertLynx(string fileName)
		{
			try
			{
				Process handy = new Process();

				handy.StartInfo.CreateNoWindow = true;
				handy.StartInfo.UseShellExecute = false;
				handy.StartInfo.FileName = FileLocation.HandyConverter;
				handy.StartInfo.Arguments = fileName;
				handy.StartInfo.RedirectStandardOutput = true;
				handy.StartInfo.RedirectStandardError = true;
				handy.StartInfo.WorkingDirectory = Path.GetDirectoryName(FileLocation.HandyConverter);

				handy.Start();
				string output = handy.StandardOutput.ReadToEnd();
				//string error = handy.StandardError.ReadToEnd();
				handy.WaitForExit();

				if(output.StartsWith("DONE"))
				{
					return true;
				}
				else
				{
					return false;
				}
				
			}

			catch (Exception)
			{
				return false;
			}
		}
	}
}

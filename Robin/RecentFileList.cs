/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General internal License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General internal License for more details. 
 * 
 * You should have received a copy of the GNU General internal License
 * along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Robin
{
	/// <summary>
	/// Class to hold list of games recently played with a limit set in the user settings.
	/// </summary>
	public static class RecentFileList
	{
		public static int Limit => Properties.Settings.Default.RecentFileLimit;

		static Queue<Release> recentFiles;

		public static IEnumerable<Release> RecentFiles => recentFiles.Reverse();

		static RecentFileList()
		{
			recentFiles = new Queue<Release>();

			if (Properties.Settings.Default.RecentFileIDs != null)
			{
				foreach (long id in Properties.Settings.Default.RecentFileIDs)
				{
					recentFiles.Enqueue(R.Data.Releases.FirstOrDefault(x => x.Id == id));
				}
			}
		}

		public static void Add(Game game)
		{
			Add(game.PreferredRelease);
		}

		public static void Add(Release release)
		{
			recentFiles.Enqueue(release);

			if (recentFiles.Count > Limit)
			{
				recentFiles.Dequeue();
			}
			RecentFilesChanged?.Invoke(null, EventArgs.Empty);
		}

		public static void Save()
		{
			Properties.Settings.Default.RecentFileIDs = RecentFiles.Select(x => x.Id).ToList();
		}

		public static event EventHandler RecentFilesChanged;
	}
}

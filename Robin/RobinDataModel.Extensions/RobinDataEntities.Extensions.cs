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
using System.Linq;
using System.Windows;

using Microsoft.EntityFrameworkCore;

namespace Robin
{
	public static class R
	{
		/// <summary>
		/// This is where the entire application gets its info
		/// </summary>
		public static RobinDataEntities Data;

		static R()
		{
			Data = new RobinDataEntities(true);
		}
	}

	public partial class RobinDataEntities : Entity
	{
		string dataSource = Robin.FileLocation.RobinData;

		public override string FileLocation => Robin.FileLocation.RobinData;

		public RobinDataEntities(bool chooser)
		{
			Stopwatch Watch = new();
			try
			{
				ChangeTracker.LazyLoadingEnabled = false;
				ChangeTracker.AutoDetectChangesEnabled = false;

				string cs = Database.GetConnectionString();

				Watch = Stopwatch.StartNew();


				Platforms.Include(x => x.Emulators).Load();
				Reporter.Report("Platforms loaded " + Watch.Elapsed.TotalSeconds.ToString("F1") + " s."); Watch.Restart();
				Roms.Load();
				Reporter.Report("Roms loaded " + Watch.Elapsed.TotalSeconds.ToString("F1") + " s."); Watch.Restart();
				Games.Include(x => x.Releases).Load();
				Reporter.Report("Games loaded " + Watch.Elapsed.TotalSeconds.ToString("F1") + " s."); Watch.Restart();
				Regions.Load();
				Reporter.Report("Regions loaded " + Watch.Elapsed.TotalSeconds.ToString("F1") + " s."); Watch.Restart();
				Collections.Include(x => x.Games).Include(x => x.Releases).Load();
				Reporter.Report("Collections loaded " + Watch.Elapsed.TotalSeconds.ToString("F1") + " s."); Watch.Restart();

			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Problem Creating Model", MessageBoxButton.OK);
			}
			catch(Microsoft.Data.Sqlite.SqliteException ex)
			{
				MessageBox.Show(ex.Message, "Sqlite Exception", MessageBoxButton.OK);
			}

			foreach (Game game in Games)
			{
				game.Releases = game.Releases.OrderBy(x => x.Region.Priority).ThenByDescending(x => x.Version).ToList();
			}
			Reporter.Report("Games ordered " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
		}

	}
}

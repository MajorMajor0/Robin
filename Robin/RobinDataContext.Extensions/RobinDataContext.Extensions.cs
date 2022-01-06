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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using Microsoft.EntityFrameworkCore;

namespace Robin
{
	public static class R
	{
		public static string FileLocation => Robin.FileLocation.RobinData;
		/// <summary>
		/// This is where the entire application gets its info
		/// </summary>
		public static RobinDataContext Data { get; }

		static R()
		{
			Stopwatch watch = Stopwatch.StartNew();
			Data = new RobinDataContext();

			Stopwatch watch2 = Stopwatch.StartNew();
			try
			{
				Data.ChangeTracker.LazyLoadingEnabled = false;
				Data.ChangeTracker.AutoDetectChangesEnabled = false;

				watch2 = Stopwatch.StartNew();

				Data.Platforms.Include(x => x.Emulators).Include(y => y.Cores).Load();
				Reporter.Report($"Platforms loaded {watch2.Elapsed.TotalSeconds:F1} s."); watch2.Restart();
				Data.Roms.Load();
				Reporter.Report($"Roms loaded {watch2.Elapsed.TotalSeconds:F1} s."); watch2.Restart();
				Data.Games.Include(x => x.Releases).Load();
				Reporter.Report($"Games loaded {watch2.Elapsed.TotalSeconds:F1} s."); watch2.Restart();
				Data.Regions.Load();
				Reporter.Report($"Regions loaded {watch2.Elapsed.TotalSeconds:F1} s."); watch2.Restart();
				Data.Collections.Include(x => x.Games).Include(x => x.Releases).Load();
				Reporter.Report($"Collections loaded {watch2.Elapsed.TotalSeconds:F1} s."); watch2.Restart();
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Problem Creating Model", MessageBoxButton.OK);
			}
			catch (Microsoft.Data.Sqlite.SqliteException ex)
			{
				MessageBox.Show(ex.Message, "Sqlite Exception", MessageBoxButton.OK);
			}

			foreach (Game game in Data.Games)
			{
				game.Releases = game.Releases.OrderBy(x => x.Region.Priority).ThenByDescending(x => x.Version).ToList();
			}
			Reporter.Report($"Games ordered {watch2.Elapsed.Seconds} s."); watch2.Restart();

			Reporter.Report($"Database loaded {watch.Elapsed.TotalSeconds:F1} s.");
		}



		public static bool Blocked { get; set; }

		//public Entity() : base()
		//{

		//}

		//public Entity(string namer) : base(namer)
		//{
		//}

		public static void Save(bool detectChanges = false)
		{
			string backupFile;

			if (detectChanges)
			{
				Data.ChangeTracker.DetectChanges();
			}

			if (Data.ChangeTracker.Entries().Any())
			{
				backupFile = Backup();
			}

			try
			{
				int nChanges = Data.SaveChanges();
				Reporter.Report($"{nChanges} Database changes saved successfully");
			}

			catch (DbUpdateConcurrencyException ex)
			{
				HandleUpdateException(ex);
			}
			catch (DbUpdateException ex)
			{
				HandleUpdateException(ex);
			}
		}

		private static void HandleUpdateException(DbUpdateException ex)
		{
			string entries = string.Empty;
			int i = 0;

			foreach (var entry in ex.Entries)
			{
				Type type = entry.GetType();
				var props = new List<PropertyInfo>(type.GetProperties());

				entries += $"Entry {i++}:\n";
				foreach (PropertyInfo prop in props)
				{
					entries += $"{prop.Name}:\t{prop.GetValue(entry, null)}\n";
				}
				entries += "\n";
			}

			string message = $"ex.Message\n\nFailed Entries:\n{entries}";
			Reporter.Report(message);

			MessageBox.Show(message, "Database Update Error", MessageBoxButton.OK);
		}
		/// <summary>
		/// Backup existing database.
		/// </summary>
		/// <returns></returns>
		public static string Backup()
		{
			Directory.CreateDirectory(Robin.FileLocation.Backup);
			string Date = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

			string CurrentFile = FileLocation;

			string backupFile = Robin.FileLocation.Backup + Path.GetFileNameWithoutExtension(CurrentFile) + Date + Path.GetExtension(CurrentFile);
			try
			{
				File.Copy(CurrentFile, backupFile);
				Reporter.Report("DB file backed up to " + backupFile);
				return backupFile;
			}
			catch (Exception ex)
			{
				Reporter.Report("Failed to backup file.");
				Reporter.Report(ex.Message);
				return null;
			}
		}

	}
}

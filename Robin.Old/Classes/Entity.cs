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
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Robin.Core
{
	public class Entity : DbContext
	{
		public virtual string FileLocation { get; set; }

		public static bool Blocked { get; set; }

		public Entity() : base()
		{

		}

		public Entity(string namer) : base(namer)
		{
		}

		public void Save(bool detectChanges = false)
		{
			string backupFile;

			if (detectChanges)
			{
				ChangeTracker.DetectChanges();
			}

			if(ChangeTracker.Entries().Any())
			{
				backupFile = Backup();
			}

			try
			{
				int nChanges = SaveChanges();
				Reporter.Report($"{nChanges} Database changes saved successfully");
			}
			catch (DbEntityValidationException dbEx)
			{
				foreach (var validationErrors in dbEx.EntityValidationErrors)
				{
					foreach (var validationError in validationErrors.ValidationErrors)
					{
						Trace.TraceInformation("Property: {0} Error: {1}",
												validationError.PropertyName,
												validationError.ErrorMessage);

						Reporter.Report($"Data validation error...\nProperty: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
					}
				}
			}
		}

		/// <summary>
		/// Backup uexisting database.
		/// </summary>
		/// <returns></returns>
		public string Backup()
		{
			Directory.CreateDirectory(Robin.Core.FileLocation.Backup);
			string Date = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

			string CurrentFile = FileLocation;

			string backupFile = Robin.Core.FileLocation.Backup + Path.GetFileNameWithoutExtension(CurrentFile) + Date + Path.GetExtension(CurrentFile);
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

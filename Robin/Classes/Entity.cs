using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
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
			int i;

			string backupFile = Backup();

			if (detectChanges)
			{
				ChangeTracker.DetectChanges();
			}

			try
			{
				i = SaveChanges();
				Reporter.Report(i + " Database changes saved successfully");
				if (i == 0)
				{
					File.Delete(backupFile);
				}
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

						Reporter.Report("Data validation error...\nProperty: " + validationError.PropertyName + "Error: " + validationError.ErrorMessage);
					}
				}
			}
		}

		public string Backup()
		{
			// Backup existing database
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

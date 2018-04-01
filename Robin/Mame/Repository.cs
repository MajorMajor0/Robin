using EntityWorker.Core.Helper;
using EntityWorker.Core.Interface;
using EntityWorker.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin.Mame
{
	public class Repository : Transaction
	{
		// there are three databases types mssql, Sqlite and PostgreSql
		public Repository(DataBaseTypes dbType = DataBaseTypes.Sqllight) : base(GetConnectionString(dbType), dbType)
		{
		}

		protected override void OnModuleStart()
		{
			if (!base.DataBaseExist())
			{
				base.CreateDataBase();
			}

			// You could choose to use this to apply you changes to the database or create your own migration
			// that will update the database, like alter drop or create.
			// Limited support for sqlite
			// Get the latest change between the code and the database. 
			// Property Rename is not supported. renaming property x will end up removing the x and adding y so there will be dataloss
			// Adding a primary key is not supported either
			//var latestChanges = GetCodeLatestChanges();
			//if (latestChanges.Any())
			//{
			//	latestChanges.Execute(true);
			//}


			// Start the migration
			//InitializeMigration();
		}

		// We could configrate our modules here instead of adding attributes in the class,
		// off course its up to you too choose.
		protected override void OnModuleConfiguration(IModuleBuilder moduleBuilder)
		{
		}


		// get the full connection string
		// for postgresql make sure to have the database name lower case
		public static string GetConnectionString(DataBaseTypes dbType)
		{
			if (dbType == DataBaseTypes.Mssql)
			{
				return @"Server=.\SQLEXPRESS; Database=CMS; User Id=root; Password=root;";
			}

			else if (dbType == DataBaseTypes.Sqllight)
			{
				return @"data source = " + Robin.FileLocation.MameData;
			}

			else
			{
				return "Host=localhost;Username=postgres;Password=root;Database=cms";
			}
		}
	}
}

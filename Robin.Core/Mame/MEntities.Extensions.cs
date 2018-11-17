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

using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace Robin.Core.Mame
{
	public static class M
	{
		public static MEntities Data;

		static M()
		{
			Data = new MEntities(false);
		}

		public static void Refresh(bool load)
		{
			Data = new MEntities(load);
		}
	}


	public partial class MEntities : Entity
	{
		string dbName = "MameDataModel";
		string dataSource = Core.FileLocation.MameData;

		public override string FileLocation => Core.FileLocation.MameData;

		public MEntities(bool load)
		{
			string connectionString = $"metadata=res://*/{dbName}.csdl|res://*/{dbName}.ssdl|res://*/{dbName}.msl;provider=System.Data.SQLite.EF6;data source = {dataSource}";
			Database.Connection.ConnectionString = connectionString;

			Configuration.LazyLoadingEnabled = false;
			Configuration.AutoDetectChangesEnabled = false;

			if (load)
			{
				Reporter.Tic("Loading MAME...", out int tic1);
				Machines.Include(x => x.Disks).Include(x => x.Roms).Load();
				Reporter.Toc(tic1);
			}

		}

	}
}

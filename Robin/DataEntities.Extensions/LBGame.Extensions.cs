using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public partial class LBGame : IComparableDB, IDBRelease
	{
		public static List<LBGame> GetGames(Platform platform)
		{
			using (RobinDataEntities Rdata = new RobinDataEntities())
			{
				Rdata.LBGames.Load();
				Rdata.LBGames.Include(x => x.LBImages).Load();
				Rdata.Regions.Load();
				return Rdata.LBGames.Where(x => x.LBPlatform_ID == platform.ID_LB).ToList();
			}
		}

		public string RegionTitle
		{
			get; set;
		}

		public Region Region { get { return null; } }


	}
}

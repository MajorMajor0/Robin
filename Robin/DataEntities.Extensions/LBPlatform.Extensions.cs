using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public partial class LBPlatform: IDBPlatform
	{
		public IList Releases => LBGames;
	}
}

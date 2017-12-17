using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public partial class LBImage
	{
		public Region Region => R.Data.Regions.FirstOrDefault(x => x.ID == Region_ID);
	}
}

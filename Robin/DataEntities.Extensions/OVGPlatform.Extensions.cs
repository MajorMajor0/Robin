using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public partial class OVGPlatform : IDBPlatform
	{
		public IList Releases => OVGReleases;

		public string Manufacturer => null;

		public DateTime? Date => null;
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Robin
{
	public partial class GBPlatform
	{
		public GBPlatform()
		{
			GBGames = new HashSet<GBGame>();
			GBReleases = new HashSet<GBRelease>();
		}

		public long ID { get; set; }
		public string Title { get; set; }
		public string Abbreviation { get; set; }
		public long? Company { get; set; }
		public string Deck { get; set; }
		public string Price { get; set; }

		public DateTime? Date { get; set; }
		public DateTime CacheDate { get; set; }

		public virtual Platform Platform { get; set; }
		public virtual HashSet<GBGame> GBGames { get; set; }
		public virtual HashSet<GBRelease> GBReleases { get; set; }
	}
}

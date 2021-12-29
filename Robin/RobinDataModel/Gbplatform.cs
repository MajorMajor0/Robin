using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Robin
{
	public partial class Gbplatform
	{
		public Gbplatform()
		{
			Gbgames = new HashSet<Gbgame>();
			Gbreleases = new HashSet<Gbrelease>();
		}

		public long Id { get; set; }
		public string Title { get; set; }
		public string Abbreviation { get; set; }
		public long? Company { get; set; }
		public string Deck { get; set; }
		public string Price { get; set; }

		public DateTime? Date { get; set; }
		public DateTime CacheDate { get; set; }

		public virtual Platform Platform { get; set; }
		public virtual HashSet<Gbgame> Gbgames { get; set; }
		public virtual HashSet<Gbrelease> Gbreleases { get; set; }
	}
}

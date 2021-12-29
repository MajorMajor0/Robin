using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Robin
{
	public partial class Gbgame
	{
		public Gbgame()
		{
			Gbreleases = new HashSet<Gbrelease>();
		}

		public long Id { get; set; }

		public long? GbplatformId { get; set; }

		public string Title { get; set; }

		public string Overview { get; set; }

		public string Developer { get; set; }

		public string Publisher { get; set; }

		public string Genre { get; set; }

		public DateTime? Date { get; set; }

		public string BoxFrontUrl { get; set; }

		public string ScreenUrl { get; set; }

		public virtual Gbplatform Gbplatform { get; set; }
		public virtual ICollection<Gbrelease> Gbreleases { get; set; }
	}
}

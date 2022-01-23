using System;
using System.Collections.Generic;

namespace Robin;

public partial class MBPlatform
{
	public MBPlatform()
	{
		MBGames = new HashSet<MBGame>();
		MBReleases = new HashSet<MBRelease>();
	}

	public long ID { get; set; }
	public string Title { get; set; }
	//public string Abbreviation { get; set; }
	//public long? Company { get; set; }
	//public string Deck { get; set; }
	//public string Price { get; set; }
	// public DateTime Date { get; set; }
	public DateTime CacheDate { get; set; }

	public virtual Platform Platform { get; set; }


	public virtual ICollection<MBGame> MBGames { get; set; }
	public virtual ICollection<MBRelease> MBReleases { get; set; }
}

using System;
using System.Collections.Generic;

namespace Robin;

public partial class MBGame
{
	public MBGame()
	{
		MBReleases = new HashSet<MBRelease>();
		MBAttributes = new HashSet<MBAttribute>();
		MBGenres = new HashSet<MBGenre>();
	}

	public long ID { get; set; }
	public string Title { get; set; }
	public string Overview { get; set; }
	public long MBPlatform_ID { get; set; }
	public DateTime? Date { get; set; }

	public virtual MBPlatform MBPlatform { get; set; }
	public virtual ICollection<MBRelease> MBReleases { get; set; }
	public virtual ICollection<MBAttribute> MBAttributes { get; set; }
	public virtual ICollection<MBGenre> MBGenres { get; set; }
}

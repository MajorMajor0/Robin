using System;
using System.Collections.Generic;

namespace Robin;

public partial class GBGame
{
	public GBGame()
	{
		GBReleases = new HashSet<GBRelease>();
	}

	public long ID { get; set; }

	public long? GBPlatform_ID { get; set; }

	public string Title { get; set; }

	public string Overview { get; set; }

	public string Developer { get; set; }

	public string Publisher { get; set; }

	public string Genre { get; set; }

	public DateTime? Date { get; set; }

	public string BoxFrontUrl { get; set; }

	public string ScreenUrl { get; set; }

	public virtual GBPlatform GBPlatform { get; set; }
	public virtual ICollection<GBRelease> GBReleases { get; set; }
}

using System.Collections.Generic;

namespace Robin;

public partial class Region
{
	public Region()
	{
		GBReleases = new HashSet<GBRelease>();
		MBReleases = new HashSet<MBRelease>();
		LBReleases = new HashSet<LBRelease>();
		OVGReleases = new HashSet<OVGRelease>();
		Releases = new HashSet<Release>();
	}

	public long ID { get; set; }
	public string Title { get; set; }
	public string Datomatic { get; set; }
	public long? ID_GB { get; set; }
	public string TitleGb { get; set; }
	public string UNCode { get; set; }
	public long? Priority { get; set; }
	public string Mame { get; set; }
	public string Launchbox { get; set; }

	public virtual ICollection<GBRelease> GBReleases { get; set; }
	public virtual ICollection<LBRelease> LBReleases { get; set; }
	public virtual ICollection<MBRelease> MBReleases { get; set; }
	public virtual ICollection<OVGRelease> OVGReleases { get; set; }
	public virtual ICollection<Release> Releases { get; set; }
}

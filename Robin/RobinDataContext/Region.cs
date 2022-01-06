using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Region
    {
        public Region()
        {
            Gbreleases = new HashSet<Gbrelease>();
            Lbreleases = new HashSet<Lbrelease>();
            Ovgreleases = new HashSet<Ovgrelease>();
            Releases = new HashSet<Release>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Datomatic { get; set; }
        public long? IdGb { get; set; }
        public string TitleGb { get; set; }
        public string Uncode { get; set; }
        public long? Priority { get; set; }
        public string Mame { get; set; }
        public string Launchbox { get; set; }

        public virtual ICollection<Gbrelease> Gbreleases { get; set; }
        public virtual ICollection<Lbrelease> Lbreleases { get; set; }
        public virtual ICollection<Ovgrelease> Ovgreleases { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

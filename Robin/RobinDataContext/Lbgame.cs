using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Lbgame
    {
        public Lbgame()
        {
            Lbreleases = new HashSet<Lbrelease>();
            Releases = new HashSet<Release>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Overview { get; set; }
        public long LbplatformId { get; set; }
        public string Genres { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string VideoUrl { get; set; }
        public string WikiUrl { get; set; }
        public string Players { get; set; }

        public virtual Lbplatform Lbplatform { get; set; }
        public virtual ICollection<Lbrelease> Lbreleases { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Lbrelease
    {
        public Lbrelease()
        {
            Lbimages = new HashSet<Lbimage>();
            Releases = new HashSet<Release>();
        }

        public long Id { get; set; }
        public long LbgameId { get; set; }
        public long RegionId { get; set; }
        public string Title { get; set; }

        public virtual Lbgame Lbgame { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Lbimage> Lbimages { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

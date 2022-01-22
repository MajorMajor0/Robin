using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class LBRelease
    {
        public LBRelease()
        {
            LBImages = new HashSet<LBImage>();
            Releases = new HashSet<Release>();
        }

        public long ID { get; set; }
        public long LBGameId { get; set; }
        public long RegionId { get; set; }
        public string Title { get; set; }

        public virtual LBGame LBGame { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<LBImage> LBImages { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

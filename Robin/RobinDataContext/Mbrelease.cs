using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Mbrelease
    {
        public Mbrelease()
        {
            Releases = new HashSet<Release>();
        }

        public long ID { get; set; }
        public string Title { get; set; }
        public long? RegionId { get; set; }
        public string Overview { get; set; }
        public long? MBGameId { get; set; }
        public string Players { get; set; }
        public long MBPlatformId { get; set; }
        public string BoxUrl { get; set; }
        public string ScreenUrl { get; set; }
        public DateTime? Date { get; set; }

        public virtual MBGame MBGame { get; set; }
        public virtual MBPlatform MBPlatform { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

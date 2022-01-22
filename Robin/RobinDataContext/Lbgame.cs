using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class LBGame
    {
        public LBGame()
        {
            LBReleases = new HashSet<LBRelease>();
            Releases = new HashSet<Release>();
        }

        public long ID { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Overview { get; set; }
        public long LBPlatformId { get; set; }
        public string Genres { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string VideoUrl { get; set; }
        public string WikiUrl { get; set; }
        public string Players { get; set; }

        public virtual LBPlatform LBPlatform { get; set; }
        public virtual ICollection<LBRelease> LBReleases { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

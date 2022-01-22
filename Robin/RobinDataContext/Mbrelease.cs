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

        public long Id { get; set; }
        public string Title { get; set; }
        public long? RegionId { get; set; }
        public string Overview { get; set; }
        public long? MbgameId { get; set; }
        public string Players { get; set; }
        public long MbplatformId { get; set; }
        public string BoxUrl { get; set; }
        public string ScreenUrl { get; set; }
        public DateTime? Date { get; set; }

        public virtual Mbgame Mbgame { get; set; }
        public virtual Mbplatform Mbplatform { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

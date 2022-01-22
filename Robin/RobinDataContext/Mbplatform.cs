using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Mbplatform
    {
        public Mbplatform()
        {
            Mbgames = new HashSet<Mbgame>();
            Mbreleases = new HashSet<Mbrelease>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        //public string Abbreviation { get; set; }
        //public long? Company { get; set; }
        //public string Deck { get; set; }
        //public string Price { get; set; }
        // public DateTime Date { get; set; }
        public DateTime CacheDate { get; set; }

        public virtual Platform Platform { get; set; }


        public virtual ICollection<Mbgame> Mbgames { get; set; }
        public virtual ICollection<Mbrelease> Mbreleases { get; set; }
    }
}

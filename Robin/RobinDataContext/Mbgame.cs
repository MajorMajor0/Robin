using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Mbgame
    {
        public Mbgame()
        {
            Mbreleases = new HashSet<Mbrelease>();
            Mbgenres = new HashSet<Mbgenre>();

        }

        public long Id { get; set; }
        public long MbplatformId { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public DateTime? Date { get; set; }

        public virtual Mbplatform Mbplatform { get; set; }
        public virtual ICollection<Mbrelease> Mbreleases { get; set; }

        public virtual ICollection<Mbgenre> Mbgenres { get; set; }
    }
}

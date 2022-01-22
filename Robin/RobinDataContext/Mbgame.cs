using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class MBGame
    {
        public MBGame()
        {
            Mbreleases = new HashSet<Mbrelease>();
            MBGenres = new HashSet<MBGenre>();

        }

        public long ID { get; set; }
        public long MBPlatformId { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public DateTime? Date { get; set; }

        public virtual MBPlatform MBPlatform { get; set; }
        public virtual ICollection<Mbrelease> Mbreleases { get; set; }

        public virtual ICollection<MBGenre> MBGenres { get; set; }
    }
}

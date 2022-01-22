using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class MBGenre
    {
        public MBGenre()
        {
            MBGames = new HashSet<MBGame>();
        }

        public long ID { get; set; }
        public string Category { get; set; }
        public long CategoryId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MBGame> MBGames { get; set; }
    }
}

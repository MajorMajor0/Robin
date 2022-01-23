using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class MBGenreCategory
    {
        public MBGenreCategory()
        {
            MBGenres = new HashSet<MBGenre>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MBGenre> MBGenres { get; set; }
    }
}

﻿using System.Collections.Generic;

namespace Robin;

public partial class MBGenre
{
    public MBGenre()
    {
        MBGames = new HashSet<MBGame>();
    }

    public long ID { get; set; }
    public long Category_ID { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }

    public virtual MBGenreCategory Category { get; set; }

    public virtual ICollection<MBGame> MBGames { get; set; }
}

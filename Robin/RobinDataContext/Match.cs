﻿using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Match
    {
        public long Id { get; set; }
        public long? IdGb { get; set; }
        public long? IdGdb { get; set; }
        public long? IdOvg { get; set; }
        public string Sha1 { get; set; }
        public long? RegionId { get; set; }
    }
}
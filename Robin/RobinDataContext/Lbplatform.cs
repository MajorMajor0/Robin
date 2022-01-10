﻿using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Lbplatform
    {
        public Lbplatform()
        {
            Lbgames = new HashSet<Lbgame>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Developer { get; set; }
        public string Manufacturer { get; set; }
        public string Cpu { get; set; }
        public string Memory { get; set; }
        public string Graphics { get; set; }
        public string Sound { get; set; }
        public string Display { get; set; }
        public string Media { get; set; }
        public string Controllers { get; set; }
        public DateTime? Date { get; set; }
        public DateTime CacheDate { get; set; }

        public virtual Platform Platform { get; set; }
        public virtual ICollection<Lbgame> Lbgames { get; set; }
    }
}
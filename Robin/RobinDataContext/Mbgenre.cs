using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Mbgenre
    {
        public Mbgenre()
        {
            Mbgames = new HashSet<Mbgame>();
        }

        public long Id { get; set; }
        public string Category { get; set; }
        public long CategoryId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Mbgame> Mbgames { get; set; }
    }
}

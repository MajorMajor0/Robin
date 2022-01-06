using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Ovgplatform
    {
        public Ovgplatform()
        {
            Ovgreleases = new HashSet<Ovgrelease>();
        }

        public long Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Ovgrelease> Ovgreleases { get; set; }
    }
}

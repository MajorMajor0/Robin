using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Lbimage
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public string Lbregion { get; set; }
        public long RegionId { get; set; }
        public long? LbreleaseId { get; set; }

        public virtual Lbrelease Lbrelease { get; set; }
    }
}

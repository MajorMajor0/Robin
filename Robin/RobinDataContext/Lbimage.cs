using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class LBImage
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public string Lbregion { get; set; }
        public long RegionId { get; set; }
        public long? LBReleaseId { get; set; }

        public virtual LBRelease LBRelease { get; set; }
    }
}

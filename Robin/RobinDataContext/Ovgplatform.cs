using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class OVGPlatform
    {
        public OVGPlatform()
        {
            OVGReleases = new HashSet<OVGRelease>();
        }

        public long ID { get; set; }
        public string Title { get; set; }

        public virtual ICollection<OVGRelease> OVGReleases { get; set; }
    }
}

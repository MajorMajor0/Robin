using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Rom
    {
        public Rom()
        {
            Releases = new List<Release>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Crc32 { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public string Size { get; set; }
        public string Source { get; set; }
        public string FileName { get; set; }

        public virtual IList<Release> Releases { get; set; }
    }
}

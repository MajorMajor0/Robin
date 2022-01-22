using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robin
{
    public partial class OVGRelease
    {
        public OVGRelease()
        {
            Releases = new HashSet<Release>();
        }

        public long ID { get; set; }
        public long? RegionId { get; set; }

        [Column("OVGPlatform_ID")]
        public long OVGPlatformId { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public string DateText { get; set; }
        public string Crc { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public string Size { get; set; }
        public string Header { get; set; }
        public string Language { get; set; }
        public string Serial { get; set; }
        public string BoxFrontUrl { get; set; }
        public string BoxBackUrl { get; set; }
        public string ReferenceUrl { get; set; }
        public string ReferenceImageUrl { get; set; }
        public DateTime? Date { get; set; }

        public virtual OVGPlatform OVGPlatform { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Release> Releases { get; set; }
    }
}

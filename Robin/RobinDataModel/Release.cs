using System;
using System.Collections.Generic;

namespace Robin
{
    public partial class Release
    {
        public Release()
        {
            Collections = new HashSet<Collection>();
        }

        public long Id { get; set; }
        public long? ID_GB { get; set; }
        public long? ID_GDB { get; set; }
        public long? ID_OVG { get; set; }
        public long? ID_LBG { get; set; }
        public long? ID_LB { get; set; }
        public long? GameId { get; set; }
        public long PlatformId { get; set; }
        public long RegionId { get; set; }
        public long? RomId { get; set; }
        public string Special { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public long PlayCount { get; set; }

        public virtual Game Game { get; set; }
        public virtual Gbrelease Gbrelease { get; set; }
        public virtual Gdbrelease Gdbrelease { get; set; }
        public virtual Lbrelease Lbrelease { get; set; }
        public virtual Lbgame Lbgame { get; set; }
        public virtual Ovgrelease Ovgrelease { get; set; }
        public virtual Platform Platform { get; set; }
        public virtual Region Region { get; set; }
        public virtual Rom Rom { get; set; }

        public virtual ICollection<Collection> Collections { get; set; }
    }
}

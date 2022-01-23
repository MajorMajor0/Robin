﻿using System;
using System.Collections.Generic;

namespace Robin;

public partial class Release
{
	public Release()
	{
		Collections = new HashSet<Collection>();
	}

	public long ID { get; set; }
	public long? ID_GB { get; set; }
	public long? ID_GDB { get; set; }
	public long? ID_OVG { get; set; }
	public long? ID_LBG { get; set; }
	public long? ID_LB { get; set; }
	public long? ID_MB { get; set; }
	public long? Game_ID { get; set; }
	public long Platform_ID { get; set; }
	public long Region_ID { get; set; }
	public long? Rom_ID { get; set; }
	public string Special { get; set; }
	public string Title { get; set; }
	public DateTime? Date { get; set; }
	public string Language { get; set; }
	public string Version { get; set; }
	public long PlayCount { get; set; }

	public virtual Game Game { get; set; }
	public virtual GBRelease GBRelease { get; set; }
	public virtual GDBRelease GDBRelease { get; set; }
	public virtual LBRelease LBRelease { get; set; }
	public virtual MBRelease MBRelease { get; set; }
	public virtual LBGame LBGame { get; set; }
	public virtual OVGRelease OVGRelease { get; set; }
	public virtual Platform Platform { get; set; }
	public virtual Region Region { get; set; }
	public virtual Rom Rom { get; set; }

	public virtual ICollection<Collection> Collections { get; set; }
}

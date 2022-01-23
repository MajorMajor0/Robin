using System;
using System.Collections.Generic;

namespace Robin;

public partial class Platform
{
	public Platform()
	{
		Cores = new List<Core>();
		Releases = new List<Release>();
		Emulators = new HashSet<Emulator>();
	}

	public long ID { get; set; }
	public string Title { get; set; }
	public byte[] LastDate { get; set; }
	public long? ID_GDB { get; set; }
	public long? ID_GB { get; set; }
	public long? ID_MB { get; set; }
	public long? ID_LB { get; set; }

	private long preferredEmulator_ID;
	public long PreferredEmulator_ID
	{
		get => preferredEmulator_ID;
		set
		{
			preferredEmulator_ID = value;
			OnPropertyChanged(nameof(preferredEmulator_ID));
		}
	}
	public string Manufacturer { get; set; }

	private bool preferred;
	public bool Preferred
	{
		get => preferred;
		set
		{
			preferred = value;
			OnPropertyChanged(nameof(preferred));
		}
	}
	public string HiganRomFolder { get; set; }
	public string HiganExtension { get; set; }
	public long HeaderLength { get; set; }
	public string Generation { get; set; }
	public string Type { get; set; }
	public DateTime? Date { get; set; }
	public string FileName { get; set; }
	public string Abbreviation { get; set; }
	public string Developer { get; set; }
	public string Cpu { get; set; }
	public string Sound { get; set; }
	public string Display { get; set; }
	public string Media { get; set; }
	public string Controllers { get; set; }
	public byte[] Rating { get; set; }
	public string Overview { get; set; }
	public DateTime CacheDate { get; set; }

	public virtual GBPlatform GBPlatform { get; set; }
	public virtual GDBPlatform GDBPlatform { get; set; }
	public virtual LBPlatform LBPlatform { get; set; }
	public virtual MBPlatform MBPlatform { get; set; }
	public virtual Emulator PreferredEmulator { get; set; }

	public virtual List<Release> Releases { get; set; }
	public virtual IList<Core> Cores { get; set; }
	public virtual ICollection<Emulator> Emulators { get; set; }
}

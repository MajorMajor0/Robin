﻿using System;
using System.Collections.Generic;

namespace Robin
{
	public partial class Platform
	{
		public Platform()
		{
			Cores = new List<Core>();
			Releases = new List<Release>();
			Emulators = new HashSet<Emulator>();
		}

		public long Id { get; set; }
		public string Title { get; set; }
		public byte[] LastDate { get; set; }
		public long? ID_GDB { get; set; }
		public long? ID_GB { get; set; }
		public long? ID_LB { get; set; }

		private long preferredEmulatorId;
		public long PreferredEmulatorId
		{
			get => preferredEmulatorId;
			set
			{
				preferredEmulatorId = value;
				OnPropertyChanged(nameof(preferredEmulatorId));
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

		public virtual Gbplatform Gbplatform { get; set; }
		public virtual Gdbplatform Gdbplatform { get; set; }
		public virtual Lbplatform Lbplatform { get; set; }
		public virtual Emulator PreferredEmulator { get; set; }

		public virtual List<Release> Releases { get; set; }
		public virtual IList<Core> Cores { get; set; }
		public virtual ICollection<Emulator> Emulators { get; set; }
	}
}
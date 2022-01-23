using System.Collections.Generic;

namespace Robin;

public partial class Core
{
	public Core()
	{
		Platforms = new HashSet<Platform>();
	}

	public long ID { get; set; }
	public string Title { get; set; }
	public string FileName { get; set; }
	public long Emulator_ID { get; set; }

	public virtual Emulator Emulator { get; set; }

	public virtual ICollection<Platform> Platforms { get; set; }
}

using System.Collections.Generic;

namespace Robin;

public partial class MBAttributeCategory
{
	public MBAttributeCategory()
	{
		MBAttributes = new HashSet<MBAttribute>();
	}
	public long Id { get; set; }
	public string Name { get; set; }

	public virtual ICollection<MBAttribute> MBAttributes { get; set; }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public partial class Core
	{
		public string FilePath => $"{FileLocation.Folder}{FileName}";
	}
}

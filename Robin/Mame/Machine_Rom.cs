using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityWorker;
using EntityWorker.Core;
using EntityWorker.Core.Attributes;
using EntityWorker.Core.Interface;
using EntityWorker.Core.InterFace;
using EntityWorker.SQLite;

namespace Robin.Mame
{
	class Machine_Rom : Entity
	{
		[NotNullable]
		[PrimaryKey]
		[ForeignKey(type: typeof(Machine), propertyName:"Machines")]
		public long Machine_ID { get; set; }

		[NotNullable]
		[PrimaryKey]
		[ForeignKey(type: typeof(Rom), propertyName:"Roms")]
		public long Rom_ID { get; set; }
	}
}

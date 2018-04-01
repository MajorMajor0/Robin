using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using EntityWorker;
using EntityWorker.Core;
using EntityWorker.Core.Attributes;
using EntityWorker.Core.Interface;
using EntityWorker.Core.InterFace;
using EntityWorker.SQLite;

namespace Robin.Mame
{
	public class Rom : Entity
	{
		[NotNullable]
		public string Name { get; set; }

		public string Region { get; set; }

		public string CRC { get; set; }

		[NotNullable]
		public string Status { get; set; }

		public long Size { get; set; }

		[NotNullable]
		public bool Optional { get; set; }

		[IndependentData]
		public List<Machine> Machines { get; set; }


		public Rom(XElement xelement)
		{
			GetPropsFromXElement(xelement);
		}


		/// <summary>
		/// Get properties from XElement "rom" from MAME -listxml
		/// </summary>
		/// <param name="xelement">XElement "rom" from MAME -listxml</param>
		public void GetPropsFromXElement(XElement xelement)
		{
			Name = xelement.Attribute("name")?.Value;
			Region = xelement.Attribute("region")?.Value;
			CRC = xelement.Attribute("crc")?.Value;
			Status = xelement.Attribute("status")?.Value ?? "good";

			if (long.TryParse(xelement.Attribute("size")?.Value, out long size))
			{
				Size = size;
			}
		}
	}
}

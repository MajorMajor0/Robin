using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Robin.Mame
{
	partial class Rom
	{
		public Rom(XElement xelement) : this()
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

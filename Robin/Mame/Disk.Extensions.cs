using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Robin.Mame
{
	partial class Disk
	{
		public Disk(XElement xelement)
		{
			GetPropsFromXElement(xelement);
		}

		/// <summary>
		/// Get properties from XElement "disk" from MAME -listxml
		/// </summary>
		/// <param name="xelement">XElement "disk" from MAME -listxml</param>
		public void GetPropsFromXElement(XElement xelement)
		{
			//Name = xelement.Attribute("name")?.Value;
			//SHA1 = xelement.Attribute("sha1")?.Value;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Robin.Mame
{
	partial class Machine
	{

		public Machine ParseMachineFromXelement(XElement xelement)
		{
			Machine machine = new Machine();
			machine.Name = xelement.Attribute("name")?.Value;
			machine.Description = xelement.Element("description")?.Value;
			machine.Year = xelement.Element("year")?.Value;
			machine.Manufacturer = xelement.Element("manufacturer")?.Value;
			machine.Status = xelement.Element("driver")?.Attribute("status")?.Value;
			machine.Emulation = xelement.Element("driver")?.Attribute("emulation")?.Value;
			machine.Players = xelement.Element("input").Attribute("players")?.Value;
			machine.Display = xelement.Element("display")?.Attribute("type")?.Value;
			machine.Control = xelement.Element("input")?.Elements("control")?.FirstOrDefault().Attribute("type").Value;

			return machine;
		}
	}
}

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
		public string CloneOf { get; set; }

		public string SampleOf { get; set; }

		public string Title => Regex.Replace(Name, @"\s\(.*\)", "");



		string parenthesisText;
		string ParenthesisText
		{
			get
			{
				if (parenthesisText == null)
				{
					parenthesisText = GetParenthesisText(Name);
				}
				return parenthesisText;
			}
		}

		public DateTime Date { get; }

		public Machine(XElement xelement) : this()
		{
			GetPropsFromXElement(xelement);
		}

		/// <summary>
		/// Get properties from xelement "machine" in MAME -listxml
		/// </summary>
		/// <param name="xelement">xelement "machine" in MAME -listxml</param>
		public void GetPropsFromXElement(XElement xelement)
		{
			Name = xelement.Attribute("name")?.Value ?? "xxx";
			Description = xelement.Element("description")?.Value ?? "xxx";
			Year = xelement.Element("year")?.Value;
			Manufacturer = xelement.Element("manufacturer")?.Value;
			Status = xelement.Element("driver")?.Attribute("status")?.Value ?? "xxx";
			Emulation = xelement.Element("driver")?.Attribute("emulation")?.Value ?? "xxx";
			Players = xelement.Element("input")?.Attribute("players")?.Value;
			Display = xelement.Element("display")?.Attribute("type")?.Value;
			Control = xelement.Element("input")?.Elements("control")?.FirstOrDefault()?.Attribute("type")?.Value;

			CloneOf = xelement.Attribute("cloneof")?.Value;
			SampleOf = xelement.Attribute("sampleof")?.Value;

			IsMechanical = xelement.Attribute("ismechanical")?.Value == "yes";
			IsBios = xelement.Attribute("isbios")?.Value == "yes";
			IsDevice = xelement.Attribute("isdevice")?.Value == "yes";
			IsRunnable = xelement.Attribute("runnable")?.Value == "yes";

		}

		/// <summary>
		/// Standardize and store text in parenthesis from XElement "machine" attribute name in MAME -listxml
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetParenthesisText(string name)
		{
			// Standardize version/revision text within parenthesis
			string parenthesisFindText = @"\(.*\)";
			string revisionFindText = @"rev[ison\.]*\s?";
			string parenthesisText = Regex.Match(name, parenthesisFindText).Value;

			if (parenthesisText != null)
			{
				parenthesisText = parenthesisText.Replace("US", "USA");

				if (Regex.IsMatch(parenthesisText, revisionFindText, RegexOptions.IgnoreCase))
				{
					parenthesisText = Regex.Replace(parenthesisText, revisionFindText, "Rev ", RegexOptions.IgnoreCase);
				}
				else
				{
					parenthesisText = Regex.Replace(parenthesisText, @"v[ersion\.]*\s?", "V ", RegexOptions.IgnoreCase);
				}
			}
			return parenthesisText;
		}


		/// <summary>
		/// Get Robin.Region from standardized parenthesis text
		/// </summary>
		/// <returns></returns>
		public static Region GetRegion(string parenthesisText)
		{
			foreach (Region region in R.Data.Regions.Local)
			{
				if (parenthesisText != null && (parenthesisText.Contains(region.Title) || parenthesisText.Contains(region.Datomatic ?? "XXX") || parenthesisText.Contains(region.UNCode ?? "XXX")))
				{
					return region;
				}
			}
			return null;
		}

		public DateTime? GetDate()
		{
			if (DateTime.TryParseExact(Year, "yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime datecatcher))
			{
				return datecatcher;
			}
			return null;
		}

	}
}

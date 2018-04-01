using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	public abstract class Entity
	{
		[PrimaryKey]
		[NotNullable]
		public long ID { get; set; }
	}


	public class Machine : Entity
	{
		[NotNullable]
		public string Name { get; set; }

		[NotNullable]
		public string Description { get; set; }

		public string Year { get; set; }

		public string Manufacturer { get; set; }

		[NotNullable]
		public string Status { get; set; }

		[NotNullable]
		public string Emulation { get; set; }

		public string Players { get; set; }

		public string Display { get; set; }

		public string Control { get; set; }


		public bool IsBios { get; set; }

		public bool IsMechanical { get; set; }

		public bool IsDevice { get; set; }

		public bool IsRunnable { get; set; }


		public string CloneOf { get; set; }

		public string SampleOf { get; set; }

		public string Title => Regex.Replace(Name, @"\s\(.*\)", "");

		[ForeignKey(type: typeof(Machine), propertyName: "Parent")]
		public long Parent_ID { get; set; }

		[ForeignKey(type: typeof(Machine), propertyName: "Sample")]
		public long Sample_ID { get; set; }

		[IndependentData]
		public List<Rom> Roms { get; set; }

		[IndependentData]
		public Machine Parent { get; set; }

		[IndependentData]
		public Machine Sample { get; set; }









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

		public Machine(XElement xelement)
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

	// EntityWorker.Core has its own way to validate the data.
	// lets create an object and call it UserRule
	// above the User class we have specified this class to be executed before save and after.
	// by adding [Rule(typeof(UserRule))] to the user class
	public class MachineRule : IDbRuleTrigger<Machine>
	{
		public void BeforeSave(IRepository repository, Machine machine)
		{
			//if (string.IsNullOrEmpty(machine.Password) || string.IsNullOrEmpty(machine.UserName))
			//{
			//	// this will do a transaction rollback and delete all changes that have happened to the database
			//	throw new Exception("Password or UserName can not be empty");

			//}
		}


		public void AfterSave(IRepository repository, Machine itemDbEntity, object objectId)
		{
			//// lets do some changes here, when the item have updated..
			//machine.Password = MethodHelper.EncodeStringToBase64(machine.Password);
			//// and now we want to save this change to the database 
			//// the EntityWorker.Core will now know that it need to update the database agen.
			//// it will detect the changes that has been made to the current object
		}
	}
}

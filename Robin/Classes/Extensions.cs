/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace Robin
{
	static class Extensions
	{

		public static BitmapImage ToBitmapImage(this Bitmap bitmap)
		{
			using (var memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();

				return bitmapImage;
			}
		}


		public static bool DownloadFileFromDB(this WebClient webclient, string url, string fileName)
		{
			try
			{
				webclient.SetStandardHeaders();
				DBTimers.Wait(url);
				Debug.WriteLine("Hit DB at: " + url + "  " + DateTime.Now);
				webclient.DownloadFile(url, fileName);
				return true;
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					var response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
					Debug.WriteLine(response);
					Debug.WriteLine(ex.Message);
					Debug.WriteLine(ex.Source);
					Debug.WriteLine(ex.TargetSite);
					Debug.WriteLine(ex.Status);
					Debug.WriteLine(ex.HResult);
				}
				return false;	
			}
		}

		public static int Save(this DbContext dbContext)
		{
			int i = 0;
			dbContext.Backup();
			dbContext.ChangeTracker.DetectChanges();

			try
			{
				i = dbContext.SaveChanges();
				//reporter.Report("Database changes saved successfully");
				return i;
			}
			catch (DbEntityValidationException dbEx)
			{
				foreach (var validationErrors in dbEx.EntityValidationErrors)
				{
					foreach (var validationError in validationErrors.ValidationErrors)
					{
						Trace.TraceInformation("Property: {0} Error: {1}",
												validationError.PropertyName,
												validationError.ErrorMessage);

						Reporter.Report("Data validation error...\nProperty: " + validationError.PropertyName + "Error: " + validationError.ErrorMessage);
					}
				}
				return 0;
			}
			finally
			{

			}

		}

		public static void Backup(this DbContext dbcontext)
		{
			// Backup existing database
			Directory.CreateDirectory(FileLocation.Backup);
			string Date = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
			string CurrentFile = dbcontext.Database.Connection.ConnectionString
				.Replace("data source=\"|DataDirectory|", FileLocation.Folder)
				.Replace("data source=\"C:\\Users\\Major Major\\Documents\\Visual Studio 2017\\Projects\\Robin\\Robin"
						, FileLocation.Folder)
				.Replace(".db3\"", ".db3");
			string BackupFile = FileLocation.Backup + Path.GetFileNameWithoutExtension(CurrentFile) + Date + Path.GetExtension(CurrentFile);
			try
			{
				//File.Copy(CurrentFile, BackupFile); TODO uncomment
				//reporter.Report("DB file backed up to " + BackupFile);
			}
			catch (Exception ex)
			{
				Reporter.Report("Failed to backup file.");
				Reporter.Report(ex.Message);
			}
		}

		public static void AddRange<T>(this ObservableCollection<T> OC, IEnumerable<T> OCtoADD)
		{
			if (OCtoADD != null)
			{
				foreach (T item in OCtoADD)
				{
					OC.Add(item);
				}
			}
		}

		public static string Clean(this string dirtyString)
		{

			string ggg = Regex.Replace(dirtyString, @"(?:\r\n|\r(?!\n)|(?<!\r)\n){2,}", "\r\n")
										.Replace(@"&lt", "<")
										.Replace(@"&gt", ">")
										.Replace(@"&quot", "\"")
										.Replace(@"&apos", "'")
										.Replace(@"&nbsp", " ");


			return ggg;
		}


		public static string Wash(this string title)
		{
			string washed = Regex.Replace(title, @"\A(A |The |La |El )", "");
			 washed = washed.Replace("IV", "4").Replace("III", "3").Replace("II", "2").
				Replace(", A", "").Replace(", The", "").Replace(", An", "").Replace(", La", "").Replace(", El", "").ToLower().
				Replace(" the ", "").Replace(" and ", "").Replace(" a ", "").
				Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("$", "").
				Replace("/", "").Replace("?", "").Replace("-", "").Replace("*", "").
				Replace("+", "").Replace("&", "").Replace("!", "").Replace(".", "").Replace(",", "").
				Replace("'", "").Replace(":", "").Replace(" ", "");
			
			return washed;
		}

		public static string SafeGetA(this XElement element,
			string element1 = null, string element2 = null, string element3 = null,
			string attribute = null)
		{
			string test = Convert.ToInt16((!string.IsNullOrEmpty(element1))).ToString() +
						  Convert.ToInt16((!string.IsNullOrEmpty(element2))).ToString() +
						  Convert.ToInt16((!string.IsNullOrEmpty(element3))).ToString() +
						  Convert.ToInt16((!string.IsNullOrEmpty(attribute))).ToString();

			int i = 0;
			i++;
			switch (test)
			{
				case "0001":
					if (element.Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return element.Attribute(attribute).Value;
					}

				case "1000":
					if (element.Element(element1) == null)
					{
						return null;
					}
					else
					{
						return element.Element(element1).Value;
					}


				case "1001":
					if (element.Element(element1) == null ||
						element.Element(element1).Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return element.Element(element1).Attribute(attribute).Value;
					}


				case "1100":
					if (element.Element(element1) == null ||
						element.Element(element1).Element(element2) == null)
					{
						return null;
					}
					else
					{
						return element.Element(element1).Element(element2).Value;
					}


				case "1101":
					if (element.Element(element1) == null ||
						element.Element(element1).Element(element2) == null ||
						element.Element(element1).Element(element2).Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return element.Element(element1).Element(element2).Attribute(attribute).Value;
					}

				case "1110":
					if (element.Element(element1) == null ||
						element.Element(element1).Element(element2) == null ||
						element.Element(element1).Element(element2).Element(element3) == null)
					{
						return null;
					}
					else
					{
						return element.Element(element1).Element(element2).Element(element3).Value;
					}

				case "1111":
					if (element.Element(element1) == null ||
						element.Element(element1).Element(element2) == null ||
						element.Element(element1).Element(element2).Element(element3) == null ||
						element.Element(element1).Element(element2).Element(element3).Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return element.Element(element1).Element(element2).Element(element3).Attribute(attribute).Value;
					}

				default:
					return null;
			}

		}

		public static string SafeGetB(this XDocument document,
			string element1 = null, string element2 = null, string element3 = null, string element4 = null, string attribute = null)
		{
			string test = Convert.ToInt16((!string.IsNullOrEmpty(element1))).ToString() +
				Convert.ToInt16((!string.IsNullOrEmpty(element2))).ToString() +
				Convert.ToInt16((!string.IsNullOrEmpty(element3))).ToString() +
				Convert.ToInt16((!string.IsNullOrEmpty(element4))).ToString() +
				Convert.ToInt16((!string.IsNullOrEmpty(attribute))).ToString();

			int i = 0;
			i++;
			switch (test)
			{
				case "00001":
					if (document.Root.Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Attribute(attribute).Value;
					}

				case "10000":
					if (document.Root.Element(element1) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Value;
					}

				case "10001":
					if (document.Root.Element(element1) == null ||
						document.Root.Element(element1).Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Attribute(attribute).Value;
					}

				case "11000":
					if (document.Root.Element(element1) == null ||
						document.Root.Element(element1).Element(element2) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Element(element2).Value;
					}


				case "11001":
					if (document.Root.Element(element1) == null ||
						document.Root.Element(element1).Element(element2) == null ||
						document.Root.Element(element1).Element(element2).Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Element(element2).Attribute(attribute).Value;
					}

				case "11100":
					if (document.Root.Element(element1) == null ||
						document.Root.Element(element1).Element(element2) == null ||
						document.Root.Element(element1).Element(element2).Element(element3) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Element(element2).Element(element3).Value;
					}

				case "11101":
					if (document.Root.Element(element1) == null ||
						document.Root.Element(element1).Element(element2) == null ||
						document.Root.Element(element1).Element(element2).Element(element3) == null ||
						document.Root.Element(element1).Element(element2).Element(element3).Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Element(element2).Element(element3).Attribute(attribute).Value;
					}

				case "11110":
					if (document.Root.Element(element1) == null ||
						document.Root.Element(element1).Element(element2) == null ||
						document.Root.Element(element1).Element(element2).Element(element3) == null ||
						document.Root.Element(element1).Element(element2).Element(element3).Element(element4) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Element(element2).Element(element3).Element(element4).Value;
					}

				case "11111":
					if (document.Root.Element(element1) == null ||
						document.Root.Element(element1).Element(element2) == null ||
						document.Root.Element(element1).Element(element2).Element(element3) == null ||
						document.Root.Element(element1).Element(element2).Element(element3).Element(element4) == null ||
						document.Root.Element(element1).Element(element2).Element(element3).Element(element4).Attribute(attribute) == null)
					{
						return null;
					}
					else
					{
						return document.Root.Element(element1).Element(element2).Element(element3).Element(element4).Attribute(attribute).Value;
					}

				default:
					return null;
			}

		}

		//Safeget info from xml nested element
		public static string SafeGetBoxArt(this XDocument Xdoc, string side, string resolution = null, string type = "Game")
		{
			try
			{
				if (Xdoc == null) return null;

				if (Xdoc.Root.Element(type) == null ||
					Xdoc.Root.Element(type).Element("Images") == null ||
					Xdoc.Root.Element(type).Element("Images").Elements("boxart") == null ||
					Xdoc.Root.Element(type).Element("Images").Elements("boxart").First(x => x.Attribute("side").Value == side) == null
					)
				{
					return null;
				}

				if (resolution == "thumb")
				{
					if (Xdoc.Root.Element(type).Element("Images").Elements("boxart").
						First(x => x.Attribute("side").Value == side).Attribute("thumb") == null)
					{
						return null;
					}
					else
					{
						return Xdoc.Root.Element(type).Element("Images").Elements("boxart").
							First(x => x.Attribute("side").Value == side).Attribute("thumb").Value;
					}
				}
				else
				{
					return (Xdoc.Root.Element(type).Element("Images").Elements("boxart").First(x => x.Attribute("side").Value == side)).Value;
				}
			}

			catch (InvalidOperationException)
			{
				return null;
			}
		}

		public static void SetStandardHeaders(this WebClient webclient)
		{
			webclient.Headers["ACCEPT"] = CONSTANTS.HEADER_ACCEPT;
			webclient.Headers["ACCEPT_ENCODING"] = CONSTANTS.HEADER_ACCEPT_ENCODING;
			webclient.Headers["ACCEPT_LANGUAGE"] = CONSTANTS.HEADER_ACCEPT_LANGUAGE;
			webclient.Headers["User-Agent"] = CONSTANTS.VERSION + "(" + Environment.OSVersion.ToString() + ")";
		}

		public static bool SafeDownloadStringDB(this WebClient webclient, string url, out string downloadtext)
		{
			webclient.SetStandardHeaders();
			try
			{
				DBTimers.Wait(url);
				Debug.WriteLine("Hit DB at: " + url + "  " + DateTime.Now);
				downloadtext = webclient.DownloadString(url);

				return true;
			}

			catch (WebException ex)
			{
				var response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

				Debug.WriteLine(response);
				Debug.WriteLine(ex.Message);
				Debug.WriteLine(ex.Source);
				Debug.WriteLine(ex.TargetSite);
				Debug.WriteLine(ex.Status);
				Debug.WriteLine(ex.HResult);
				downloadtext = response;
				return false;
			}
		}
	}
}

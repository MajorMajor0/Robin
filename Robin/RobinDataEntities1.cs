//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Data.Entity.Validation;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Xml.Linq;

//namespace Robin
//{
//	public partial class RobinDataEntities : DbContext
//	{
//		//	public void GetPlatforms(LocalDBs database)
//		//	{
//		//		XDocument XMLdoc = new XDocument();
//		//		string url;
//		//		using (WebClient webclient = new WebClient())
//		//		{
//		//			webclient.Headers["User-Agent"] = "AlfredENeuman";

//		//			if (database == LocalDBs.GiantBomb)
//		//			{
//		//				int offset = 0;
//		//				string urlbase = @"http://www.giantbomb.com/api/platforms/?api_key=561e43b5ca29977bb624d52357764e459e22d3bd&field_list=name,id,company,image,release_date,deck";
//		//				url = urlbase;
//		//				int total = 100000;
//		//				int i = 0;
//		//				int j = 0;

//		//				Stopwatch Watch = new Stopwatch();
//		//				List<Platform> platlist = new List<Platform>();
//		//				while (i <= total)
//		//				{
//		//					webclient.Headers["User-Agent"] = "AlfredENeuman";
//		//					try
//		//					{
//		//						XMLdoc = XDocument.Parse(webclient.DownloadString(url));
//		//					}
//		//					catch (WebException)
//		//					{
//		//						continue;
//		//					}

//		//					int r = int.Parse(XMLdoc.SafeGetB(element1: "number_of_page_results"));

//		//					foreach (XElement element in XMLdoc.Descendants("platform"))
//		//					{
//		//						Platform platform = new Platform();

//		//						platform.Title = element.SafeGetA(element1: "name");
//		//						platform.ID_GB = long.Parse(element.SafeGetA(element1: "id"));

//		//						if (!(Platforms.Local.Any(x => x.ID_GB == platform.ID_GB)))
//		//						{
//		//							Watch.Restart();
//		//							platlist.Add(platform);
//		//							Debug.WriteLine(Watch.ElapsedMilliseconds.ToString() + " ms to add platform");
//		//							j++;
//		//						}
//		//						i++;
//		//					}

//		//					total = int.Parse(XMLdoc.SafeGetB(element1: "number_of_total_results"));
//		//					offset = +int.Parse(XMLdoc.SafeGetB(element1: "number_of_page_results"));
//		//					url = urlbase + @"&offset=" + offset;
//		//				}
//		//				Platforms.Local.AddRange(platlist);
//		//			}

//		//			if (database == LocalDBs.GamesDB)
//		//			{
//		//				url = @"http://thegamesdb.net/api/GetPlatformsList.php";


//		//				// Pull down the xml file containing platform data from gamesdb
//		//				try
//		//				{
//		//					XMLdoc = XDocument.Load(url);
//		//				}
//		//				catch (WebException)
//		//				{

//		//				}

//		//				foreach (XElement element in XMLdoc.Descendants("Platform"))
//		//				{
//		//					Platform platform = new Platform();

//		//					// platform.ID_GamesDB = int.Parse(XMLdoc.SafeGet2("Platform", "id") ?? "0");
//		//					platform.Title = element.SafeGetA(element1: "name");
//		//					platform.ID_GDB = long.Parse(element.SafeGetA(element1: "id"));
//		//				}
//		//			}
//		//		}
//		//	}
//	}
//}

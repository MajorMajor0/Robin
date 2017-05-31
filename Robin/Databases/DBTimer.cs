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
 
using System.Diagnostics;

namespace Robin
{
	public class DBTimers
	{
		public static DBTimer GamesDB = new DBTimer(1);
		public static DBTimer GiantBomb = new DBTimer(1100);
		public static DBTimer GameFAQ = new DBTimer(1500);
		public static DBTimer LaunchBox = new DBTimer(1000);

		public static void Wait(string url)
		{
			if (url.ToLower().Contains("thegamesdb.net"))
			{
				GamesDB.Wait();
				return;
			}

			if (url.ToLower().Contains("giantbomb.com"))
			{
				GiantBomb.Wait();
				return;
			}

			if (url.ToLower().Contains("gamefaqs.net"))
			{
				GameFAQ.Wait();
				return;
			}

			if (url.ToLower().Contains("launchbox-app.com"))
			{
				LaunchBox.Wait();
				return;
			}
		}

		public class DBTimer : Stopwatch
		{
			public int WaitTime;

			public DBTimer(int threshold)
			{
				WaitTime = threshold;
				Start();
			}

			public void Wait()
			{
				while (ElapsedMilliseconds < WaitTime) { }

				Restart();
			}
		}
	}
}

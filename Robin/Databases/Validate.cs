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
 
 using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;

namespace Robin
{
	class Validate
	{
		public static void ValidateGame()
		{
			using (RobinDataEntities Rdata = new RobinDataEntities())
			{
				Rdata.Configuration.LazyLoadingEnabled = false;
				Rdata.Games.Load();
				Rdata.Releases.Load();
				List<Game> list = new List<Game>();

				int i = 0;
				foreach (Game game in Rdata.Games)
				{
					if (game.Releases.Count == 0)
					{
						i++;
						Debug.WriteLine(game.ID);
						list.Add(game);
					}
				}

				Debug.WriteLine("Total = " + i);
				Rdata.Games.RemoveRange(list);
				int k = Rdata.Save();
				Reporter.Report(k + " games removed");
			}
		}
	}
}

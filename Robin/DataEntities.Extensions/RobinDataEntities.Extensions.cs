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

using System.Data.Entity;
using System.Linq;

namespace Robin
{
	public partial class RobinDataEntities
	{
		public RobinDataEntities(object sender)
		{
			Configuration.LazyLoadingEnabled = false;
			Configuration.AutoDetectChangesEnabled = false;
			Platforms.Load();
			Roms.Load();
			Releases.Load();
			Games.Load();
			Regions.Load();
			Collections.Load();
			Collections.Include(x => x.Games).Load();
			Collections.Include(x => x.Releases).Load();

			if (sender.GetType() == typeof(MainWindowViewModel))
			{
				Emulators.Load();
				Emulators.Include(x => x.Platforms1).Load();

				foreach (Game game in Games)
				{
					game.Releases = game.Releases.OrderBy(x => x.Region.Priority).ToList();
				}
			}

			if (sender.GetType() == typeof(DatabaseWindowViewModel))
			{
				LBImages.Load();
				LBGames.Load();
				LBPlatforms.Load();
				GDBPlatforms.Load();
				GDBReleases.Load();
				GBGames.Load();
				GBReleases.Load();
				GBPlatforms.Load();
				OVGReleases.Load();
			}
		}
	}
}

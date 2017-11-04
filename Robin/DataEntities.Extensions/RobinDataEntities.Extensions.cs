﻿/*This file is part of Robin.
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
using System.Diagnostics;
using System.Linq;

namespace Robin
{
	public static class R
	{
		public static RobinDataEntities Data;
	}


	public partial class RobinDataEntities
	{
		public RobinDataEntities(object sender)
		{

			Configuration.LazyLoadingEnabled = false;
			Configuration.AutoDetectChangesEnabled = false;

            Stopwatch Watch = Stopwatch.StartNew();

			Platforms.Include(x=>x.Emulators).Load();

            Reporter.Report("Platforms loaded " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
			Roms.LoadAsync();
            Reporter.Report("Roms loaded " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
            Releases.LoadAsync();
            Reporter.Report("Releases loaded " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
            Games.LoadAsync();
            Reporter.Report("Games loaded " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
            Regions.LoadAsync();
            Reporter.Report("Regions loaded " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
            Collections.Include(x=>x.Games).Include(x=>x.Releases).Load();
            Reporter.Report("Collections loaded " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
            Emulators.Include(x=>x.Platforms).Load();
            Reporter.Report("Emulators loaded " + Watch.Elapsed.Seconds + " s."); Watch.Restart();

            foreach (Game game in Games)
			{
				game.Releases = game.Releases.OrderBy(x => x.Region.Priority).ThenByDescending(x=>x.Version).ToList();
			}
            Reporter.Report("Games ordered " + Watch.Elapsed.Seconds + " s."); Watch.Restart();
        }

		//		public void LoadAll()
		//		{
		//#if DEBUG
		//			Stopwatch Watch = new Stopwatch();
		//			Watch.Start();
		//			Stopwatch Watch1 = new Stopwatch();
		//			Watch1.Start();
		//			Debug.WriteLine("\n\n****************************************************");
		//#endif
		//			Task[] tasks = new Task[7];

		//			tasks[0] = Task.Factory.StartNew(() =>
		//			{
		//				Platforms.Load();
		//#if DEBUG
		//				Debug.WriteLine("Platforms: " + Watch.ElapsedMilliseconds); Watch.Restart();
		//#endif
		//			});

		//			tasks[1] = Task.Factory.StartNew(() =>
		//			{
		//				Roms.Load();
		//#if DEBUG
		//				Debug.WriteLine("Roms: " + Watch.ElapsedMilliseconds); Watch.Restart();
		//#endif
		//			});

		//			tasks[2] = Task.Factory.StartNew(() =>
		//			{
		//				Releases.Load();
		//#if DEBUG
		//				Debug.WriteLine("Releases: " + Watch.ElapsedMilliseconds); Watch.Restart();
		//#endif
		//			});

		//			tasks[3] = Games.LoadAsync();
		//			Task.Factory.StartNew(() =>
		//			{
		//#if DEBUG
		//				Debug.WriteLine("Games: " + Watch.ElapsedMilliseconds); Watch.Restart();
		//#endif
		//			});

		//			tasks[4] = Task.Factory.StartNew(() =>
		//			{
		//				Regions.Load();
		//#if DEBUG
		//				Debug.WriteLine("Regions: " + Watch.ElapsedMilliseconds); Watch.Restart();
		//#endif
		//			});

		//			tasks[5] = Task.Factory.StartNew(() =>
		//			{
		//				Collections.LoadAsync();
		//#if DEBUG
		//				Debug.WriteLine("Collections: " + Watch.ElapsedMilliseconds); Watch.Restart();
		//#endif
		//			});

		//			tasks[6] = Task.Factory.StartNew(() =>
		//			{
		//				Emulators.LoadAsync();
		//#if DEBUG
		//				Debug.WriteLine("Emulators: " + Watch.ElapsedMilliseconds); Watch.Restart();
		//#endif
		//			});

		//#if DEBUG
		//			Debug.WriteLine("Total: " + Watch1.ElapsedMilliseconds);
		//			Debug.WriteLine("****************************************************\n\n");
		//#endif
		//			//Collections.Include(x => x.Games).Load();
		//			//Collections.Include(x => x.Releases).Load();

		//			//Emulators.Include(x => x.Platforms1).Load();

		//			Task.WaitAll(tasks);

		//			foreach (Game game in Games)
		//			{
		//				game.Releases = game.Releases.OrderBy(x => x.Region.Priority).ToList();
		//			}
		//		}
	}
}

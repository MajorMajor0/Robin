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
using System.ComponentModel;

namespace Robin
{
	public interface IDBobject
	{
		string Title { get; }
		bool Included { get; }
		bool IsCrap { get; set; }
		bool Preferred { get; set; }
		bool Unlicensed { get; }
		bool HasArt { get; }

		void Play();

		void ScrapeArt();
	}

	public interface IDBRelease
	{
		long ID { get; }
		string Title { get; }
		Region Region { get; }
		string Overview { get; }
		DateTime Date { get; }
	}

	public interface IDBPlatform
	{
		long ID { get; }
		string Title { get; }
		string Manufacturer { get; }
	}

	public class DatabaseCache
	{
		public LocalDB DB { get; set; }
		public List<IDBobject> Platforms;
		public List<IDBobject> Games;
		public List<IDBobject> Releases;
	}

	public enum LocalDB
	{
		[Description("Unknown")]
		Unknown = 0,
		[Description("GamesDB")]
		GamesDB = 1,
		[Description("GiantBomb")]
		GiantBomb = 2,
		[Description("OpenVGDB")]
		OpenVGDB = 3,
		[Description("LaunchBox")]
		LaunchBox = 4
	}
	
}


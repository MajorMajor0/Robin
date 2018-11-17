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
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;

namespace Robin.Core
{
	public interface IDBobject
	{
		string Title { get; }
		string MainDisplay { get; }
		bool Included { get; }
		bool IsCrap { get; set; }
		bool Preferred { get; set; }
		bool Unlicensed { get; }
		bool HasArt { get; }
		string WhyCantIPlay { get; }

		void Play();

		int ScrapeArt(ArtType artType, LocalDB localDB);
	}

	public interface IDBRelease
	{
		long ID { get; }
		string Title { get; }
		string Overview { get; }
		string RegionTitle { get; }
		LocalDB LocalDB { get; }

		Region Region { get; }

		DateTime? Date { get; }
		int ScrapeBoxFront();
	}

	public interface IDBPlatform
	{
		long ID { get; }
		string Title { get; }
		string Manufacturer { get; }
		DateTime? Date { get; }
		DateTime CacheDate { get; set; }
		IList Releases { get; }
		bool Preferred { get; }
		Platform RPlatform { get; }
		int MatchedReleaseCount { get; }
	}

	/// <summary>
	/// A local cache of an online database, Games DB, GiantBomb, LaunchBox...
	/// </summary>
	public interface IDB : IDisposable
	{
		LocalDB DB { get; }
		string Title { get; }
		DbSet Platforms { get; }
		DbSet Releases { get; }
		bool HasRegions { get; }

		/// <summary>
		/// Update the list of platforms in the local DB cache, GBPlatform, GDBlatform, LBPlatform...
		/// </summary>
		void CachePlatforms();

		/// <summary>
		/// Update the local DB cache of realeases for one platform, inlcluding the list of releases and associated metadata. LBRealease, GBRelease, GDBRelease...
		/// </summary>
		/// <param name="platform"></param>
		void CachePlatformReleases(Platform platform);

		/// <summary>
		/// Update the local DB cache of games for one platform, inlcluding the list o games and associated metadata.
		/// </summary>
		/// <param name="platform"></param>
		void CachePlatformGames(Platform platform);
		
		/// <summary>
		/// Update the local DB cache of platform associated metadata
		/// </summary>
		/// <param name="platform">Robin.Platform associated with the DBPlatorm to update.</param>
		void CachePlatformData(Platform platform);

		/// <summary>
		/// Report to the UI how many database entries and of what type have been updated or added since the last save changes for a local DB cache.
		/// </summary>
		/// <param name="detect">Whether to detect changes prior to reporting. Detecting changes takes about 4 seconds. This can be set to false if no changes have been made since the last detect changes. Detecting changes is only necessary for updates, it is not necessary to detect additions.</param>
		void ReportUpdates(bool detect);
	}

	public interface ICollectionItem
	{
		string Title { get; }

	}

	public enum LocalDB
	{
		[Description("Unknown")]
		Unknown,
		[Description("Games DB")]
		GamesDB,
		[Description("GiantBomb")]
		GiantBomb,
		[Description("Open VGDB")]
		OpenVGDB,
		[Description("LaunchBox")]
		LaunchBox,
		[Description("Datomatic")]
		Datomatic,
		[Description("Robin")]
		Robin
	}

	public enum ArtType
	{
		[Description("All")]
		All,
		[Description("Box Front")]
		BoxFront,
		[Description("Box Back")]
		BoxBack,
		[Description("Banner")]
		Banner,
		[Description("Console")]
		Console,
		[Description("Controller")]
		Controller,
		[Description("Screen Shot")]
		Screen,
		[Description("Clear Logo")]
		Logo,
		[Description("Box 3D")]
		Box3D,
		[Description("Marquee")]
		Marquee,
		[Description("Control Panel")]
		ControlPanel,
		[Description("Control Information")]
		ControlInformation,
		[Description("Cartridge Front")]
		CartFront,
		[Description("Cartridg Back")]
		CartBack,
		[Description("Cartridge 3D")]
		Cart3D
	}

}


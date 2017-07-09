using System;
using System.Data.Entity;


namespace Robin
{
	class RobinDB : IDB
	{
		public string Title { get { return "Robin"; } }

		public LocalDB DB { get { return LocalDB.Robin; } }

		public DbSet Platforms => R.Data.Platforms;

		public DbSet Releases => R.Data.Releases;

		public void CachePlatformData(Platform platform)
		{
			throw new NotImplementedException();
		}

		public void CachePlatformGames(Platform platform)
		{
			throw new NotImplementedException();
		}

		public void CachePlatformReleases(Platform platform)
		{
			throw new NotImplementedException();
		}

		public void CachePlatforms()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}

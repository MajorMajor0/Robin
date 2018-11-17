//using System;
//using System.Data.Entity;
//using System.Diagnostics;
//using System.Linq;

//namespace Robin
//{
//    class RobinDB : IDB
//    {
//        public string Title { get { return "Robin"; } }

//        public LocalDB DB { get { return LocalDB.Robin; } }

//        public DbSet Platforms => R.Data.Platforms;

//        public DbSet Releases => R.Data.Releases;

//        public void CachePlatformData(IDBPlatform idbPlatform)
//        {
//            throw new NotImplementedException();
//        }

//        public void CachePlatformGames(IDBPlatform idbPlatform)
//        {
//            throw new NotImplementedException();
//        }

//        public void CachePlatformReleases(IDBPlatform idbPlatform)
//        {
//            throw new NotImplementedException();
//        }

//        public void CachePlatforms()
//        {
//            throw new NotImplementedException();
//        }

//        public void CachePlatform(IDBPlatform idbPlatform)
//        {
//            throw new NotImplementedException();
//        }

//        public void ReportUpdates(bool detect)
//        {


//        }


//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

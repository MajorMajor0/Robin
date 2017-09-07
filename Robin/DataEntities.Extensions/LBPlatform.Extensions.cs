using System.Collections;
using System.Linq;

namespace Robin
{
    public partial class LBPlatform : IDBPlatform
    {
        public IList Releases => LBReleases;

        public IList Games => LBGames;

        public Platform RPlatform
        {
            get
            {
                return R.Data.Platforms.FirstOrDefault(x => x.ID_LB == ID);
            }
        }

        public bool Preferred
        {
            get
            {
                if (RPlatform != null)
                {
                    return RPlatform.Preferred;
                }
                return false;
            }
        }
    }
}




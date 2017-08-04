using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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




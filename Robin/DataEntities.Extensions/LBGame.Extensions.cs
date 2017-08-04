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
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
    public partial class LBGame 
    {
        //public static List<LBGame> GetGames(Platform platform)
        //{
        //    R.Data.LBGames.Load();
        //    R.Data.LBGames.Include(x => x.LBImages).Load();
        //    R.Data.Regions.Load();
        //    return R.Data.LBGames.Where(x => x.LBPlatform_ID == platform.ID_LB).ToList();
        //}

        public string RegionTitle
        {
            get; set;
        }

        public Region Region { get { return null; } }

        public string Regions
        {
            get
            {
                if (LBImages != null)
                {
                    return string.Join(", ", LBImages.Select(x => x.Region.Title).Distinct());
                }
                return null;
            }
        }

        //public string BoxFrontURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Front"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Front"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string BoxBackURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Back"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - Back"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string Box3DURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - 3D"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Box - 3D"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string LogoURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Clear Logo"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Clear Logo"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string BannerURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Banner"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Banner"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string MarqueeURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Marquee"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Marquee"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string ScreenURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Screenshot - Gameplay"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Screenshot - Gameplay"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string ControlPanelURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Control Panel"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Control Panel"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string ControlInformationURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Controls Information"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Arcade - Controls Information"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string CartFrontURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Front"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Front"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string CartBackURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Back"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - Back"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}

        //public string Cart3DURL(long? regionID)
        //{
        //    LBImage lbImage = null;
        //    if (regionID != null)
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - 3D"));
        //    }

        //    else
        //    {
        //        lbImage = LBImages.FirstOrDefault(x => (x.Region_ID == regionID && x.Type == "Cart - 3D"));
        //    }

        //    if (lbImage != null)
        //    {
        //        return Launchbox.IMAGESURL + lbImage.FileName;
        //    }

        //    else
        //    {
        //        return null;
        //    }
        //}


        //public string BoxFrontPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-BXF.jpg"; }
        //}

        //public string BoxBackPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-BXB.jpg"; }
        //}

        //public string Box3DPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-BX3.jpg"; }
        //}

        //public string ScreenPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-SCR.jpg"; }
        //}

        //public string LogoPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-LGO.jpg"; }
        //}

        //public string BannerPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-BNR.jpg"; }
        //}

        //public string CartFrontPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-CF.jpg"; }
        //}

        //public string Cart3DPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-C3D.jpg"; }
        //}

        //public string MarqueePath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-MAR.jpg"; }
        //}

        //public string ControlPanelPath
        //{
        //    get { return FileLocation.Temp + "LBR-" + ID + "-BXF.jpg"; }
        //}


        //public int ScrapeBoxFront()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(BoxFrontPath))
        //        {
        //            if (BoxFrontURL(null) != null)
        //            {
        //                Reporter.Report("Getting front box art for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(BoxFrontURL(null), BoxFrontPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("BoxFrontPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No front box art URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeBoxBack()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(BoxBackPath))
        //        {
        //            if (BoxBackURL(null) != null)
        //            {
        //                Reporter.Report("Getting back box art for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(BoxBackURL(null), BoxBackPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("BoxBackPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No back box art URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeBox3D()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(Box3DPath))
        //        {
        //            if (Box3DURL(null) != null)
        //            {
        //                Reporter.Report("Getting 3D box art for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(Box3DURL(null), Box3DPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("Box3DPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No 3D box art URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeScreen()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(ScreenPath))
        //        {
        //            if (ScreenURL(null) != null)
        //            {
        //                Reporter.Report("Getting screen shot for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(ScreenURL(null), ScreenPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("ScreenPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No screen shot URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeLogo()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(LogoPath))
        //        {
        //            if (BoxFrontURL(null) != null)
        //            {
        //                Reporter.Report("Getting clear logo for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(LogoURL(null), LogoPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("LogoPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No clear logo URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeBanner()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(BannerPath))
        //        {
        //            if (BannerURL(null) != null)
        //            {
        //                Reporter.Report("Getting banner for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(BannerURL(null), BannerPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("BannerPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No banner URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeCartFront()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(CartFrontPath))
        //        {
        //            if (CartFrontURL(null) != null)
        //            {
        //                Reporter.Report("Getting front cartridge art for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(CartFrontURL(null), CartFrontPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("CartFrontPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No front cartridg art URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeCart3D()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(Cart3DPath))
        //        {
        //            if (BoxFrontURL(null) != null)
        //            {
        //                Reporter.Report("Getting 3D cartridge art for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(Cart3DURL(null), Cart3DPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("Cart3DPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No 3D cartridge art URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeMarquee()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(MarqueePath))
        //        {
        //            if (MarqueeURL(null) != null)
        //            {
        //                Reporter.Report("Getting marquee art for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(MarqueeURL(null), MarqueePath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("MarqueePath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No marquee art URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        //public int ScrapeControlPanel()
        //{
        //    using (WebClient webclient = new WebClient())
        //    {
        //        if (!File.Exists(ControlPanelPath))
        //        {
        //            if (ControlPanelURL(null) != null)
        //            {
        //                Reporter.Report("Getting control panel art for LBGame " + Title + "...");

        //                if (webclient.DownloadFileFromDB(ControlPanelURL(null), ControlPanelPath))
        //                {
        //                    Reporter.ReportInline("success!");
        //                    OnPropertyChanged("ControlPanelPath");
        //                }
        //                else
        //                {
        //                    Reporter.ReportInline("dammit!");
        //                    return -1;
        //                }
        //            }

        //            else
        //            {
        //                Reporter.Report("No control panel art URL exists.");
        //            }
        //        }

        //        else
        //        {
        //            Reporter.Report("File already exists.");
        //        }
        //    }
        //    return 0;
        //}

        [Conditional("DEBUG")]
        public void SetLBReleasePlatform()
        {
            foreach(LBRelease lbRelease in LBReleases)
            {
                lbRelease.LBPlatform = LBPlatform;
            }
        }

        public void CreateReleases()
        {
            List<LBImage> lbImages = LBImages.Where(x => x.LBRelease_ID == null).ToList();

            for (int i = 0; i < lbImages.Count; i++)
            {
                LBRelease lbRelease = LBReleases.FirstOrDefault(x => x.Region == lbImages[i].Region);

                if (lbRelease == null)
                {
                    lbRelease = new LBRelease();
                    LBReleases.Add(lbRelease);
                    lbRelease.Region = lbImages[i].Region;
                    lbRelease.LBPlatform = LBPlatform;
                }

                lbRelease.LBImages.Add(lbImages[i]);
            }
        }
    }
}

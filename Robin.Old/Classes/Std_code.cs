//using System;
//using System.Collections.Generic;
//using System.ComponentModel;


//namespace Robin
//{
//    [Serializable]

//    public static class Good
//    {
//        public static List<RegionCode> Regions { get; set; }
//        public static List<GoodCode> Codes { get; set; }

//        static Good()
//        {
//            Regions = new List<RegionCode>();

//            Regions.Add(new RegionCode("(1)", "Japan & Korea", "1"));
//            Regions.Add(new RegionCode("(4)", "USA  & Brazil NTSC", "4"));
//            Regions.Add(new RegionCode("(A)", "Australia", "A"));
//            Regions.Add(new RegionCode("(J)", "Japan", "J"));
//            Regions.Add(new RegionCode("(B)", "Non USA (Genesis)", "B"));
//            Regions.Add(new RegionCode("(K)", "Korea", "K"));
//            Regions.Add(new RegionCode("(C)", "China", "C"));
//            Regions.Add(new RegionCode("(NL)", "Netherlands", "NL"));
//            Regions.Add(new RegionCode("(E)", "Europe", "E"));
//            Regions.Add(new RegionCode("(PD)", "Public Domain", "PD"));
//            Regions.Add(new RegionCode("(F)", "France", "F"));
//            Regions.Add(new RegionCode("(S)", "Spain", "S"));
//            Regions.Add(new RegionCode("(F)", "World (Genesis)", "F"));
//            Regions.Add(new RegionCode("(FC)", "French Canadian", "FC"));
//            Regions.Add(new RegionCode("(SW)", "Sweden", "SW"));
//            Regions.Add(new RegionCode("(FN)", "Finland", "FN"));
//            Regions.Add(new RegionCode("(U)", "USA", "U"));
//            Regions.Add(new RegionCode("(G)", "Germany", "G"));
//            Regions.Add(new RegionCode("(UK)", "England", "UK"));
//            Regions.Add(new RegionCode("(GR)", "Greece", "GR"));
//            Regions.Add(new RegionCode("(Unk)", "Unknown", "Unk"));
//            Regions.Add(new RegionCode("(HK)", "Hong Kong", "HK"));
//            Regions.Add(new RegionCode("(I)", "Italy", "I"));
//            Regions.Add(new RegionCode("(H)", "Holland", "H"));
//            Regions.Add(new RegionCode("(Unl)", "Unlicensed", "Unl"));

//            Codes.Add(new GoodCode("[a]", "Alternate", "a"));
//            Codes.Add(new GoodCode("[p]", "Pirate", "p"));
//            Codes.Add(new GoodCode("[b]", "BadDump", "b"));
//            Codes.Add(new GoodCode("[t]", "Trained", "t"));
//            Codes.Add(new GoodCode("[f]", "Fixed", "f"));
//            Codes.Add(new GoodCode("[T]", "Translation", "T"));
//            Codes.Add(new GoodCode("[h]", "Hack", "h"));
//            Codes.Add(new GoodCode("(-)", "UnknownYear", "uy"));
//            Codes.Add(new GoodCode("[o]", "Overdump", "o"));
//            Codes.Add(new GoodCode("[!]", "VerifiedGoodDump", "g"));
//            Codes.Add(new GoodCode("(M#)", "Multilanguage", "M"));
//            Codes.Add(new GoodCode("(###)", "Checksum", "cs"));
//            Codes.Add(new GoodCode("(??k)", "ROMSize", "rs"));
//            Codes.Add(new GoodCode("ZZZ_", "Unclassified", "zzz"));
//            Codes.Add(new GoodCode("(Unl)", "Unlicensed", "unl"));
//        }

//        public class RegionCode
//        {
//            public string Region { get; set; }
//            public string code { get; set; }
//            public string Goodcode { get; set; }

//            public RegionCode() { }

//            public RegionCode(string good_code, string region, string cod)
//            {
//                Region = region;
//                code = code;
//                Goodcode = good_code;
//            }

//            public class StandardCodes
//        {
//            public bool a { get; set; }
//            public bool p { get; set; }
//            public bool b { get; set; }
//            public bool t { get; set; }
//            public bool f { get; set; }
//            public bool T { get; set; }
//            public bool h { get; set; }
//            public bool uy { get; set; }
//            public bool o { get; set; }
//            public bool g { get; set; }
//            public bool unl { get; set; }
//            public bool zzz { get; set; }

//            public StandardCodes ()
//            {
//                a = false;
//                p = false;
//                b = false;
//                t = false;
//                f = false;
//                T = false;
//                h = false;
//                uy = false;
//                o = false;
//                g = false;
//                unl = false;
//                zzz = false;
//            }     
//            }

//            //public event PropertyChangedEventHandler PropertyChanged;

//            //protected void OnPropertyChanged(string name)
//            //{
//            //    PropertyChangedEventHandler handler = PropertyChanged;
//            //    if (handler != null)
//            //    {
//            //        handler(this, new PropertyChangedEventArgs(name));
//            //    }
//            //}
//        }

//        public class GoodCode
//        {
//            public string Description { get; set; }
//            public string Code { get; set; }
//            public string Goodcode { get; set; }

//            public GoodCode() { }

//            public GoodCode(string good_code, string description, string code)
//            {
//                Description = description;
//                Code = code;
//                Goodcode = good_code;
//            }
//        }
//    }

//    public static class DatoMaticCodes
//    {

//        public static List<string> Regions { get; set; }
//        public static List<string> Languages { get; set; }
//        public static List<string> Specials { get; set; }
//        public static List<string> VideoFormats { get; set; }

//        static DatoMaticCodes()
//        {
//            Regions = new List<string>();
//            Languages = new List<string>();
//            Specials = new List<string>();
//            VideoFormats = new List<string>();

//            Regions.Add("Asia");
//            Regions.Add("Australia");
//            Regions.Add("Brazil");
//            Regions.Add("Canada");
//            Regions.Add("China");
//            Regions.Add("Europe");
//            Regions.Add("France");
//            Regions.Add("Germany");
//            Regions.Add("Italy");
//            Regions.Add("Japan");
//            Regions.Add("Korea");
//            Regions.Add("Russia");
//            Regions.Add("Spain");
//            Regions.Add("Sweden");
//            Regions.Add("USA");
//            Regions.Add("World");

//            Languages.Add("En");
//            Languages.Add("Fr");
//            Languages.Add("De");
//            Languages.Add("Es");
//            Languages.Add("It");
//            Languages.Add("Nl");
//            Languages.Add("Sv");
//            Languages.Add("Da");
//            Languages.Add("Fi");

//            Specials.Add("Cart Present");
//            Specials.Add("GameCube Edition");
//            Specials.Add("Gold Edition");
//            Specials.Add("No Cart Present");
//            Specials.Add("RAM");
//            Specials.Add("Sample");
//            Specials.Add("Unknown");
//            Specials.Add("Unl");
//            Specials.Add("NES Test");
//            Specials.Add("NTSC Demo");
//            Specials.Add("NTDEC, Gluk Video");
//            Specials.Add("Aladdin Compact Cartridge");
//            Specials.Add("ArchiMENdes Hen");
//            Specials.Add("Bulletproof");
//            Specials.Add("Genteiban!");
//            Specials.Add("Gluk Video");
//            Specials.Add("Hacker inc.");
//            Specials.Add("Hacker");
//            Specials.Add("Hwang Shinwei");
//            Specials.Add("Namco");
//            Specials.Add("Plug-Thru Cart");
//            Specials.Add("RCM Group");
//            Specials.Add("Sachen");
//            Specials.Add("Sunsoft");
//            Specials.Add("Taito");
//            Specials.Add("Tengen");
//            Specials.Add("UBI Soft");
//            Specials.Add("Victor");
//            Specials.Add("Yi");

//            VideoFormats.Add("NTSC");
//            VideoFormats.Add("PAL");
//        }
//    }

//}



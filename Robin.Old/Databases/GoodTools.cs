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
using System.IO;

namespace Robin.Core
{
    class GoodTools
    {
        public void ParseGood(string filename)
        {
            Release ROM = new Release();

            // Load static list of region codes and good codes
            //Regions Regions = new Regions();


            // Pull name file and folder from ROM file name
            var name = Path.GetFileNameWithoutExtension(filename);
            //file = Path.GetFileName(file_name);
            //folder = Path.GetDirectoryName(file_name);

            // Run over list of good codes and search name for each
            foreach (Good.GoodCode gc in Good.Codes)
            {
                // If good code is found in name
                if (name.IndexOf(gc.Goodcode) != -1)
                {
                    string code_letter = gc.Code;

                    try
                    {
                        // Set the standard code that was found in the current ROM
                        typeof(Release).GetProperty(code_letter).SetValue(ROM, true);
                    }

                    catch (NullReferenceException) { /*ignore*/}
                }
            }

            // Run over list of region codes and search name for each
            foreach (Good.RegionCode rc in Good.Regions)
            {
                // If region code is found in name
                if (name.IndexOf(rc.Goodcode) != -1)
                {
                    //string code_letter = rc.code;
                    break;
                }
            }

            // Extract title from file name
            int name_length = name.IndexOf("(") - 1;
            if (name_length == -1)
            {
                name_length = name.IndexOf("[") - 1;
            }

            // Move "the" to front of name
            if (name_length > 3)
            {
                name = name.Substring(0, name_length);
            }

            if (name.Substring(name.Length - 3, 3).ToLower() == "the")
            {
                name = "The " + name.Substring(0, name.Length - 5);
            }

            //crc Crc = crc.Load(filename);
            //Title = name;
            ////this.File = file;
            ////Region = region;
            //CRC32 = Crc.CRC32;
        }

        static class Good
        {
            public static List<RegionCode> Regions { get; }
            public static List<GoodCode> Codes { get; }

            static Good()
            {
                Regions = new List<RegionCode>();

                Regions.Add(new RegionCode("(1)", "Japan & Korea", "1"));
                Regions.Add(new RegionCode("(4)", "USA  & Brazil NTSC", "4"));
                Regions.Add(new RegionCode("(A)", "Australia", "A"));
                Regions.Add(new RegionCode("(J)", "Japan", "J"));
                Regions.Add(new RegionCode("(B)", "Non USA (Genesis)", "B"));
                Regions.Add(new RegionCode("(K)", "Korea", "K"));
                Regions.Add(new RegionCode("(C)", "China", "C"));
                Regions.Add(new RegionCode("(NL)", "Netherlands", "NL"));
                Regions.Add(new RegionCode("(E)", "Europe", "E"));
                Regions.Add(new RegionCode("(PD)", "Public Domain", "PD"));
                Regions.Add(new RegionCode("(F)", "France", "F"));
                Regions.Add(new RegionCode("(S)", "Spain", "S"));
                Regions.Add(new RegionCode("(F)", "World (Genesis)", "F"));
                Regions.Add(new RegionCode("(FC)", "French Canadian", "FC"));
                Regions.Add(new RegionCode("(SW)", "Sweden", "SW"));
                Regions.Add(new RegionCode("(FN)", "Finland", "FN"));
                Regions.Add(new RegionCode("(U)", "USA", "U"));
                Regions.Add(new RegionCode("(G)", "Germany", "G"));
                Regions.Add(new RegionCode("(UK)", "England", "UK"));
                Regions.Add(new RegionCode("(GR)", "Greece", "GR"));
                Regions.Add(new RegionCode("(Unk)", "Unknown", "Unk"));
                Regions.Add(new RegionCode("(HK)", "Hong Kong", "HK"));
                Regions.Add(new RegionCode("(I)", "Italy", "I"));
                Regions.Add(new RegionCode("(H)", "Holland", "H"));
                Regions.Add(new RegionCode("(Unl)", "Unlicensed", "Unl"));

                Codes.Add(new GoodCode("[a]", "Alternate", "a"));
                Codes.Add(new GoodCode("[p]", "Pirate", "p"));
                Codes.Add(new GoodCode("[b]", "BadDump", "b"));
                Codes.Add(new GoodCode("[t]", "Trained", "t"));
                Codes.Add(new GoodCode("[f]", "Fixed", "f"));
                Codes.Add(new GoodCode("[T]", "Translation", "T"));
                Codes.Add(new GoodCode("[h]", "Hack", "h"));
                Codes.Add(new GoodCode("(-)", "UnknownYear", "uy"));
                Codes.Add(new GoodCode("[o]", "Overdump", "o"));
                Codes.Add(new GoodCode("[!]", "VerifiedGoodDump", "g"));
                Codes.Add(new GoodCode("(M#)", "Multilanguage", "M"));
                Codes.Add(new GoodCode("(###)", "Checksum", "cs"));
                Codes.Add(new GoodCode("(??k)", "ROMSize", "rs"));
                Codes.Add(new GoodCode("ZZZ_", "Unclassified", "zzz"));
                Codes.Add(new GoodCode("(Unl)", "Unlicensed", "unl"));
            }

            public class RegionCode
            {
                public string Region { get; }
                public string code { get; }
                public string Goodcode { get; }

                //public RegionCode() { }

                public RegionCode(string good_code, string region, string cod)
                {
                    Region = region;
                    code = cod;
                    Goodcode = good_code;
                }

                //public class StandardCodes
                //{
                //	public bool a { get; set; }
                //	public bool p { get; set; }
                //	public bool b { get; set; }
                //	public bool t { get; set; }
                //	public bool f { get; set; }
                //	public bool T { get; set; }
                //	public bool h { get; set; }
                //	public bool uy { get; set; }
                //	public bool o { get; set; }
                //	public bool g { get; set; }
                //	public bool unl { get; set; }
                //	public bool zzz { get; set; }

                //	public StandardCodes()
                //	{
                //		a = false;
                //		p = false;
                //		b = false;
                //		t = false;
                //		f = false;
                //		T = false;
                //		h = false;
                //		uy = false;
                //		o = false;
                //		g = false;
                //		unl = false;
                //		zzz = false;
                //	}
                //}
            }

            public class GoodCode
            {
                public string Description { get; set; }
                public string Code { get; set; }
                public string Goodcode { get; set; }

                //public GoodCode() { }

                public GoodCode(string good_code, string description, string code)
                {
                    Description = description;
                    Code = code;
                    Goodcode = good_code;
                }
            }
        }


    }
}

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

#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Robin
{
	class DebugStuff
	{
		public async static Task MainWindowBonusAsync()
		{
			Reporter.Report("BONUS!");
			await Task.Run(() =>
			{
				Mame.Database mame = new Mame.Database();
				mame.CacheDataBase3();
			});

			Reporter.Report("Finished");
		}

		static void ForEachSpeedTest()
		{
			Stopwatch watch = Stopwatch.StartNew();
			HashSet<Release> hs = new HashSet<Release>(R.Data.Releases);
			foreach (Release release in hs)
			{
				string s = release.Title;
				long i = release.Id;
			}
			Debug.WriteLine($"Straight from DbSet: {watch.ElapsedMilliseconds} elapsed.");

		}


		static void MetaTest()
		{
			List<Release> releases = R.Data.Releases.ToList();
			Release release = releases.Find(x => x.Id == 1000);
			release.Title = "xkcd";
			Datomatic dat = new Datomatic();
			dat.ReportUpdates(true);
		}

		static void FindSpeedTest()
		{
			Stopwatch watch = Stopwatch.StartNew();
			Dictionary<long, Release> releases = R.Data.Releases.ToDictionary(x => x.Id, x => x);
			List<long> list = R.Data.Releases.Select(x => x.Id).ToList();
			int yes = 0;
			int no = 0;

			Release release;
			foreach (long id in list)
			{
				if (releases.TryGetValue(id, out release))
				{
					yes++;
				}
				else
				{
					no++;
				}
			}
			Debug.WriteLine($"{watch.ElapsedMilliseconds} elapsed. {yes} yes, {no} no.");
		}


		static void HashsetSpeedTest()
		{
			Platform NES = R.Data.Platforms.FirstOrDefault(x => x.Id == 22);

			Stopwatch Watch1 = Stopwatch.StartNew();
			List<Rom> roms = roms = R.Data.Roms.Where(x => x.PlatformId == NES.Id).ToList();
			Debug.WriteLine($"List created: {Watch1.ElapsedMilliseconds}."); Watch1.Restart();

			HashSet<string> stringDing = roms.Select(x => x.Crc32) as HashSet<string>;
			Debug.WriteLine($"Hashset created: {Watch1.ElapsedMilliseconds}."); Watch1.Restart();


		}

		static void TestHash()
		{
			Platform NES = R.Data.Platforms.FirstOrDefault(x => x.Id == 22);
			foreach (Rom rom in NES.Roms.Where(x => x.FileName != null))
			{
				string Crc32a = Audit.GetHash(rom.FilePath, HashOption.Crc32, (int)NES.HeaderLength);
				string Sha1a = Audit.GetHash(rom.FilePath, HashOption.Sha1, (int)NES.HeaderLength);
				string Md5a = Audit.GetHash(rom.FilePath, HashOption.Md5, (int)NES.HeaderLength);

				string Crc32b = rom.Crc32;
				string Sha1b = rom.Sha1;
				string Md5b = rom.Md5;

				if (Crc32a == Crc32b && Md5a == Md5b && Sha1a == Sha1b)
				{
					Debug.WriteLine($"OK - {rom.Title} - {rom.Id}");
				}

				else
				{
					Crc32a = Audit.GetHash(rom.FilePath, HashOption.Crc32, 0);
					Sha1a = Audit.GetHash(rom.FilePath, HashOption.Sha1, 0);
					Md5a = Audit.GetHash(rom.FilePath, HashOption.Md5, 0);
				}

				if (Crc32a == Crc32b && Md5a == Md5b && Sha1a == Sha1b)
				{
					Debug.WriteLine($"OK - {rom.Title} - {rom.Id}");
				}

				Debug.Assert(Crc32a == Crc32b || Crc32b == null, $"Crc32 failure on {rom.Title} - {rom.Id}");
				Debug.Assert(Sha1a == Sha1b || Sha1b == null, $"Sha1 failure on {rom.Title} - {rom.Id}");
				Debug.Assert(Md5a == Md5b || Md5b == null, $"Md5 failure on {rom.Title} - {rom.Id}");
			}
		}

		//static void SetMessMachines()
		//{
		//	int i = 0;
		//	foreach (string messMachine in messMachines)
		//	{
		//		Release release = R.Data.Releases.FirstOrDefault(x => x.Rom.FileName != null && x.Rom.FileName.Split('.')[0] == messMachine);

		//		if (release != null)
		//		{
		//			release.Game.IsMess = true;
		//			Debug.WriteLine(i++);
		//		}
		//	}
		//}

		public async static Task DatabaseWindowBonusAsync()
		{
			Reporter.Report("BONUS!");
			await Task.Run(() => { });
		}

		public static void SetFactoryDatabase()
		{
			foreach (Release release in R.Data.Releases)
			{
				release.IsBeaten = false;
				release.Rating = null;
				release.PlayCount = 0;
			}

			foreach (Collection collection in R.Data.Collections)
			{
				collection.Releases.Clear();
				collection.Games.Clear();
			}
		}

		static void GetMameStatusesAsync()
		{
			Platform arcadePlatform = R.Data.Platforms.FirstOrDefault(x => x.Id == CONSTANTS.ARCADE_PlatformId);

			Stopwatch Watch = Stopwatch.StartNew();
			Stopwatch Watch1 = Stopwatch.StartNew();

			XmlReaderSettings settings = new XmlReaderSettings();

			List<string> driverStatuses = new List<string>();
			List<string> emulationStatuses = new List<string>();
			settings.DtdProcessing = DtdProcessing.Parse;

			arcadePlatform = R.Data.Platforms.FirstOrDefault(x => x.Title.Contains("Arcade"));

			Watch1.Restart();
			Reporter.Report("Getting xml file from MAME...");

			// Scan through xml file from MAME and pick out working games

			using (Process process = Mame.Database.MAMEexe(@"-lx"))
			using (XmlReader reader = XmlReader.Create(process.StandardOutput, settings))
			{
				while (reader.Read())
				{
					if (reader.Name == "machine")
					{
						XElement machineElement = XNode.ReadFrom(reader) as XElement;
						driverStatuses.Add(machineElement.SafeGetA(element1: "driver", attribute: "status"));
						emulationStatuses.Add(machineElement.SafeGetA(element1: "driver", attribute: "emulation"));
					}
				}
			}

			List<List<string>> uniqueDriverStatuses;
			List<List<string>> uniqueEmulationStatuses;

			uniqueDriverStatuses = driverStatuses.Distinct().Select(x => new List<string>(new string[] { x, driverStatuses.Count(y => y == x).ToString() })).ToList();
			uniqueEmulationStatuses = emulationStatuses.Distinct().Select(x => new List<string>(new string[] { x, emulationStatuses.Count(y => y == x).ToString() })).ToList();

			Reporter.Report("Finished: " + Watch.Elapsed.ToString(@"m\:ss"));
		}

		static void SetAdult()
		{
			foreach (Release release in R.Data.Releases)
			{
				if (adults.Contains(release.Rom.FileName))
				{
					release.IsAdult = true;
				}
			}

		}

		static string[] adults = {"3kokushi.zip",
		"47pie2.zip",
		"47pie2o.zip",
		"7jigen.zip",
		"7toitsu.zip",
		"akiss.zip",
		"apparel.zip",
		"av2mj1bb.zip",
		"av2mj2rg.zip",
		"bakatono.zip",
		"bakuhatu.zip",
		"bananadr.zip",
		"bballs.zip",
		"bigtwin.zip",
		"bijokkog.zip",
		"bijokkoy.zip",
		"blandia.zip",
		"blandiap.zip",
		"blckgalb.zip",
		"blockgal.zip",
		"bubl2000.zip",
		"cannball.zip",
		"ccasino.zip",
		"chinmoku.zip",
		"citylove.zip",
		"club90s.zip",
		"club90sa.zip",
		"cmehyou.zip",
		"crystal2.zip",
		"crystalg.zip",
		"daiyogen.zip",
		"ddenlovr.zip",
		"dokyusei.zip",
		"dokyusp.zip",
		"dondenmj.zip",
		"drgpunch.zip",
		"fantasia.zip",
		"fantsia2.zip",
		"fantsy95.zip",
		"finalbny.zip",
		"fromanc2.zip",
		"fromanc4.zip",
		"fromance.zip",
		"fromancr.zip",
		"funybubl.zip",
		"gakusai.zip",
		"gakusai2.zip",
		"gal10ren.zip",
		"galhustl.zip",
		"galkaika.zip",
		"galkoku.zip",
		"galpani2.zip",
		"galpani4.zip",
		"galpanib.zip",
		"galpanic.zip",
		"galpanis.zip",
		"galpans2.zip",
		"galspnbl.zip",
		"gfire2.zip",
		"gionbana.zip",
		"glass.zip",
		"grndtour.zip",
		"gumbo.zip",
		"gundeala.zip",
		"gundealr.zip",
		"gundealt.zip",
		"gundl94.zip",
		"hanamai.zip",
		"hanamomo.zip",
		"hanaoji.zip",
		"hexa.zip",
		"hgkairak.zip",
		"hnayayoi.zip",
		"hnfubuki.zip",
		"hnkochou.zip",
		"hnoridur.zip",
		"hotgmck.zip",
		"hotgmck3.zip",
		"hotpinbl.zip",
		"hourouki.zip",
		"housemn2.zip",
		"housemnq.zip",
		"hyhoo.zip",
		"hyhoo2.zip",
		"hyouban.zip",
		"hypreac2.zip",
		"hypreact.zip",
		"idhimitu.zip",
		"idolmj.zip",
		"iemoto.zip",
		"imekura.zip",
		"janjans1.zip",
		"janshin.zip",
		"jantouki.zip",
		"jjparad2.zip",
		"jjparads.zip",
		"jogakuen.zip",
		"kaguya.zip",
		"kanatuen.zip",
		"kirarast.zip",
		"koikois2.zip",
		"koinomp.zip",
		"korinai.zip",
		"ladykill.zip",
		"lastfero.zip",
		"lemnangl.zip",
		"lvgirl94.zip",
		"maddonna.zip",
		"maddonnb.zip",
		"magix.zip",
		"mahretsu.zip",
		"maiko.zip",
		"majs101b.zip",
		"marukin.zip",
		"maya.zip",
		"mayumi.zip",
		"mcnpshnt.zip",
		"mcontest.zip",
		"mfunclub.zip",
		"mgakuen.zip",
		"mgakuen2.zip",
		"mgmen89.zip",
		"mhgaiden.zip",
		"missmw96.zip",
		"missw96.zip",
		"mj4simai.zip",
		"mjanbari.zip",
		"mjangels.zip",
		"mjcamera.zip",
		"mjclinic.zip",
		"mjderngr.zip",
		"mjdialq2.zip",
		"mjdiplob.zip",
		"mjegolf.zip",
		"mjelct3.zip",
		"mjelct3a.zip",
		"mjelctrn.zip",
		"mjfocus.zip",
		"mjfocusm.zip",
		"mjfriday.zip",
		"mjgottsu.zip",
		"mjhokite.zip",
		"mjikaga.zip",
		"mjkinjas.zip",
		"mjkjidai.zip",
		"mjkoiura.zip",
		"mjkojink.zip",
		"mjlaman.zip",
		"mjlstory.zip",
		"mjnanpaa.zip",
		"mjnanpas.zip",
		"mjnanpau.zip",
		"mjnatsu.zip",
		"mjnquest.zip",
		"mjsikakb.zip",
		"mjsikaku.zip",
		"mjsister.zip",
		"mjuraden.zip",
		"mjyougo.zip",
		"mjyuugi.zip",
		"mjyuugia.zip",
		"mkeibaou.zip",
		"mladyhtr.zip",
		"mmcamera.zip",
		"mmehyou.zip",
		"moegonta.zip",
		"mrokumei.zip",
		"mscoutm.zip",
		"msjiken.zip",
		"missw96.zip",
		"natsuiro.zip",
		"nekkyoku.zip",
		"neruton.zip",
		"newfant.zip",
		"ngpgal.zip",
		"niyanpai.zip",
		"nmsengen.zip",
		"ntopstar.zip",
		"ojanko2.zip",
		"ojankoc.zip",
		"ojankohs.zip",
		"ojankoy.zip",
		"ojousan.zip",
		"orangec.zip",
		"otatidai.zip",
		"otonano.zip",
		"pachiten.zip",
		"pairlove.zip",
		"pairs.zip",
		"pairsa.zip",
		"pairsten.zip",
		"paradise.zip",
		"pastelgl.zip",
		"patimono.zip",
		"pcktgal.zip",
		"pcktgal2.zip",
		"pcktgalb.zip",
		"peekaboo.zip",
		"peepshow.zip",
		"perestrf.zip",
		"perestro.zip",
		"pipibibi.zip",
		"pipibibs.zip",
		"pkladiel.zip",
		"pkladies.zip",
		"plgirls.zip",
		"plgirls2.zip",
		"ponchin.zip",
		"ponchina.zip",
		"primella.zip",
		"psailor1.zip",
		"psailor2.zip",
		"pstadium.zip",
		"pushman.zip",
		"puzznic.zip",
		"qmhayaku.zip",
		"quiz18k.zip",
		"realbrk.zip",
		"renaiclb.zip",
		"rmhaihai.zip",
		"rmhaihib.zip",
		"rmhaijin.zip",
		"rmhaisei.zip",
		"rongrong.zip",
		"ryouran.zip",
		"sadari.zip",
		"sailorwr.zip",
		"sailorws.zip",
		"scandal.zip",
		"scandalm.zip",
		"secolove.zip",
		"seiha.zip",
		"seiham.zip",
		"sexyparo.zip",
		"sichuan2.zip",
		"sos.zip",
		"spbactn.zip",
		"spbactnj.zip",
		"splash.zip",
		"srmp2.zip",
		"srmp3.zip",
		"srmp4.zip",
		"srmp4o.zip",
		"srmp7.zip",
		"sshangha.zip",
		"sshanghb.zip",
		"stoffy.zip",
		"supertr2.zip",
		"supertr3.zip",
		"suzume.zip",
		"sxyreact.zip",
		"teljan.zip",
		"telmahjn.zip",
		"tgtball.zip",
		"themj.zip",
		"tmmjprd.zip",
		"tokimbsj.zip",
		"tokyogal.zip",
		"tontonb.zip",
		"triplew1.zip",
		"triplew2.zip",
		"uchuuai.zip",
		"ultramhm.zip",
		"untoucha.zip",
		"usg185.zip",
		"usg252.zip",
		"usg32.zip",
		"vanilla.zip",
		"vipclub.zip",
		"vitaminc.zip",
		"whoopee.zip",
		"wiggie.zip",
		"yarunara.zip",
		"zerozone.zip" };


		static string[] messMachines = new string[]
			{
				"1292apvs",
"1392apvs",
"ie15",
"k286i",
"386i",
"3b1",
"3do",
"3do_pal",
"3do_m2",
"4004clk",
"ficpio2",
"sgi_ip6",
"d6809",
"68ksbc",
"elwro800",
"mtc9016",
"hp9835a",
"hp9835b",
"hp9845a",
"hp9845b",
"hp9845c",
"hp9845s",
"hp9845t",
"a5120",
"a5130",
"a7150",
"pegasus",
"pegasusm",
"abc1600",
"abc80",
"abc800c",
"abc800m",
"abc802",
"abc806",
"electron",
"bbcb_us",
"acrnsys1",
"adam",
"uvc",
"advision",
"advsnha",
"agat7",
"agat9",
"ai1000",
"aim65",
"aim65_40",
"al520ex",
"alice",
"alice32",
"alice90",
"alnattck",
"alnchase",
"apexc",
"asma2k",
"asmapro",
"alphatro",
"mits680b",
"al8800bt",
"alto2",
"altos5",
"am64",
"ac1",
"ac1_32",
"ac1scch",
"amaztron",
"amico2k",
"a1000n",
"a1000",
"a1200n",
"a1200",
"a2000n",
"a2000",
"a3000n",
"a3000",
"a400030n",
"a400030",
"a4000n",
"a4000",
"a4000tn",
"a4000t",
"a500n",
"a500",
"a500pn",
"a500p",
"a600n",
"a600",
"cd32n",
"cd32",
"cpc464",
"cpc464p",
"cpc6128",
"cpc6128f",
"cpc6128s",
"cpc6128p",
"cpc664",
"gx4000",
"pc20",
"pc2086",
"pc2386",
"pc3086",
"ppc512",
"ppc640",
"amust",
"apc",
"apfimag",
"apfm1000",
"apogee",
"apollo80",
"dn3000",
"dn3000_19i",
"dn3500",
"dn3500_19i",
"dn5500",
"dn5500_19i",
"dsp3000",
"dsp3500",
"dsp5500",
"apple3",
"apple2c",
"apple2c3",
"apple2c4",
"apple2c0",
"apple2cp",
"apple2e",
"apple2ee",
"apple2eeuk",
"apple2ep",
"apple2euk",
"apple1",
"apple2gsr0p2",
"apple2gsr0p",
"apple2gsr0",
"apple2gsr1",
"apple2gsr3lp",
"apple2gsr3p",
"apple2gs",
"apple2",
"apple2jp",
"apple2p",
"applix",
"f1",
"f10",
"f1e",
"f2",
"aprfte",
"aprpand",
"ftsserv",
"aplanst",
"aplannb",
"aplsbon",
"aplscar",
"apricot",
"fp",
"xb42663",
"qi600",
"qi900",
"apvxft",
"apxena1",
"apxenp2",
"apxeni",
"apxenls3",
"apxlsam",
"xb42639",
"xb42664",
"xb42639a",
"xb42664a",
"apricotxi",
"aquarius",
"bndarc",
"arcadia",
"a3010",
"a3020",
"a310",
"a7000",
"a7000p",
"argo",
"bbcmarm",
"asr10",
"asrx",
"asst128",
"astro",
"astrocmd",
"a1200xl",
"a130xe",
"a2600",
"a2600p",
"a400",
"a400pal",
"a5200",
"a600xl",
"a65xe",
"a65xea",
"a7800",
"a7800p",
"a800",
"a800pal",
"a800xe",
"a800xl",
"a800xlp",
"xegs",
"atm",
"atmtb2",
"atom",
"atombb",
"atomeb",
"attachef",
"attache",
"amu880",
"ax150",
"ax170",
"ax20",
"ax350",
"ax370",
"b128",
"b128hp",
"b16",
"b256",
"b256hp",
"ngenb38",
"b500",
"babbage",
"astrocdw",
"astrocdl",
"astrocde",
"banctec",
"bankshot",
"b2m",
"b2mrom",
"bmjr",
"bbcm",
"bbcm512",
"bbcmaiv",
"bbcmc",
"bbcmc_ar",
"bbcmet",
"bbcmt",
"bbca",
"bbcb",
"bbcb_de",
"bbcbp128",
"bbcbp",
"bcs3",
"bcs3a",
"bcs3b",
"bcs3c",
"bebox2",
"bebox",
"besta88",
"bestzx",
"beta",
"a5105",
"bigboard",
"bigbord2",
"bigtrak",
"binbug",
"bitgrpha",
"bitgrphb",
"bk0010",
"bk0010fd",
"bk001001",
"bk0011m",
"bk8t",
"blitzs",
"bob85",
"bw12",
"bw14",
"bw2",
"borisdpl",
"brailab4",
"braiplus",
"bs9009cx",
"br8641",
"vbrc",
"bridgec3",
"bbcbc",
"bruc100",
"wmbullet",
"wmbulletf",
"busicom",
"bw230",
"bx256hp",
"byte",
"c10",
"c80",
"c8002",
"cat",
"tcaveman",
"cb308",
"cbm3008",
"cbm3016",
"cbm3032",
"cbm3032b",
"cbm4016",
"cbm4032",
"cbm4032f",
"cbm4032b",
"cbm610",
"cbm620",
"cbm620_hu",
"cbm710",
"cbm720",
"cbm720_de",
"cbm720_se",
"cbm730",
"cbm8032",
"cbm8032_de",
"cbm8032_se",
"cbm8096",
"cbm8296",
"cbm8296ed",
"cbm8296d",
"cbm8296d_de",
"cbm8296dgv_de",
"cbm8296gd",
"ccs2422",
"ccs2810",
"ccs300",
"cd2650",
"cdimono1",
"cdc721",
"cdtvn",
"cdtv",
"cdx",
"cf1200",
"cf2000",
"cf2700",
"cf2700g",
"cf3000",
"cf3300",
"cfx9850",
"cgc7900",
"c1pmf",
"c1p",
"csc",
"channelf",
"channlf2",
"chaos",
"cc10",
"ccmk1",
"ccmk2",
"ssystem3",
"chesstrv",
"chessmst",
"czz50",
"cip01",
"cip03",
"cm1800",
"cm32l",
"cobrasp",
"cobra80",
"codata",
"cnsector",
"coleco",
"colecop",
"coco",
"cocoe",
"coco2",
"coco2b",
"coco3",
"coco3h",
"coco3dw1",
"coco3p",
"cgenie",
"cgenienz",
"c116",
"c128_de",
"c128",
"c128p",
"c128_se",
"c128cr",
"c128d",
"c128dp",
"c128d81",
"c128dcr_de",
"c128dcr",
"c128dcrp",
"c128dcr_se",
"c16_hu",
"c16",
"c16p",
"c232",
"c264",
"c386sx16",
"c64_jp",
"c64",
"c64p",
"c64_se",
"c64dtv",
"c64gs",
"c64c",
"c64cp",
"c64c_es",
"c64c_se",
"c64dx",
"c64g",
"c65",
"c900",
"compc1",
"pc10iii",
"v364",
"comp4",
"cc40",
"compis",
"compis2",
"jtc",
"jtces88",
"jtces23",
"jtces40",
"compclr2",
"comquest",
"comx35n",
"comx35p",
"concept",
"c2717",
"c2717pmd",
"cortex",
"microkit",
"vp111",
"vip",
"tccombat",
"cosmicos",
"cp1",
"cp400",
"cpc330k",
"cpc88",
"cpu09",
"ivg09",
"craft",
"bcclimbr",
"gckong",
"crvision",
"crvisioj",
"crvisio2",
"cx3000tc",
"cx5f1",
"cx5f",
"cx5m",
"cx5m128",
"cx5m2",
"cx7m",
"cx7m128",
"cybikov1",
"cybikov2",
"cybikoxt",
"czk80",
"d110",
"dagz80",
"dai",
"mbdtower",
"dgone",
"ds2",
"dator3k",
"dauphin",
"dct11em",
"dmv",
"dectalk",
"dendy",
"dg680",
"didakt90",
"alfa",
"dgama87",
"dgama88",
"dgama89",
"didaktk",
"didakm91",
"didakm92",
"didakm93",
"gdigdug",
"digel804",
"dim68k",
"dina",
"beehive",
"dms5000",
"dms86",
"drpcjr",
"bdoramon",
"cdkong",
"tecnbras",
"fdpc200",
"tadpc200",
"tadpc20a",
"dpc200e",
"dps1",
"edracula",
"dragon200",
"dragon200e",
"dragon32",
"dragon64",
"d64plus",
"dgnalpha",
"dgnbeta",
"d6800",
"dm500",
"dm5620",
"dm7000",
"dceu",
"dcjp",
"dc",
"dw225",
"drwrt450",
"drwrt100",
"drwrt200",
"drwrt400",
"bambball",
"dsb46",
"dual68",
"dvk_ksm",
"dx64",
"dynavisn",
"eacc",
"ec1840",
"ec1841",
"ec1842",
"ec1845",
"ec1847",
"ec1849",
"ec65",
"ec65k",
"edu64",
"einst256",
"einstein",
"einstei2",
"ekusera",
"ebball",
"ebball2",
"ebball3",
"elecbowl",
"elecdet",
"efball",
"elekscmp",
"elektor",
"mc1502",
"mc1702",
"pk88",
"elf2",
"enmirage",
"ep128",
"ep64",
"ep804",
"eps",
"eps16p",
"ehx20",
"ehx20e",
"erik",
"wales210",
"es210_es",
"jonos",
"esq1",
"esqm",
"eti660",
"europc",
"excali64",
"osbexec",
"exeltel",
"exl100",
"expert10",
"expert11",
"expert13",
"expert20",
"expert3i",
"expert3t",
"expertac",
"expertdp",
"expertdx",
"expertpl",
"exp85",
"falcon30",
"falcon40",
"famicom",
"fds",
"famitwin",
"fb01",
"fc100",
"gsfc200",
"gsfc80u",
"fellow",
"fk1",
"fm11",
"fm16beta",
"fm7",
"fm77av",
"fm7740sx",
"fm8",
"fmnew7",
"fmtowns",
"fmtownsa",
"carmarty",
"fmtownsftv",
"fmtownshr",
"fmtownsmx",
"fmtownssj",
"fmtownsux",
"fmtmarty",
"fmtmarty2",
"fmx",
"f1392",
"fforce2",
"fp1100",
"fp200",
"fp6000",
"fpc500",
"fpc900",
"ace100",
"bfriskyt",
"fs1300",
"fs4000a",
"fs4000",
"fs4500",
"fs4600",
"fs4700",
"fs5000",
"fs5500f1",
"fs5500f2",
"fsa1",
"fsa1a",
"fsa1f",
"fsa1fm",
"fsa1fx",
"fsa1gt",
"fsa1mk2",
"fsa1st",
"fsa1wsx",
"fsa1wx",
"fsa1wxa",
"fstm1",
"ft68m",
"fnvision",
"grfd2301",
"galaxy",
"galaxyp",
"cgalaxn",
"egalaxn2",
"galaxy2",
"galeb",
"sms1kr",
"sms1krfm",
"smskr",
"gamate",
"gameboy",
"gba",
"gbcolor",
"gbpocket",
"gamegear",
"gamegeaj",
"gmaster",
"gamepock",
"gamecom",
"gameking",
"genesis",
"32x",
"geneve",
"gbs5505x",
"iq128_fr",
"iq128",
"iqtv512",
"gj4000",
"gj5000",
"gjmovie",
"gjrstar2",
"gjrstar3",
"gjrstar",
"gl2000",
"gl2000c",
"gl2000p",
"gl3000s",
"gl4000",
"gl4004",
"gl5000",
"gl5005x",
"gl6000sl",
"gl6600cx",
"gl7007sl",
"gl8008cx",
"glcolor",
"glmcolor",
"glmmc",
"gln",
"glpn",
"glscolor",
"gmtt",
"gwnf",
"gfc1080",
"gfc1080a",
"gimix",
"gizmondo",
"gchinatv",
"gp2x",
"gp32",
"rx78",
"hanihac",
"harriet",
"hb10",
"hb101p",
"hb10p",
"hb201",
"hb201p",
"hb20p",
"hb501p",
"hb55",
"hb55d",
"hb55p",
"hb701fd",
"hb75d",
"hb75p",
"hotbit11",
"hotbit12",
"hotbi13b",
"hotbi13p",
"hotbit20",
"hbf1",
"hbf12",
"hbf1xd",
"hbf1xdj",
"hbf1xdm2",
"hbf1xv",
"hbf5",
"hbf500",
"hbf500f",
"hbf500p",
"hbf700d",
"hbf700f",
"hbf700p",
"hbf700s",
"hbf900",
"hbf900a",
"hbf9p",
"hbf9pr",
"hbf9s",
"hbf9sp",
"hbg900ap",
"hbg900p",
"hc128",
"hc2000",
"hc5",
"hc6",
"hc7",
"jvchc7gb",
"hc85",
"hc88",
"hc90",
"victhc90",
"hc91",
"victhc95",
"victhc95a",
"kc85_2",
"h2hbaseb",
"h2hfootb",
"et3400",
"h19",
"h8",
"h89",
"hector1",
"hec2hr",
"hec2hrp",
"hec2hrx",
"hec2mdhrx",
"hec2mx40",
"hec2mx80",
"ghalien",
"hprotr8a",
"hprot2r6",
"hprot1",
"hisaturn",
"dcdev",
"hmg1292",
"hmg1392",
"hmg2650",
"homez80",
"homelab2",
"homelab3",
"homelab4",
"nshrz",
"hp165ka0",
"hp16500b",
"hp1650b",
"hp1651b",
"hp9816",
"hp38g",
"hp39g",
"hp48g",
"hp48gp",
"hp48gx",
"hp48s",
"hp48sx",
"hp49g",
"hp49gp",
"hp9k310",
"hp9k320",
"hp9k330",
"hp9k340",
"hp9k370",
"hp9k380",
"hp9k382",
"hr16",
"hr16b",
"ht1080z",
"ht1080z2",
"ht108064",
"hxhdci2k",
"hunter2",
"hx10",
"hx10d",
"hx10dp",
"hx10e",
"hx10f",
"hx10s",
"hx10sa",
"hx20",
"hx20i",
"hx21",
"hx21i",
"hx22",
"hx22i",
"hx23",
"hx23f",
"hx23i",
"hx33",
"hx34",
"hx34i",
"hs",
"ibm5550",
"ibm6580",
"ibm5140",
"ibm5150",
"ibm5155",
"ibmpcjr",
"ibmpcjx",
"ibm5170",
"ibm5170a",
"ibm5162",
"ibmps1es",
"i8530h31",
"i8535043",
"i8550021",
"i8550061",
"i8555081",
"i8580071",
"i8580111",
"i8530286",
"ibm5160",
"ics8080",
"if800",
"impuls03",
"indiana",
"ip244415",
"ip224613",
"ip225015",
"ingtelma",
"imds",
"imds2",
"intmpt03",
"intv",
"intv2",
"intvkbd",
"intvecs",
"intvoice",
"interact",
"ixl2000",
"vc4000",
"intervsn",
"invspace",
"vinvader",
"inves",
"ipb",
"ipc",
"ipds",
"iqunlimz",
"dpc100",
"dpc180",
"dpc200",
"iq151",
"cpc300",
"cpc300e",
"sgi_ip2",
"ip204415",
"irisha",
"isbc286",
"isbc2861",
"isbc86",
"iskr1030m",
"iskr1031",
"iskr3104",
"itmcmtp3",
"itttelma",
"itt3030",
"ivelultr",
"sarpc_j233",
"gjackpot",
"jaguar",
"jaguarcd",
"jet",
"jade",
"jopac",
"jr100",
"jr100u",
"jr200",
"jr200u",
"juicebox",
"junior",
"jupace",
"jupiter2",
"jupiter3",
"k1003",
"k8915",
"stratos",
"dcprt",
"kay1024",
"kaypro10",
"kaypro2x",
"kaypro4",
"kaypro4a",
"kaypro4p88",
"kayproii",
"robie",
"kc85_111",
"kc85_3",
"kc85_4",
"kc85_5",
"kc87_10",
"kc87_11",
"kc87_20",
"kc87_21",
"kccomp",
"bmsoccer",
"kim1",
"kingman",
"kmc5000",
"bmboxing",
"compani1",
"konin",
"kontiki",
"kr03",
"kramermc",
"krista2",
"kristall2",
"kt76",
"quorum48",
"kc85",
"ladictee",
"lambda",
"lantutor",
"lnsy1392",
"laser110",
"laser128",
"las128ex",
"las128e2",
"laser200",
"laser210",
"las3000",
"laser310",
"laser310h",
"laser350",
"laser500",
"laser700",
"lcmate2",
"pc4",
"laseractj",
"laseract",
"clcd",
"leapster",
"leapstertv",
"lvision",
"leonardo",
"lc80",
"lc80_2",
"taitons1",
"lexipcm",
"lft1230",
"lft1510",
"lik",
"lisa",
"lisa2",
"lisa210",
"pulsarlb",
"lilprof",
"lilprof78",
"ampro",
"llc1",
"llc2",
"lnw80",
"lola8a",
"casloopy",
"octopus",
"luxorvec",
"luxorves",
"lynx",
"lynx128k",
"lynx48k",
"lynx96k",
"lzcolor64",
"m10",
"cm1200",
"m5p",
"m5",
"shmc1200",
"olivm15",
"m24",
"m240",
"m6805evs",
"m82",
"unitron",
"machiman",
"mac128k",
"mac512k",
"mac512ke",
"macclasc",
"macclas2",
"maccclas",
"macii",
"mac2fdhd",
"maciihmu",
"maciici",
"maciicx",
"maciifx",
"maciisi",
"maciivi",
"maciivx",
"maciix",
"maclc",
"maclc520",
"maclc2",
"maclc3",
"macplus",
"macprtb",
"macpb100",
"macpb140",
"macpb145",
"macpb145b",
"macpb160",
"macpb170",
"macpb180",
"macpb180c",
"macqd700",
"macse",
"macsefd",
"macse30",
"macxl",
"magic6",
"mstation",
"manager",
"maniac",
"mmerlin",
"smsj",
"sms1",
"sms1pal",
"sms",
"smspal",
"smssdisp",
"mathmagi",
"mato",
"bml3",
"bml3mk2",
"bml3mk5",
"mbh2",
"mbh25",
"mbh50",
"mbh70",
"mbc16",
"mbc200",
"mbc55x",
"mc10",
"mc1000",
"mc8020",
"mc8030",
"mccpm",
"mc7105",
"mcb216",
"basic31",
"basic52",
"megadriv",
"megadrij",
"32xe",
"32xj",
"megaduck",
"megast_fr",
"megast_de",
"megast_se",
"megast_sg",
"megast_uk",
"megast",
"megaste_fr",
"megaste_de",
"megaste_it",
"megaste_es",
"megaste_se",
"megaste_uk",
"megaste",
"megacda",
"megacd",
"megacdj",
"megacd2",
"megacd2j",
"aiwamcd",
"megapc",
"megapcpl",
"megapcpla",
"mekd2",
"mx2178",
"mm4",
"mm4tk",
"mm50",
"mm5",
"mm5tk",
"academy",
"alm32",
"alm16",
"amsterd",
"berlinp",
"bpl32",
"dallas",
"dallas16",
"dallas32",
"lond030",
"gen32",
"gen32_41",
"gen32_oc",
"glasgow",
"lond020",
"lyon16",
"lyon32",
"megaiv",
"milano",
"mm2",
"monteciv",
"phc64",
"polgar",
"rebel5",
"roma",
"roma32",
"van16",
"van32",
"m79152pc",
"meritum",
"meritum_net",
"merlin",
"tonto",
"mes",
"mice",
"md2",
"md3",
"mpf1",
"mpf1p",
"mpf1b",
"mbee128p",
"mbee128",
"mbee",
"mbee256",
"mbeeic",
"mbee56",
"mbeepc85",
"mbeepc85b",
"mbeepc85s",
"mbeepc",
"mbeeppc",
"mbeett",
"mkit09",
"mmf9000",
"mmf9000_se",
"micronic",
"mprof3",
"microtan",
"microvsn",
"mikro80",
"mikrolab",
"mm1m6",
"mm1m7",
"mikron2",
"mikrosha",
"m86rk",
"mvbfree",
"mini2440",
"minicom",
"misterx",
"mistrum",
"mk14",
"mk82",
"mk83",
"mk85",
"mk88",
"mk90",
"mlf110",
"mlf120",
"mlf48",
"mlf80",
"mlfx1",
"mlg1",
"mlg10",
"mlg3",
"mlg30",
"mmd1",
"mmd2",
"mmt8",
"mo5",
"mo5nr",
"mo5e",
"mo6",
"mod8",
"modellot",
"molecula",
"mpc100",
"mpc200",
"mpc200sp",
"mpc2300",
"mpc2500f",
"mpc64",
"mpt02h",
"mpt05",
"mpu1000",
"mpu2000",
"imsai",
"mpz80",
"mr61",
"mrrack",
"mratlus",
"ms0515",
"cmspacmn",
"ms9540",
"msbc1",
"mt32",
"mtx500",
"mtx512",
"mu100",
"mu100r",
"multi16",
"multi8",
"multmega",
"mx10",
"mx101",
"mx15",
"mx1600",
"mx64",
"myvision",
"myb3k",
"mycom",
"mz1500",
"mz2000",
"mz2200",
"mz2500",
"mz2520",
"mz3500",
"mz6500",
"mz700",
"mz700j",
"mz800",
"mz80a",
"mz80b",
"mz80k",
"mz80kj",
"nanos",
"nascom1",
"nascom2",
"nc100",
"nc150",
"nc200",
"nd80z",
"neat",
"ct386sx",
"aes",
"neocd",
"neocdzj",
"neocdz",
"ngp",
"ngpc",
"newbraina",
"newbrain",
"newbraineim",
"newbrainmd",
"next",
"nextct",
"nextctc",
"nexts",
"nexts2",
"nextsc",
"nextst",
"nextstc",
"ngen",
"nimbus",
"n64",
"n64dd",
"nespal",
"nes",
"nms801",
"nms8220",
"nms8220a",
"nms8245",
"nms8245f",
"nms8250",
"nms8250f",
"nms8250j",
"nms8255",
"nms8255f",
"nms8260",
"nms8280",
"nms8280f",
"nms8280g",
"diablo68",
"sexpertb",
"sexpertc",
"sfortea",
"sforteb",
"sforteba",
"sfortec",
"ob68k1a",
"krvnjvtv",
"oc2000",
"okean240t",
"okean240a",
"odyssey2",
"odyssey3",
"okean240",
"m20",
"m40",
"omni2",
"ondrat",
"ondrav",
"orao",
"orao103",
"psion1",
"psioncm",
"psionla",
"psionlam",
"psionlz",
"psionlz64",
"psionlz64s",
"psionp200",
"psionp350",
"psionp464",
"psionxp",
"oric1",
"orica",
"telstrat",
"orion128",
"orionms",
"orionz80",
"orionzms",
"orionide",
"orionidm",
"orionpro",
"orizon",
"ormatu",
"osborne1",
"nano",
"omv1000",
"omv2000",
"pimps",
"p112",
"p500",
"p500p",
"p8000_16",
"p8000",
"tmpacman",
"epacman2",
"cpacmanr1",
"cpacman",
"packmon",
"plldium",
"palmiii",
"palmiiic",
"palmm100",
"palmm130",
"palmm505",
"palmm515",
"palmpers",
"palmpro",
"palmv",
"palmvx",
"palmz22",
"partner",
"pasogo",
"pasopia",
"paso1600",
"pasopia7lcd",
"pasopia7",
"pb1000",
"pb2000c",
"pc",
"pcega",
"pcherc",
"pcmda",
"xtvga",
"cmdpc30",
"pce",
"pc100",
"ataripc3",
"pc486mu",
"pc6001",
"pc6001a",
"pc6001mk2",
"pc6001sr",
"pc6601",
"pc7000",
"pc8001",
"pc8001mk2",
"pc8201",
"pc8201a",
"npc8300",
"pc8401a",
"pc8500",
"pc8801",
"pc8801fa",
"pc8801ma",
"pc8801ma2",
"pc8801mc",
"pc8801mh",
"pc8801mk2",
"pc8801mk2fr",
"pc8801mk2mr",
"pc8801mk2sr",
"pc88va",
"pc88va2",
"pc9801bx2",
"pc9801f",
"pc9801rs",
"pc9801rx",
"pc9801ux",
"pc9801vm",
"pc9821as",
"pc9821v13",
"pc9821v20",
"pc9821xs",
"pc9821",
"pc9821ce2",
"pc9821ne",
"pc9821ap2",
"pcd",
"pce220",
"pcfx",
"pcfxga",
"pcg850v",
"at",
"atvga",
"at386",
"at486",
"ct486",
"at586x3",
"at586",
"pcm",
"pc1512dd",
"pc1512hd10",
"pc1512hd20",
"pc1512",
"pc1640dd",
"pc1640hd20",
"pc1640hd30",
"pc1640",
"pc200",
"pc8300",
"pcw10",
"pcw16",
"pcw8256",
"pcw8512",
"pcw9256",
"pcw9512",
"pdp1",
"pdp11qb",
"pdp11ub",
"pdp11ub2",
"pecom32",
"pecom64",
"pencil2",
"pda600",
"pentagon",
"pent1024",
"olypeopl",
"perfect1",
"prsarcde",
"votrpss",
"pet2001",
"pet20018",
"pet2001b16",
"pet2001b32",
"pet2001b",
"pet2001n16",
"pet2001n32",
"pet2001n",
"pet4016",
"pet4032",
"pet4032f",
"pet4032b",
"pet64",
"pet8032",
"phc2",
"phc25",
"phc25j",
"phc28",
"phc28l",
"phc28s",
"p2000m",
"p2000t",
"phunsy",
"pico",
"picoj",
"picou",
"pilot1k",
"pilot5k",
"pioner",
"pipbug",
"pippin",
"pitagjr",
"lviv",
"pk6128c",
"hobby",
"vesta",
"pk8002",
"kontur",
"korvet",
"neiva",
"plan80",
"plus4",
"plus4p",
"pm68k",
"pmd851",
"pmd852",
"pmd852a",
"pmd852b",
"pmd853",
"pmi80",
"pc1245",
"pc1250",
"pc1251",
"pc1255",
"pc1260",
"pc1261",
"pc1350",
"pc1360",
"pc1401",
"pc1402",
"pc1403",
"pc1403h",
"pc1450",
"pc1500",
"poisk1",
"poisk2",
"pokemini",
"gpoker",
"poly1",
"poly88",
"poly8813",
"poly880",
"polyvcg",
"poppympt",
"pofo",
"pow3000",
"pmac6100",
"pv9234",
"pp01",
"pp1292",
"pp1392",
"cvicny",
"prav82",
"prav8c",
"prav8d",
"prav8dd",
"prav8m",
"pc1000",
"pc2000",
"prestige",
"prestmpt",
"primoa32",
"primoa48",
"primoa64",
"primob32",
"primob48",
"primob64",
"primoc64",
"pro80",
"pro128",
"prof180x",
"prof181x",
"prof80",
"profweis",
"profi",
"prose2ko",
"prose2k",
"pt68k2",
"pt68k4",
"pv1000",
"pv16",
"pv2000",
"pv7",
"pve500",
"piopx7",
"piopx7uk",
"px4",
"px4p",
"px8",
"piopxv60",
"pyl601",
"pyl601a",
"pbqbert",
"ql_dk",
"ql_fr",
"ql_de",
"ql_gr",
"ql_it",
"ql_es",
"ql_se",
"ql",
"ql_us",
"qtsbc",
"quorum",
"qx10",
"radio86",
"radio16",
"radio4k",
"rk700716",
"rk7007",
"radiorom",
"radioram",
"radio99",
"radionic",
"rainbow100a",
"rainbow",
"rainbow190",
"raisedvl",
"rameses",
"ravens",
"ravens2",
"rvoicepc",
"replica1",
"rex6000",
"ringo470",
"rpc600",
"rpc700",
"risc",
"rm380z",
"sh4robot",
"rt1715",
"rt1715lc",
"rt1715w",
"rowtrn2k",
"rwtrntcs",
"rpc86",
"rs128",
"sabavdpl",
"sabavpl2",
"sacstate",
"bmsafari",
"sage2",
"samcoupe",
"ssam88s",
"sapi1",
"sapizps2",
"sapizps3",
"sapizps3b",
"sapizps3a",
"saturnjp",
"saturneu",
"saturn",
"savia84",
"sbc6510",
"sc3000",
"sf7000",
"sc3000h",
"sc80",
"sc1",
"sc2",
"tvg2000",
"scorpio",
"seattle",
"tmscramb",
"sd1",
"sd132",
"sdk85",
"sdk86",
"segacd",
"32x_scd",
"segacd2",
"selz80",
"sg1000",
"sg1000m2",
"sg1000m3",
"ti99_4p",
"sheenhvc",
"instruct",
"simon",
"sitcom",
"slc1",
"slicer",
"sm1800",
"ssem",
"smc777",
"sms1000",
"socrates",
"socratfc",
"softbox",
"sol20",
"psa",
"pse",
"psj",
"psu",
"pockstat",
"sorcerer",
"sorcererd",
"sc55",
"soundic",
"space84",
"einvader",
"splasfgt",
"sun4_60",
"sun_s10",
"sun4_75",
"sun_s20",
"sun4_40",
"sun4_50",
"sun4_20",
"spc1000",
"spc800",
"fspc800",
"snmathp",
"snmath",
"snread",
"snspelljp",
"snspelluk",
"snspelluka",
"snspell",
"snspella",
"snspellb",
"special",
"specialp",
"specialm",
"specimx",
"spektrbk",
"spektr01",
"splitsec",
"spt1500",
"spt1700",
"spt1740",
"sq1",
"sq80",
"sqrack",
"tisr16",
"sr16",
"tisr16ii",
"st_fr",
"st_de",
"st_nl",
"st_es",
"st_se",
"st_sg",
"st_uk",
"st",
"msthawk",
"starwbc",
"starwbcp",
"ste_fr",
"ste_de",
"ste_it",
"ste_es",
"ste_se",
"ste_sg",
"ste_uk",
"ste",
"stopthie",
"stopthiep",
"sfach",
"sfzbch",
"sfzch",
"sarpc",
"studio2",
"sun2_120",
"sun2_50",
"sun3_110",
"sun3_260",
"sun3_50",
"sun3_60",
"sun3_150",
"sun3_e",
"sun3_460",
"sun3_80",
"sun4_300",
"sun1",
"supracan",
"scv",
"scv_pal",
"supergb",
"snespal",
"snes",
"spc4000",
"ssimon",
"super6",
"superslv",
"intvsrs",
"svision",
"svisionn",
"svisionp",
"svisions",
"sv8000",
"super80e",
"super80",
"super80d",
"super80m",
"super80v",
"super80r",
"sb2m600b",
"sbrain",
"supercon",
"sgx",
"superpet",
"zdsupers",
"ssfball",
"svi318n",
"svi318",
"svi328n",
"sv328n80",
"svi328",
"sv328p80",
"svi603",
"svi728",
"svi738ar",
"svi738dk",
"svi738",
"svi738pl",
"svi738sp",
"svi738sw",
"swtpc",
"swtpc09",
"swtpc09i",
"swtpc09u",
"swtpc09d3",
"swyft",
"sx16",
"sx64",
"sx64p",
"sym1",
"systec",
"fanucs15",
"sys2900",
"a6809",
"fanucspg",
"fanucspgm",
"sys80",
"t2000sx",
"test410",
"test420",
"t9000",
"vcc",
"t1000hx",
"t1000rl",
"t1000sl2",
"t1000sx",
"t1000tl2",
"t1000tx",
"tandy102",
"tandy200",
"tandy2k",
"tandy2khd",
"tandy12",
"tanodr64",
"tb303",
"tc2048",
"tdv2324",
"tec1",
"tecjmon",
"tek4051",
"tek4052a",
"tek4107a",
"tek4109a",
"tccosmos",
"telefevr",
"telngtcs",
"tmc2000",
"tmc2000e",
"tmc600s2",
"tempestm",
"wofch",
"tmtennis",
"terak",
"batmantv",
"avigo",
"avigo_fr",
"avigo_de",
"avigo_it",
"avigo_es",
"tibusan1",
"ti990_10",
"ti990_4",
"ti990_4v",
"tiprog",
"ti1000",
"ti1270",
"ti30",
"ti73",
"ti74",
"ti81",
"ti81v2",
"ti82",
"ti83",
"ti83p",
"ti83pse",
"ti84p",
"ti84pse",
"ti85",
"ti86",
"ti89",
"ti89t",
"ti92",
"ti92p",
"ti95",
"ti99_224",
"ti99_232",
"ti99_4e",
"ti99_4",
"ti99_4ae",
"ti99_4a",
"ti99_4ev",
"ti99_4qi",
"ti99_4qe",
"ti99_8e",
"ti99_8",
"ti630",
"tiki100",
"tim011",
"tim100",
"ts1000",
"ts1500",
"ht68k",
"tk90x",
"tk95",
"tk80",
"tk80bs",
"nectk85",
"tk2000",
"tk3000",
"tk85",
"990189",
"990189v",
"evmbug",
"to7",
"to770",
"to770a",
"to8",
"to8d",
"to9",
"to9p",
"tbbympt3",
"pyuuta",
"pyuutajr",
"tutor",
"tc4",
"tpc310",
"tpp311",
"tps312",
"trakcvg",
"tricep",
"tmtron",
"trsm100",
"trs80m16",
"trs80m4",
"trs80m4p",
"trs80",
"trs80l2",
"trs80m2",
"trs80m3",
"trs80pc3",
"tryomvgc",
"ts2068",
"ts802",
"ts802h",
"ts803h",
"ts816",
"tt030_fr",
"tt030_de",
"tt030_pl",
"tt030_uk",
"tt030",
"tunixha",
"tg16",
"tvlinkp",
"tv950",
"tvc64",
"tvc64p",
"tvc64pru",
"tvc4000",
"tx0_64kw",
"tx0_8kw",
"tx8000",
"votrtnt",
"ufombs",
"uk2086",
"uk101",
"uknc",
"unior",
"unistar",
"hpz80unk",
"ut88",
"ut88mini",
"uts20",
"orbituvi",
"uzebox",
"canonv10",
"canonv20",
"canonv20e",
"canonv20f",
"canonv20g",
"canonv20s",
"canonv25",
"canonv30",
"canonv30f",
"canonv8",
"vsaturn",
"vsmilef",
"vsmile",
"vax785",
"vc6000",
"vcs80",
"vector06",
"vector1",
"vec1200",
"vector4",
"vectrex",
"v6809",
"vfx",
"vfxsd",
"vg5k",
"vg8000",
"vg8010",
"vg8010f",
"vg802000",
"vg802020",
"vg8020f",
"vg8230",
"vg8230j",
"vg8235",
"vg8235f",
"vg8240",
"vic10",
"vic1001",
"vic20",
"vic20_se",
"vic20p",
"victor",
"victor9k",
"mpt02",
"vdmaster",
"vidbrain",
"vmdtbase",
"videopac",
"g7400",
"vii",
"vip64",
"vboy",
"visicom",
"visor",
"v1050",
"svmu",
"vixen",
"vk100",
"vsc",
"database",
"v200",
"pes",
"ficvt503",
"vt100",
"vt101",
"vt102",
"vt105",
"vt131",
"vt180",
"vt220",
"vt320",
"vt520",
"vta2000",
"iqunlim",
"itunlim",
"vz2000",
"vz200de",
"vz200",
"vz300",
"walle",
"wangpc",
"mpc10",
"mpc25fd",
"mpc27",
"phc23",
"phc35j",
"phc55fd2",
"phc70fd",
"phc70fd2",
"phc77",
"wicat",
"wildfire",
"wizatron",
"wizzard",
"wmega",
"wmegam2",
"wswan",
"wscolor",
"mwcbaseb",
"xeye",
"x07",
"cpc400",
"cpc400s",
"x1",
"x1turbo",
"x1turbo40",
"x1twin",
"x68000",
"x68ksupr",
"x68kxvi",
"x68030",
"x168",
"x820",
"x820ii",
"xor100",
"copera",
"yc64",
"y503iiire",
"y503iiir",
"yis303",
"yis503",
"yis503f",
"yis503ii",
"y503iir2",
"y503iir",
"yis503m",
"yis604",
"yis60464",
"y805128",
"y805256",
"y805128r2e",
"y805128r2",
"z100",
"z1013s60",
"z1013k76",
"z1013k69",
"z1013",
"z1013a2",
"z80dev",
"z80ne",
"z80net",
"z80netb",
"z80netf",
"z88",
"z88dk",
"z88fi",
"z88fr",
"z88de",
"z88it",
"z88no",
"z88es",
"z88se",
"z88ch",
"z88tr",
"z9001",
"zackman",
"zsl5500",
"zsl5600",
"zslc1000",
"zslc3000",
"zslc750",
"zslc760",
"cpc50a",
"cpc50b",
"cpc51",
"cpc61",
"cpg120",
"zexall",
"zorba",
"zrt80",
"zsbc3",
"zvezda",
"spectrum",
"specpls2",
"specpl2a",
"specpls3",
"specpl3e",
"sp3e8bit",
"sp3eata",
"sp3ezcf",
"spec128",
"spec80k",
"specide",
"zx80",
"zx81",
"zx97"
			};

	}
}
#endif
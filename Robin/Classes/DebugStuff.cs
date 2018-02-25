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

#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
				Directory.CreateDirectory(FileLocation.Art.BoxFrontThumbs);
				foreach (Release release in R.Data.Releases.Local)
				{
					release.CreateThumbnail();
				}
			});

			Reporter.Report("Finished");
		}

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
			Platform arcadePlatform = R.Data.Platforms.FirstOrDefault(x => x.ID == CONSTANTS.ARCADE_PLATFORM_ID);

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

			using (Process process = MAME.MAMEexe(@"-lx"))
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
			foreach (Release release in R.Data.Releases.Local)
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

		static void MoveToGame()

		{
			foreach (Game game in R.Data.Games.Local)
			{
				game.IsGame = game.Releases[0].IsGame;
				game.IsCrap = game.Releases[0].IsCrap;
				game.IsAdult = game.Releases[0].IsAdult;
				game.IsBeaten = game.Releases[0].IsBeaten;
				game.Unlicensed = game.Releases[0].Unlicensed;

				int i = 0;
				while (game.Overview == null && i < game.Releases.Count)
				{
					game.Overview = game.Releases[i].Overview;
					i++;
				}

				i = 0;
				while (game.Developer == null && i < game.Releases.Count)
				{
					game.Developer = game.Releases[i].Developer;
					i++;
				}

				i = 0;
				while (game.Publisher == null && i < game.Releases.Count)
				{
					game.Publisher = game.Releases[i].Publisher;
					i++;
				}

				i = 0;
				while (game.VideoFormat == null && i < game.Releases.Count)
				{
					game.VideoFormat = game.Releases[i].VideoFormat;
					i++;
				}

				i = 0;
				while (game.Players == null && i < game.Releases.Count)
				{
					game.Players = game.Releases[i].Players;
					i++;
				}

				i = 0;
				while (game.Rating == null && i < game.Releases.Count)
				{
					game.Rating = game.Releases[i].Rating;
					i++;
				}

				List<string> genres = new List<string>();

				foreach (Release release in game.Releases)
				{
					genres.AddRange(release.GenreList);
				}

				game.Genre = string.Join(", ", genres.Distinct().OrderBy(x => x));



			}
		}
	}
}
#endif
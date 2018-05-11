using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public class Audit
	{
		/// <summary>
		/// Result of auditing a rom file on the disk
		/// </summary>
		public class Result
		{
			public Rom Rom { get; set; }
			public Rom Parent { get; set; }
			public Status Status { get; set; }
			public string FilePath { get; set; }
			public string CRC32 { get; set; }
			public string MD5 { get; set; }
			public string SHA1 { get; set; }
			public string FileName => Path.GetFileName(FilePath);

			public Result()
			{
			}

			public Result(Rom rom)
			{
				Rom = rom;
				FilePath = rom.FilePath;
			}
		}

		/// <summary>
		/// Get an audit result from a line in MAME.exe -verifyroms commandline switch
		/// </summary>
		/// <param name="line">ANy line in in the string returned by m=MAME.exe -verifyroms</param>
		/// <returns></returns>
		public static Result GetResultFromMameLine(string line)
		{
			Result returner = new Result();

			// Standardize line format
			line = line.Replace("romset ", "").Replace(" is", "").Replace(" available", "");

			string[] liner = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			returner.Rom = R.Data.Roms.FirstOrDefault(x => x.FileName == liner[0] + ".zip");

			// liner length will b 2 if there is no parent
			if (liner.Length == 2)
			{
				if (Enum.TryParse(liner[1].Capitalize(), out Status status))
				{
					returner.Status = status;
				}
			}

			// liner length will be 3 if there is a parent--liner [2] == "[parentname]"
			if (liner.Length == 3)
			{
				returner.Parent = R.Data.Roms.FirstOrDefault(x => x.FileName == liner[1].Replace("[", "").Replace("]", "") + ".zip");

				if (Enum.TryParse(liner[2].Capitalize(), out Status status))
				{
					returner.Status = status;

				}
			}
			Debug.Assert((returner.Status != 0), $"{returner.Rom.FileName} has an unknown status");

			return returner;
		}

		/// <summary>
		/// Get the checksum of a file and return as a string
		/// </summary>
		/// <param name="file">File name of file to get hash from</param>
		/// <param name="hashOption">Default = SHA1, also MD5 and CRC32</param>
		/// <param name="headerlength">Number of bytes to skip prior to computing hash. Should try both platform header length and 0 when comparing to known checksum to account for Rom files with dumped intros. Default = 0.</param>
		/// <returns>Returns hash as hex string</returns>
		public static string GetHash(string file, HashOption hashOption = HashOption.SHA1, int headerlength = 0)
		{
			string hash;
			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				hash = GetHash(stream, hashOption, headerlength);
			}
			return hash;
		}

		/// <summary>
		/// Get the CRC of a file stream and return as a string
		/// </summary>
		/// <param name="stream">FileStream</param>
		/// <param name="hashOption">Default = SHA1, also MD5 and CRC32</param>
		/// <param name="headerlength">Number of bytes to skip prior to computing hash. Should try both platform header length and 0 when comparing to known checksum to account for Rom files with dumped intros. Default = 0.</param>
		/// <returns>Returns hash as hex string</returns>
		public static string GetHash(Stream stream, HashOption hashOption = HashOption.SHA1, int headerlength = 0)
		{
			string hash = "";
			int streamLength = (int)stream.Length;

			if (streamLength < headerlength)
			{
				return "";
			}
			// Read header and discard
			stream.Seek(headerlength, SeekOrigin.Begin);

			// Read everything after the header into a byte array
			byte[] buffer = new byte[streamLength - headerlength];
			stream.Read(buffer, 0, (streamLength - headerlength));

			switch (hashOption)
			{
				case HashOption.CRC32:
					var crc32 = new Ionic.Crc.CRC32();
					foreach (byte b in buffer)
					{
						crc32.UpdateCRC(b);
					}
					hash = crc32.Crc32Result.ToString("x8").ToUpper();
					break;

				case HashOption.MD5:
					var md5 = MD5.Create();
					byte[] hashBytes = md5.ComputeHash(buffer);
					hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToUpper();
					break;

				case HashOption.SHA1:
					SHA1Managed managedSHA1 = new SHA1Managed();
					byte[] shaBuffer = managedSHA1.ComputeHash(buffer);
					foreach (byte b in shaBuffer)
					{
						hash += b.ToString("x2").ToUpper();
					}
					break;
			}

			return hash;
		}

		public static TitledCollection<Result> AuditRoms(Platform platform)
		{
			;
			if (platform.ID == CONSTANTS.ARCADE_PLATFORM_ID)
			{
				return Mame.Database.AuditRoms();
			}

			else
			{
				return AuditNonMameRoms(platform);
			}
		}

		/// <summary>
		/// Audits existance of roms for patforms that are not mame. Cycles through all roms under the platform, checks the existance of a file under ROM.filename, checks the  crc
		/// </summary>
		/// <returns>A list of Audit.Result </returns>
		static TitledCollection<Result> AuditNonMameRoms(Platform platform)
		{
			TitledCollection<Result> returner = new TitledCollection<Result>(platform.Title);

			// Record all files in the directory to keep track of which have been audited
			HashSet<string> files = new HashSet<string>(Directory.GetFiles(platform.RomDirectory));

			int headerLength = (int)platform.HeaderLength;
			int romCount = platform.Roms.Count;

			Reporter.Tic($"Auditing {romCount} {platform.Title} database ROMs...", out int tic1);
			// First audit all ROMS in the database
			foreach (Rom rom in platform.Roms)
			{
				var result = new Result(rom);
				returner.Add(result);

				if (File.Exists(rom.FilePath))
				{
					if (rom.CRC32 != null)
					{
						if (rom.CRC32 == GetHash(rom.FilePath, HashOption.CRC32, headerLength) ||
							rom.CRC32 == GetHash(rom.FilePath, HashOption.CRC32, 0))
						{
							result.Status = Status.Good;
						}
						else
						{
							result.Status = Status.Bad;
						}
					}

					else if (rom.MD5 != null)
					{
						if (rom.MD5 == GetHash(rom.FilePath, HashOption.MD5, headerLength) ||
							rom.MD5 == GetHash(rom.FilePath, HashOption.MD5, 0))
						{
							result.Status = Status.Good;
						}
						else
						{
							result.Status = Status.Bad;
						}
					}

					else if (rom.SHA1 != null)
					{
						if (rom.SHA1 == GetHash(rom.FilePath, HashOption.SHA1, headerLength) ||
							rom.SHA1 == GetHash(rom.FilePath, HashOption.SHA1, 0))
						{
							result.Status = Status.Good;
						}
						else
						{
							result.Status = Status.Bad;
						}
					}

					else
					{
						result.Status = Status.Unknown;
					}
				}

				else
				{
					result.Status = Status.Missing;
				}

				// Remove from the list so we don't check again below
				files.Remove(rom.FilePath);
			}
			Reporter.Toc(tic1);

			// Then go through all files in the folder not checked yet and mark as superfluous
			Reporter.Tic("Creating hash sets...", out int tic2);
			HashSet<string> crcs = new HashSet<string>(returner.Select(x => x.CRC32));
			HashSet<string> sha1s = new HashSet<string>(returner.Select(x => x.SHA1));
			HashSet<string> md5s = new HashSet<string>(returner.Select(x => x.MD5));
			Reporter.Toc(tic2);

			Reporter.Tic("Auditing orpan files on disks...", out int tic3);
			foreach (string file in files)
			{
				Result result = new Result
				{
					Status = Status.Extra,
					FilePath = file
				};

				if (crcs.Contains(GetHash(file, HashOption.CRC32, (int)platform.HeaderLength)) ||
					crcs.Contains(GetHash(file, HashOption.CRC32, 0)) ||
					md5s.Contains(GetHash(file, HashOption.MD5, (int)platform.HeaderLength)) ||
					md5s.Contains(GetHash(file, HashOption.MD5, 0)) ||
					sha1s.Contains(GetHash(file, HashOption.SHA1, (int)platform.HeaderLength)) ||
					sha1s.Contains(GetHash(file, HashOption.SHA1, 0)))
				{
					result.Status = Status.Duplicate;
				}
			}
			Reporter.Toc(tic3);

			return returner;

		}
	}

	public enum Status
	{
		[Description("Unknown")]
		Unknown,
		[Description("Good")]
		Good,
		[Description("Bad")]
		Bad,
		[Description("Best")]
		Best,
		[Description("Missing")]
		Missing,
		[Description("Duplicate")]
		Duplicate,
		[Description("Extra")]
		Extra,
		[Description("Does Not Exist")]
		DNE
	}

	public enum HashOption
	{
		[Description("CRC32")]
		CRC32,
		[Description("SHA1")]
		SHA1,
		[Description("MD5")]
		MD5
	}
}


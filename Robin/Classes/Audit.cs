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

		public class Result
		{
			public Rom Rom { get; set; }
			public Rom Parent { get; set; }
			public Status Status { get; set; }

			public Result()
			{
			}

			public Result(Rom rom)
			{
				Rom = rom;
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

			returner.Rom = R.Data.Roms.Local.FirstOrDefault(x => x.FileName == liner[0] + ".zip");

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
				returner.Parent = R.Data.Roms.Local.FirstOrDefault(x => x.FileName == liner[1].Replace("[", "").Replace("]", "") + ".zip");

				if (Enum.TryParse(liner[2].Capitalize(), out Status status))
				{
					returner.Status = status;

				}
			}
			Debug.Assert((returner.Status != 0), $"{returner.Rom.FileName} has an unknown status");

			return returner;
		}

		/// <summary>
		/// Get the CRC of a file and return as a string
		/// </summary>
		/// <param name="file">FIle name of file to get hash from</param>
		/// <param name="method">Default = SHA1. No other method yet</param>
		/// <param name="headerlength">Number of bytes to skip prior to computing hash. Default = 0.</param>
		/// <returns>CRC as hex string</returns
		public static string GetHash(string file, HashOptions hashOption = HashOptions.SHA1, int headerlength = 0)
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
		/// <param name="method">Default = SHA1. No other method yet</param>
		/// <param name="headerlength">Number of bytes to skip prior to computing hash. Default = 0.</param>
		/// <returns>CRC as hex string</returns>
		public static string GetHash(Stream stream, HashOptions hashOption = HashOptions.SHA1, int headerlength = 0)
		{
			string hash = "";
			int streamLength = (int)stream.Length;

			if (streamLength < headerlength)
			{
				return "";
			}
			// Read header
			stream.Seek(headerlength, SeekOrigin.Begin);

			// Read buffer
			byte[] buffer = new byte[streamLength - headerlength];
			stream.Read(buffer, 0, (streamLength - headerlength));

			switch (hashOption)
			{
				case HashOptions.CRC32:

					var crc32 = new Ionic.Crc.CRC32();

					foreach(byte b in buffer)
					{
						crc32.UpdateCRC(b);
					}
					hash = crc32.Crc32Result.ToString("x8").ToUpper();

					break;


				case HashOptions.SHA1:
					SHA1Managed managedSHA1 = new SHA1Managed();
					byte[] shaBuffer = managedSHA1.ComputeHash(buffer);
					foreach (byte b in shaBuffer)
					{
						hash += b.ToString("x2").ToUpper();
					}
					break;

				case HashOptions.MD5:
					using (var md5 = MD5.Create())
					{
						byte[] hashBytes = md5.ComputeHash(buffer);
						hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToUpper();
					}
					break;
			}

			return hash;
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
		Missing
	}

	public enum HashOptions
	{
		[Description("CRC32")]
		CRC32,
		[Description("SHA1")]
		SHA1,
		[Description("MD5")]
		MD5
	}
}


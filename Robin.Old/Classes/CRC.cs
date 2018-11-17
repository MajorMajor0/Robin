//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Security.Cryptography;

//namespace Robin
//{
//    [Serializable]
//    public class crc
//    {
//        public string SHA1 { get; set; }
//        public string MD5 { get; set; }
//        public string CRC32 { get; set; }

//        string Default_crc = "0000000000";

//        public event PropertyChangedEventHandler PropertyChanged;

//        public crc()
//        {
//            SHA1 = Default_crc;
//            MD5 = Default_crc;
//            CRC32 = Default_crc;
//        }

//        public crc(string sha1 = "0000000000", string md5 = "0000000000", string crc32 = "0000000000")
//        {
//            SHA1 = sha1;
//            MD5 = md5;
//            CRC32 = crc32;
//        }

//        protected void OnPropertyChanged(string name)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//        }

//        public static crc Load(string file_name)
//        {
//            Crc32 crc32 = new Crc32();
//            string hash_crc32 = String.Empty;
//            string hash_md5 = String.Empty;
//            string hash_sha1 = string.Empty;
//            var Sha1 = System.Security.Cryptography.SHA1.Create();
//            var Md5 = System.Security.Cryptography.MD5.Create();

//            // Get CRC32
//            FileStream fs = File.OpenRead(file_name);
//            fs.Position = 0;
//            foreach (byte b in crc32.ComputeHash(fs)) hash_crc32 += b.ToString("x2").ToLower();

//            // Get SHA1 and MD5
//            fs.Position = 0;
//            hash_md5 = BitConverter.ToString(Md5.ComputeHash(fs)).Replace("-",String.Empty);
//            fs.Position = 0;
//            hash_sha1 = BitConverter.ToString(Sha1.ComputeHash(fs)).Replace("-", String.Empty);

//            return new crc(hash_sha1, hash_md5, hash_crc32);
//        }

//        // *****************************************************************************************************************
//        // Copyright (c) Damien Guard. All rights reserved.
//        // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
//        // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//        // Originally published at http://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net

//        /// Implements a 32-bit CRC hash algorithm compatible with Zip etc.
//        /// </summary>
//        /// <remarks>
//        /// Crc32 should only be used for backward compatibility with older file formats
//        /// and algorithms. It is not secure enough for new applications.
//        /// If you need to call multiple times for the same data either use the HashAlgorithm
//        /// interface or remember that the result of one Compute call needs to be ~ (XOR) before
//        /// being passed in as the seed for the next Compute call.
//        /// </remarks>

//        public sealed class Crc32 : HashAlgorithm
//        {
//            public const UInt32 DefaultPolynomial = 0xedb88320u;
//            public const UInt32 DefaultSeed = 0xffffffffu;
//            private static UInt32[] defaultTable;
//            private readonly UInt32 seed;
//            private readonly UInt32[] table;
//            private UInt32 hash;

//            public Crc32()
//                : this(DefaultPolynomial, DefaultSeed)
//            {
//            }
//            public Crc32(UInt32 polynomial, UInt32 seed)
//            {
//                table = InitializeTable(polynomial);
//                this.seed = hash = seed;
//            }

//            public override void Initialize()
//            {
//                hash = seed;
//            }

//            protected override void HashCore(byte[] buffer, int start, int length)
//            {
//                hash = CalculateHash(table, hash, buffer, start, length);
//            }

//            protected override byte[] HashFinal()
//            {
//                var hashBuffer = UInt32ToBigEndianBytes(~hash);
//                HashValue = hashBuffer;
//                return hashBuffer;
//            }

//            public override int HashSize { get { return 32; } }

//            public static UInt32 Compute(byte[] buffer)
//            {
//                return Compute(DefaultSeed, buffer);
//            }

//            public static UInt32 Compute(UInt32 seed, byte[] buffer)
//            {
//                return Compute(DefaultPolynomial, seed, buffer);
//            }

//            public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
//            {
//                return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
//            }

//            private static UInt32[] InitializeTable(UInt32 polynomial)
//            {
//                if (polynomial == DefaultPolynomial && defaultTable != null)
//                    return defaultTable;
//                var createTable = new UInt32[256];
//                for (var i = 0; i < 256; i++)
//                {
//                    var entry = (UInt32)i;
//                    for (var j = 0; j < 8; j++)
//                        if ((entry & 1) == 1)
//                            entry = (entry >> 1) ^ polynomial;
//                        else
//                            entry = entry >> 1;
//                    createTable[i] = entry;
//                }
//                if (polynomial == DefaultPolynomial)
//                    defaultTable = createTable;
//                return createTable;
//            }

//            private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size)
//            {
//                var crc = seed;
//                for (var i = start; i < size - start; i++)
//                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
//                return crc;
//            }

//            private static byte[] UInt32ToBigEndianBytes(UInt32 uint32)
//            {
//                var result = BitConverter.GetBytes(uint32);
//                if (BitConverter.IsLittleEndian)
//                    Array.Reverse(result);
//                return result;
//            }
//            // *******************************************************************************************************
//        }
//    }
//}



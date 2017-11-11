using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Zstandard.Net.Tests
{
    [TestClass]
    public class DictionaryTests
    {
        [TestMethod]
        public void DictionaryCompression_AllCompressionLevels_CorrectCompression()
        {
            var data = File.ReadAllBytes("Data/loremipsum.txt");
            var dictionary = File.ReadAllBytes("Data/loremipsum.zdict");

            for (int i = 1; i <= ZstandardStream.MaxCompressionLevel; i++)
            {
                var compressed = this.Compress(data, dictionary, i);
                Assert.IsTrue(compressed.Length < data.Length / 3);
                Assert.IsTrue(this.Decompress(compressed, dictionary).SequenceEqual(data));
            }
        }

        [TestMethod]
        public void DictionaryCompression_DifferentCompressionLevels_DifferentCompressionRatios()
        {
            var data = File.ReadAllBytes("Data/loremipsum.txt");
            var dictionary = File.ReadAllBytes("Data/loremipsum.zdict");
            var compressedMin = this.Compress(data, dictionary, 1);
            var compressedMax = this.Compress(data, dictionary, ZstandardStream.MaxCompressionLevel);
            Assert.IsTrue(compressedMin.Length > compressedMax.Length);
        }

        [TestMethod]
        public void StandardCompression_IncompressibleData_NoCompression()
        {
            var data = this.GetRandomData(65536);
            var dictionary = File.ReadAllBytes("Data/loremipsum.zdict");
            var compressed = this.Compress(data, dictionary, 11);
            Assert.IsTrue(compressed.Length < data.Length + 32);
        }

        //-----------------------------------------------------------------------------------------

        private byte[] Compress(byte[] data, byte[] dictionaryRaw, int compressionLevel)
        {
            using (var memoryStream = new MemoryStream())
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Compress))
            using (var dictionary = new ZstandardDictionary(dictionaryRaw))
            {
                compressionStream.CompressionLevel = compressionLevel;
                compressionStream.CompressionDictionary = dictionary;
                compressionStream.Write(data, 0, data.Length);
                compressionStream.Close();
                return memoryStream.ToArray();
            }
        }

        private byte[] Decompress(byte[] compressed, byte[] dictionaryRaw)
        {
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            using (var dictionary = new ZstandardDictionary(dictionaryRaw))
            using (var temp = new MemoryStream())
            {
                compressionStream.CompressionDictionary = dictionary;
                compressionStream.CopyTo(temp);
                return temp.ToArray();
            }
        }

        private byte[] GetRandomData(int size)
        {
            var data = new byte[size];
            var random = new Random();
            random.NextBytes(data);
            return data;
        }
    }
}

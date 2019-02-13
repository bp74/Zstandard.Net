using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;

namespace Zstandard.Net.Tests
{
    [TestClass]
    public class StandardTests
    {
        [TestMethod]
        public void StandardCompression_AllCompressionLevels_CorrectCompression()
        {
            var data = File.ReadAllBytes("Data/Test.txt");

            for (int i = 1; i <= ZstandardStream.MaxCompressionLevel; i++)
            {
                var compressed = this.Compress(data, i);
                Assert.IsTrue(compressed.Length < data.Length / 3);
                Assert.IsTrue(this.Decompress(compressed).SequenceEqual(data));
            }
        }

        [TestMethod]
        public void StandardCompression_DifferentCompressionLevels_DifferentCompressionRatios()
        {
            var data = File.ReadAllBytes("Data/Test.txt");
            var compressedMin = this.Compress(data, 1);
            var compressedMax = this.Compress(data, ZstandardStream.MaxCompressionLevel);
            Assert.IsTrue(compressedMin.Length > compressedMax.Length);
        }

        [TestMethod]
        public void StandardCompression_IncompressibleData_NoCompression()
        {
            var data = this.GetRandomData(65536);
            var compressed = this.Compress(data, 11);
            Assert.IsTrue(compressed.Length < data.Length + 32);
        }

        //-----------------------------------------------------------------------------------------

        private byte[] Compress(byte[] data, int compressionLevel)
        {
            using (var memoryStream = new MemoryStream())
            using (var compressionStream = new ZstandardStream(memoryStream, compressionLevel))
            {
                compressionStream.Write(data, 0, data.Length);
                compressionStream.Close();
                return memoryStream.ToArray();
            }
        }

        private byte[] Decompress(byte[] compressed)
        {
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            using (var temp = new MemoryStream())
            {
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

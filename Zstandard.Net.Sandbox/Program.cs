using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zstandard.Net.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = ZstandardStream.Version;
            var maxCompressionLevel = ZstandardStream.MaxCompressionLevel;

            var input = GetTestFile("kennedy.xls");
            var compressed = default(byte[]);
            var output = default(byte[]);

            // compress
            using (var memoryStream = new MemoryStream())
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Compress))
            {
                compressionStream.CompressionLevel = maxCompressionLevel;
                compressionStream.Write(input, 0, input.Length);
                compressionStream.Close();
                compressed = memoryStream.ToArray();
            }

            // decompress
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            {
                using (var ms = new MemoryStream())
                {
                    compressionStream.CopyTo(ms);
                    output = ms.ToArray();
                }
            }

            // test output
            if (output.SequenceEqual(input) == false)
            {
                throw new Exception("Output is different from input!");
            }
        }

        static byte[] GetTestFile(string name)
        {
            // http://corpus.canterbury.ac.nz/descriptions/#cantrbry

            using (var memoryStream = new MemoryStream())
            using (var zipFile = ZipFile.OpenRead("cantrbry.zip"))
            using (var zipFileStream = zipFile.GetEntry(name).Open())
            {
                zipFileStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}

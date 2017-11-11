using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Zstandard.Net.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = ZstandardStream.Version;
            var maxCompressionLevel = ZstandardStream.MaxCompressionLevel;
            var stopwatch = Stopwatch.StartNew();

            var input = GetTestFile("kennedy.xls");
            var compressed = default(byte[]);
            var output = default(byte[]);

            // compress
            using (var memoryStream = new MemoryStream())
            using (var compressionStream = new ZstandardStream(memoryStream, compressionLevel: 10))
            {
                //compressionStream.CompressionLevel = 6; // maxCompressionLevel;
                compressionStream.Write(input, 0, input.Length);
                compressionStream.Close();
                compressed = memoryStream.ToArray();
            }

            // decompress
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            using (var temp = new MemoryStream())
            {
                compressionStream.CopyTo(temp);
                output = temp.ToArray();
            }

            // test output
            if (output.SequenceEqual(input) == false)
            {
                throw new Exception("Output is different from input!");
            }

            // write info
            Console.WriteLine($"Input       : {input.Length}");
            Console.WriteLine($"Compressed  : {compressed.Length}");
            Console.WriteLine($"Output      : {output.Length}");
            Console.WriteLine($"-------------------------------------------");
            Console.WriteLine($"Ratio       : {1.0f * input.Length / compressed.Length}");
            Console.WriteLine($"Time        : {stopwatch.Elapsed.TotalMilliseconds} ms");

            Console.Read();
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

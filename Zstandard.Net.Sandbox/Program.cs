using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Zstandard.Net.Sandbox
{
    static class Program
    {
        static void Main(string[] args)
        {
            var version = ZstandardStream.Version;
            var maxCompressionLevel = ZstandardStream.MaxCompressionLevel;

            var input1 = GetTestFile("kennedy.xls");
            StandardCompression(input1, 6);
            Console.ReadLine();

            var input2 = File.ReadAllBytes("loremipsum.txt");
            var dictionary = new ZstandardDictionary("loremipsum.zdict");
            StandardCompression(input2, 22);
            DictionaryCompression(input2, dictionary, 22);
            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        private static void StandardCompression(byte[] input, int compressionLevel)
        {
            var stopwatch = Stopwatch.StartNew();
            var compressed = default(byte[]);
            var output = default(byte[]);

            // compress
            using (var memoryStream = new MemoryStream())
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Compress))
            {
                compressionStream.CompressionLevel = compressionLevel;
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
            Console.WriteLine($"Is64Bit     : {Environment.Is64BitProcess}");
            Console.WriteLine();
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        private static void DictionaryCompression(byte[] input, ZstandardDictionary dictionary, int compressionLevel)
        {
            var stopwatch = Stopwatch.StartNew();
            var compressed = default(byte[]);
            var output = default(byte[]);

            // compress
            using (var memoryStream = new MemoryStream())
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Compress))
            {
                compressionStream.CompressionLevel = compressionLevel;
                compressionStream.CompressionDictionary = dictionary;
                compressionStream.Write(input, 0, input.Length);
                compressionStream.Close();
                compressed = memoryStream.ToArray();
            }

            // decompress
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            using (var temp = new MemoryStream())
            {
                compressionStream.CompressionDictionary = dictionary;
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
            Console.WriteLine($"Is64Bit     : {Environment.Is64BitProcess}");
            Console.WriteLine();
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

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

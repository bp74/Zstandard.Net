using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.IO.Compression;
using Zstandard.Net.Benchmark.Tests;

namespace Zstandard.Net.Benchmark
{
    static class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CompressMemoryAllocation>();
        }

        public static byte[] GetTestFile(string name)
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

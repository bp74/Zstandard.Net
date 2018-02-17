using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using System;
using System.IO;
using System.IO.Compression;

namespace Zstandard.Net.Benchmark.Tests
{
    [MemoryDiagnoser]
    public class CompressMemoryAllocation
    {
        //private static byte[] data = Program.GetTestFile("fields.c"); // 10 KB
        //private static byte[] data = Program.GetTestFile("asyoulik.txt"); // 100 KB
        private static byte[] data = Program.GetTestFile("kennedy.xls"); // 1000 KB

        //-----------------------------------------------------------------------------------------
        
        [Benchmark]
        public long Deflate_Fastest()
        {
            return this.Compress(ms => new DeflateStream(ms, CompressionLevel.Fastest));
        }

        [Benchmark]
        public long Deflate_Optimal()
        {
            return this.Compress(ms => new DeflateStream(ms, CompressionLevel.Optimal));
        }

        [Benchmark]
        public long Zstandard_Default()
        {
            return this.Compress(ms => new ZstandardStream(ms, CompressionMode.Compress));
        }

        //-----------------------------------------------------------------------------------------

        private long Compress(Func<Stream, Stream> compressionStreamFunc)
        {
            using (var ms = new DevNullStream())
            {
                using (var cs = compressionStreamFunc(ms))
                {
                    cs.Write(data, 0, data.Length);
                }

                return ms.Length;
            }
        }

    }
}

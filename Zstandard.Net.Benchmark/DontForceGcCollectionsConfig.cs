using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Zstandard.Net.Benchmark
{
    public class DontForceGcCollectionsConfig : ManualConfig
    {
        public DontForceGcCollectionsConfig()
        {
            // tell BenchmarkDotNet not to force GC collections after every iteration
            AddJob(Job.Default.WithGcMode(new GcMode() { Force = false }));
        }
    }
}

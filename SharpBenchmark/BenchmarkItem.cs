using System;

namespace SharpBenchmark
{
    public class BenchmarkItem
    {
        public Action Test { get; set; }
        public string Title { get; set; }
    }
}
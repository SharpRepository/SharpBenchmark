using System;

namespace SharpBenchmark
{
    public class BenchmarkResult
    {
        public string Title { get; set; }
        public double TotalTime { get; set; }
        public int TimeRun { get; set; }

        public double AverageTime
        {
            get { return TotalTime/Convert.ToDouble(TimeRun); }
        }

        public string Display
        {
            get { return String.Format("   {0}\r\n      {1} ms total -- {2} avg ms per", Title, TotalTime, AverageTime); }
        }
    }
}
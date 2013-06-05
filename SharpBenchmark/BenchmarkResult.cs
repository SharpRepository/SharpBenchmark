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

        public string Display(double fastestTime)
        {
            string percentSlowerText = String.Empty;
            if (fastestTime != TotalTime)
            {
                percentSlowerText = String.Format("\r\n      {0:0.0%} slower", (TotalTime/fastestTime) - 1.0);
            }

            return String.Format("   {0}{3}\r\n      {1:#,0.00} ms total -- {2:0.0000} avg ms per", Title, TotalTime, AverageTime, percentSlowerText);
        }
    }
}
using System;

namespace SharpBenchmark.Samples
{
    public abstract class Sample : ISample
    {
        protected Benchmarker Benchmarker { get; set; }

        protected Sample()
        {
            Benchmarker = new Benchmarker() {CopyResultsToClipboard = true};
        }

        public void AddTest(string label, Action test)
        {
            Benchmarker.AddTest(label, test);
        }

        public void RunTests(int num)
        {
            Benchmarker.RunTests(num);
        }

        public abstract void Execute(string[] args);
    }
}
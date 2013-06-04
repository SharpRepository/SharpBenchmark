using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpBenchmark
{
    public class Benchmarker
    {
        private readonly IList<BenchmarkItem> _tests = new List<BenchmarkItem>();
        private readonly TextWriter _writer;
        private StringBuilder _builder;
        private readonly string _fileOutputPath = null;
        private double _totalMilliseconds = 0;

        private readonly IDictionary<string, BenchmarkResult> _results = new Dictionary<string, BenchmarkResult>();

        public Benchmarker(string fileOutputPath) : this()
        {
            _fileOutputPath = fileOutputPath;
        }

        public Benchmarker(TextWriter writer) : this()
        {
            _writer = writer;
        }

        public Benchmarker()
        {
            if (_writer == null)
                _writer = Console.Out;
        }

        public double WarmUpPercentage = 0.05;

        public bool CopyResultsToClipboard { get; set; }

        public void AddTest(string title, Action test)
        {
            _tests.Add(new BenchmarkItem { Title = title, Test = test });
        }

        public void RunTests(TimeSpan totalTime)
        {
            RunWarmUp(1);

            InternalRun(null, totalTime);
        }

        public void RunTests(int minutes, int seconds)
        {
            RunTests(TimeSpan.FromSeconds((minutes * 60) + seconds));
        }

        public void RunTests(int num = 100000)
        {
            RunWarmUp(num);

            InternalRun(num, null);
        }

        private void RunWarmUp(int totalNum)
        {
            // WARM-UP
            // run thru each of them once because otherwise the first loop is slower due to the Just In Time compilation
            var numWarmups = Math.Ceiling(totalNum * WarmUpPercentage);

            for (var i = 0; i < numWarmups; i++)
            {
                foreach (var test in _tests)
                {
                    test.Test();
                }
            }
        }

        private void InternalRun(int? totalTests, TimeSpan? totalTime)
        { 
            _builder = new StringBuilder();

            WriteLine();

            if (totalTests.HasValue)
            {
                WriteLine(String.Format("Running each test {0:#,0} times", totalTests));
            }
            else
            {
                WriteLine(String.Format("Running tests for {0}", totalTime));
            }

            WriteLine();
            WriteLine("----------------------------------------------------");
            WriteLine("Running tests: ");

            if (totalTests.HasValue)
            {
                for (var i = 0; i < _tests.Count; i++)
                {
                    WriteLine(String.Format("   Pass {0}...", i + 1));
                    InternalSingleRun(totalTests.Value/_tests.Count);
                }
            }
            else if (totalTime.HasValue)
            {
                // the batch size will get adjusted based on how long they are taking to process
                //  we need to start with 1 and then increase based on their speed
                var batchSize = 1;
                long totalTimesRan = 0;
                
                while (_totalMilliseconds < totalTime.Value.TotalMilliseconds)
                {
                    InternalSingleRun(batchSize);
                    totalTimesRan += batchSize;

                    // adjust batch size based on the current pace
                    var percentDone = _totalMilliseconds/totalTime.Value.TotalMilliseconds;
                    var estimatedMoreRuns = totalTimesRan/percentDone;

                    // let's be safe and try to split it up into a batch for each test so it gets to run in various order
                    batchSize = (int)Math.Floor(estimatedMoreRuns / _tests.Count+1);

                    WriteLine(String.Format("   Running new batch of {0:#,0} tests", batchSize));
                }

                WriteLine();
                WriteLine(String.Format("   Ran {0:#,0} times in time allotted", totalTimesRan));
                WriteLine(String.Format("   Took a total of {0}", TimeSpan.FromMilliseconds(_totalMilliseconds)));
            }

            DisplayResults();

            var resultsText = _builder.ToString();

            // write to the file if there is one
            if (!String.IsNullOrEmpty(_fileOutputPath))
                File.WriteAllText(_fileOutputPath, resultsText);

            if (CopyResultsToClipboard)
            {
                OpenClipboard(IntPtr.Zero);
                var ptr = Marshal.StringToHGlobalUni(resultsText);
                SetClipboardData(13, ptr);
                CloseClipboard();
                Marshal.FreeHGlobal(ptr);
            }
        }

        private void InternalSingleRun(int num)
        {
            var sw = new Stopwatch();
            
            foreach (var benchmarkItem in _tests.OrderBy(x => Guid.NewGuid()))
            {
                sw.Reset();
                sw.Start();

                for (var i = 0; i < num; i++)
                {
                    benchmarkItem.Test();
                }
                sw.Stop();

                AddResult(benchmarkItem, sw.Elapsed.TotalMilliseconds, num);
            }
        }

        private void DisplayResults()
        {
            WriteLine();
            WriteLine("Results: (fastest first)");

            foreach (var item in _results.Values.OrderBy(x => x.TotalTime))
            {
                WriteLine(item.Display);
            }

            WriteLine();
        }

        private void AddResult(BenchmarkItem item, double totalTime, int timesRun)
        {
            if (!_results.ContainsKey(item.Title))
            {
                _results.Add(item.Title, new BenchmarkResult { Title = item.Title, TotalTime = 0.0, TimeRun = 0});
            }

            var result = _results[item.Title];
            result.TimeRun += timesRun;
            result.TotalTime += totalTime;
            _totalMilliseconds += totalTime;
        }


        private void WriteLine(string text = "")
        {
            Write(text + "\r\n");
        }

        private void Write(string text = "")
        {
            _writer.Write(text);
            _builder.Append(text);
        }

        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);
    }
}

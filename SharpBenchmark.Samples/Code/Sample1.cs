using System.Threading;

namespace SharpBenchmark.Samples.Code
{
    class Sample1 : Sample
    {
        public override void Execute(string[] args)
        {
            AddTest("Increment using lock", IncrementUsingLock);
            AddTest("Interlocked Increment", InterlockedIncrement);

            RunTests(5000000);
        }

        private readonly object _lock = new object();
        private int _int1 = 1;
        private int _int2 = 1;

        public void IncrementUsingLock()
        {
            lock (_lock)
            {
                _int1++;
            }
        }

        public void InterlockedIncrement()
        {
            Interlocked.Increment(ref _int2);
        }
    }
}

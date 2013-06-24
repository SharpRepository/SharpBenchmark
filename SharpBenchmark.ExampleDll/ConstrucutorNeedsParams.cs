using System.Threading;

namespace SharpBenchmark.ExampleDll
{
    public class ConstrucutorNeedsParams
    {
        private readonly int _delay;
        public ConstrucutorNeedsParams(int delay)
        {
            _delay = delay;
        }

        public void Algorithm1()
        {
            Thread.Sleep(_delay);
        }

        public void Algorithm2(string name)
        {
            Thread.Sleep(_delay);
        }
    }
}

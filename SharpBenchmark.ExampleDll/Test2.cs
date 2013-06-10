using System.Threading;

namespace SharpBenchmark.ExampleDll
{
    public class Test2
    {
        public void Algorithm1(int delay)
        {
            Thread.Sleep(delay);
        }

        public void Algorithm2(int delay)
        {
            Thread.Sleep(delay);
        }

        public static void StaticAlgorithm(int delay)
        {
            Thread.Sleep(delay);
        }
    }
}
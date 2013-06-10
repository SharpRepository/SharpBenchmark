using System.Threading;

namespace SharpBenchmark.ExampleDll
{
    public static class TestStatic
    {
        public static void StaticAlgorithm(int delay, string name, bool isSomething)
        {
            Thread.Sleep(delay);
        }
    }
}

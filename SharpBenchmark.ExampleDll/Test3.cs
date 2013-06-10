using System;
using System.Threading;

namespace SharpBenchmark.ExampleDll
{
    public class Test3
    {
        public void Algorithm1(int delay, string name, DateTime date)
        {
            Thread.Sleep(delay);
        }

        public void Algorithm2(int delay, string name, bool isSomething)
        {
            Thread.Sleep(delay);
        }

        public void Algorithm3(int delay, decimal price)
        {
            Thread.Sleep(delay);
        }

        public static void StaticAlgorithm(int delay, string name, bool isSomething)
        {
            Thread.Sleep(delay);
        }
    }
}

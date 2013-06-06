using System.Threading;

namespace SharpBenchmark.ExampleDll
{
    public class Test1
    {
        public void Algorithm1()
        {
            Thread.Sleep(125);
        }

        public void Algorithm2()
        {
            Thread.Sleep(235);
        }
    }

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
    }
}

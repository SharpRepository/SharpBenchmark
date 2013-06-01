using System;
using System.Linq;

namespace SharpBenchmark.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var allSamples = typeof(ISample).Assembly.GetTypes();

            while (true)
            {
                Console.Write("Which sample would you like to run?  (enter q to quit): ");
                var input = Console.ReadLine();

                if (input == "q" || input == null)
                    break;

                var parts = input.Split(' ');
                var sampleNum = 0;
                if (!Int32.TryParse(parts[0], out sampleNum))
                {
                    Console.WriteLine("Please enter an integer.  Try again.");
                    continue;
                }

                // get the problem class based on the name
                var problemType = allSamples.FirstOrDefault(x => x.Name == "Sample" + sampleNum);

                if (problemType == null)
                {
                    Console.WriteLine("Invalid sample number.  Try again.");
                    continue;
                }

                var sample = (ISample)Activator.CreateInstance(problemType);

                var tmp = parts.ToList();
                tmp.RemoveAt(0);

                sample.Execute(tmp.ToArray());
            }
        }
    }
}

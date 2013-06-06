using System;
using System.Configuration;
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
                Console.Write("Which sample would you like to run?  (enter q to quit, enter load to load a dll): ");
                var input = Console.ReadLine();

                if (input == "q" || input == null)
                    break;

                if (input == "load")
                {
                    LoadDll();
                    continue;
                }

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

        static void LoadDll()
        {
            Console.WriteLine("Full Path to DLL (or blank to use testing DLL): ");
            var path = Console.ReadLine();
            //Console.WriteLine("Full Path to DLL: ");
            if (String.IsNullOrEmpty(path))
                path = @"C:\Projects\SharpBenchmark\SharpBenchmark.ExampleDll\bin\Debug\SharpBenchmark.ExampleDll.dll";

            if (!String.IsNullOrEmpty(path))
            {
                var dll = System.Reflection.Assembly.LoadFrom(path);

                var types = dll.GetTypes();

                Console.WriteLine("Types in the DLL");
                Console.WriteLine("-------------------------");

                var i = 1;
                foreach (var type in types)
                {
                    Console.WriteLine("{0}) {1} ", i, type.FullName);

                    i++;
                }

                Console.WriteLine();
                Console.WriteLine("Enter the number of the type you'd like to use: ");
                var input = Console.ReadLine();

                var typeNum = Int32.Parse(input);
                var selectedType = types[typeNum - 1];

                Console.WriteLine();
                Console.WriteLine("Methods in this type");
                Console.WriteLine("-------------------------");

                // get all public methods
                i = 1;
                var methods = selectedType.GetMethods();
                foreach (var method in methods)
                {
                    Console.Write("{0}) {1}(", i, method.Name);

                    foreach (var pi in method.GetParameters())
                    {
                        Console.Write("{0} {1}, ", pi.ParameterType, pi.Name);
                    }

                    Console.WriteLine(")");
                    i++;
                }

                Console.WriteLine();
                Console.WriteLine("Enter the number(s) of the method you'd like to use: ");
                input = Console.ReadLine();

                var benchmarker = new Benchmarker();

                foreach (var methodNum in input.Split(',').Select(x => Int32.Parse(x.Trim())))
                {
                    var selectedMethod = methods[methodNum - 1];

                    // create an action with this method 
                    // TODO: make this work with methods that have parameters, right now it won't
                    var newType = Activator.CreateInstance(selectedType);
                    var del = (Action)Delegate.CreateDelegate(typeof(Action), newType, selectedMethod);

                    benchmarker.AddTest(selectedMethod.Name, del);
                }
                    
                benchmarker.RunTests(20);
            }
        }
    }
}

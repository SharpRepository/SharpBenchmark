using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SharpBenchmark.AssemblyHelper;

namespace SharpBenchmark.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var allSamples = typeof(ISample).Assembly.GetTypes();

            while (true)
            {
                Console.Write("Which sample would you like to run?  (enter q to quit, enter dll to load a dll): ");
                var input = Console.ReadLine();

                if (input == "q" || input == null)
                    break;

                if (input == "dll")
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

            if (String.IsNullOrEmpty(path)) return;

            var dll = Assembly.LoadFrom(path);
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
            var methods = selectedType.GetMethods(BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                Console.Write("{0}) {1}(", i, method.Name);

                var piNum = 1;
                foreach (var pi in method.GetParameters())
                {
                    Console.Write("{0}{1} {2}", piNum != 1 ? "," : "", pi.ParameterType, pi.Name);
                    piNum++;
                }

                Console.WriteLine(")");
                i++;
            }

            Console.WriteLine();
            Console.Write("Enter the number(s) of the methods you'd like to use (comma separated): ");
            input = Console.ReadLine();
            Console.WriteLine();

            var benchmarker = new Benchmarker();

            foreach (var methodNum in input.Split(',').Select(x => Int32.Parse(x.Trim())))
            {
                var selectedMethod = methods[methodNum - 1];

                var parameters = selectedMethod.GetParameters();

                var paramValues = new List<object>();
                foreach (var parameter in parameters)
                {
                    Console.Write("  Enter a value for the parameter {0} {1}: ", parameter.ParameterType, parameter.Name);
                    var paramInput = Console.ReadLine();
                    Console.WriteLine();

                    paramValues.Add(Convert.ChangeType(paramInput, parameter.ParameterType));
                }

                // call the delegate from my action with the proper parameters
                benchmarker.AddTest(selectedMethod.Name, ActionHelper.BuildTestAction(selectedType, selectedMethod, paramValues.ToArray()));
            }

            //benchmarker.AddTest("Hard-coded", () => new Test3().Algorithm2(125, "Test", true));
                    
            benchmarker.RunTests(20);
        }

       
    }
}

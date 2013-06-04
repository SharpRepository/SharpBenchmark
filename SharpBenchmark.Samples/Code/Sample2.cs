using System;
using System.Collections.Generic;
using System.Linq;
namespace SharpBenchmark.Samples.Code
{
    class Sample2 : Sample
    {
        public override void Execute(string[] args)
        {
            AddTest("new TestClass()", () => NewTestClass());
            AddTest("Activator.CreateInstance<TestClass>()", () => ActivatorCreateInstance());
            AddTest("new T()", () => NewTestClassByType<TestClass>());
            AddTest("Activator.CreateInstance<T>()", () => ActivatorCreateInstanceGeneric<TestClass>());
            AddTest("Activator.CreateInstance(type)", () => ActivatorCreateInstanceType(typeof (TestClass)));

            RunTests(2000000);
        }

        private static TestClass NewTestClass()
        {
            return new TestClass();
        }

        private static TestClass ActivatorCreateInstance()
        {
            return Activator.CreateInstance<TestClass>();
        }

        private static T NewTestClassByType<T>() where T : new()
        {
            return new T();
        }

        private static T ActivatorCreateInstanceGeneric<T>()
        {
            return Activator.CreateInstance<T>();
        }

        private static object ActivatorCreateInstanceType(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public class TestClass
        {
            public int Id { get; set; }

            public TestClass()
            {
                
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpBenchmark.Wpf.Models
{
    public class AssemblyExplorerItem
    {
        public AssemblyExplorerItem(string path)
        {
            var dll = Assembly.LoadFrom(path);

            Name = dll.FullName;
            Classes = dll.GetTypes().Select(x => new ClassItem(x)).ToList();
        }

        public string Name { get; set; }        
        public IList<ClassItem> Classes { get; set; } 

        // for the tree view
        public IList<ClassItem> Children { get { return Classes; } }
        public string Display
        {
            get { return Name; }
        }

        public class ClassItem
        {
            public ClassItem(Type type)
            {
                Type = type;
                Name = type.FullName;

                Constructors = type.GetConstructors().Select(x => new ConstructorItem(x)).ToList()  ;

                Methods = type.GetMethods().Where(x => x.DeclaringType != typeof(object)).Select(x => new MethodItem(Type, x)).ToList();
            }

            public string Name { get; set; }
            public Type Type { get; set; }

            public IList<ConstructorItem> Constructors { get; set; }
            public IList<MethodItem> Methods { get; set; }

            // for tree view
            public IList<MethodItem> Children { get { return Methods; } }
            public string Display
            {
                get { return Name; }
            }
        }

        public class MethodItem
        {
            public MethodItem(Type instanceType, MethodInfo methodInfo)
            {
                MethodInfo = methodInfo;
                InstanceType = instanceType;
                Name = MethodInfo.Name;

                Parameters = MethodInfo.GetParameters().Select(x => new ParameterItem(x)).ToList();
            }

            public MethodInfo MethodInfo { get; set;  }
            public Type InstanceType { get; set; }
            public string Name { get; set; }

            public IList<ParameterItem> Parameters { get; set; }

            // for tree view
            //public IList<MethodItem> Children { get { return Methods; } }
            public string Display
            {
                get
                {
                    var parameters = String.Join(",", Parameters.Select(x => String.Format("{0} {1}", x.TypeName, x.Name)));

                    return String.Format("{0}({1})", Name, parameters);
                }
            }
        }

        public class ConstructorItem
        {
            private readonly ConstructorInfo _constructorInfo;

            public ConstructorItem(ConstructorInfo constructorInfo)
            {
                _constructorInfo = constructorInfo;

                Parameters = _constructorInfo.GetParameters().Select(x => new ParameterItem(x)).ToList();
            }

            public IList<ParameterItem> Parameters { get; set; }
        }

        public class ParameterItem
        {
            public ParameterItem(ParameterInfo parameterInfo)
            {
                Name = parameterInfo.Name;
                Type = parameterInfo.ParameterType;
                TypeName = Type.Name;
            }

            public string Name { get; set; }
            public Type Type { get; set; }
            public string TypeName { get; set; }

            public string InputValue { get; set; }
        }
    }
}

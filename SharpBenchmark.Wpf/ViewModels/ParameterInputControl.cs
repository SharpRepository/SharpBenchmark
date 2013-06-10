using System;
using System.Collections.Generic;
using System.Reflection;
using Caliburn.Micro;
using SharpBenchmark.Wpf.Models;

namespace SharpBenchmark.Wpf.ViewModels
{
    public class ParameterInputControl : PropertyChangedBase
    {
        private readonly AssemblyExplorerItem.MethodItem _methodItem;

        public ParameterInputControl(AssemblyExplorerItem.MethodItem methodItem)
        {
            _methodItem = methodItem;
            MethodInfo = _methodItem.MethodInfo;
            InstanceType = _methodItem.InstanceType;
        }

        public MethodInfo MethodInfo { get; set; }

        public Type InstanceType { get; set; }

        public string Name
        {
            get { return _methodItem.Name; }
        }

        public string Display
        {
            get { return _methodItem.Display; }
        }

        public IList<AssemblyExplorerItem.ParameterItem> Parameters
        {
            get { return _methodItem.Parameters; }
        }
    }
}

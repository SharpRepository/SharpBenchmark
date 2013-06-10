using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using SharpBenchmark.AssemblyHelper;
using SharpBenchmark.Wpf.Models;
using SharpBenchmark.Wpf.ViewModels;

namespace SharpBenchmark.Wpf 
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        private string _assemblyFilePath;
        public string AssemblyFilePath
        {
            get { return _assemblyFilePath; }
            set
            {
                _assemblyFilePath = value;
                NotifyOfPropertyChange(() => AssemblyFilePath);
            }
        }

        public IList<string> ComboValueType
        {
            get { return new List<string> { "Seconds", "Iterations"};}
        }

        private string _selectedComboValueType = "Seconds";
        public string SelectedComboValueType
        {
            get { return _selectedComboValueType; }
            set 
            {
                _selectedComboValueType = value;
                NotifyOfPropertyChange(() => SelectedComboValueType);
            }
        }

        private string _testConfigValue = "30";
        public string TestConfigValue
        {
            get { return _testConfigValue; }
            set 
            { 
                _testConfigValue = value;
                NotifyOfPropertyChange(() => TestConfigValue);
            }
        }

        public IList<AssemblyExplorerItem> AssemblyExplorerItems
        {
            get { return GetTreeViewItems(); }
        }

        private IList<AssemblyFilePath> _loadedAssemblyFiles = Repositories.AssemblyFileRepository.GetAll().ToList();
        public IList<AssemblyFilePath> LoadedAssemblyFiles
        {
            get { return _loadedAssemblyFiles; }
            set
            {
                _loadedAssemblyFiles = value;
                NotifyOfPropertyChange(() => LoadedAssemblyFiles);
                NotifyOfPropertyChange(() => AssemblyExplorerItems);
            }
        }

        public void OpenFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
                                     {
                                         FileName = "Assembly File", 
                                         DefaultExt = ".dll",
                                         Filter = "Assembly files (.dll)|*.dll"
                                     };

            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                AssemblyFilePath = openFileDialog.FileName;

                Repositories.AssemblyFileRepository.Add(new AssemblyFilePath { Path = AssemblyFilePath});
            }
        }

        public void ReloadTree()
        {
            _loadedAssemblyFiles = Repositories.AssemblyFileRepository.GetAll().ToList();
        }

        private IList<AssemblyExplorerItem> GetTreeViewItems()
        {
            return _loadedAssemblyFiles.Select(x => new AssemblyExplorerItem(x.Path)).ToList();
        }

        public void SelectedItem(object item)
        {
            var methodItem = item as AssemblyExplorerItem.MethodItem;
            if (methodItem != null)
            {
                _parameterInputControls.Add(new ParameterInputControl(methodItem));
            }
        }

        private ObservableCollection<ParameterInputControl> _parameterInputControls = new ObservableCollection<ParameterInputControl>();
        public ObservableCollection<ParameterInputControl> ParameterInputControls
        {
            get { return _parameterInputControls; }
        }

        private string _resultDisplay;
        public string ResultDisplay
        {
            get { return _resultDisplay; }
            set
            {
                _resultDisplay = value;
                NotifyOfPropertyChange(() => ResultDisplay);
            }
        }

        public async void RunTests()
        {
            ResultDisplay = String.Format("Running tests for {0} {1} ...", TestConfigValue, SelectedComboValueType);


            var  memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            var benchmarker = new Benchmarker(tw);

            foreach (var input in _parameterInputControls)
            {
                var args = input.Parameters.Select(x => Convert.ChangeType(x.InputValue, x.Type)).ToArray();

                benchmarker.AddTest(input.Display, ActionHelper.BuildTestAction(input.InstanceType, input.MethodInfo, args));
            }

            if (SelectedComboValueType == "Iterations")
            {
                await Task.Run(() => benchmarker.RunTests(Int32.Parse(TestConfigValue)));     
            }
            else
            {
                await Task.Run(() => benchmarker.RunTests(TimeSpan.FromSeconds(Int32.Parse(TestConfigValue))));     
            }
           

            
            tw.Close();
            memoryStream.Close();

            var resultsText = Encoding.UTF8.GetString(memoryStream.ToArray());
            
            tw.Dispose();

            ResultDisplay = resultsText;

            //_windowManager.ShowWindow(new ResultsViewModel(resultsText));
        }
    }
}
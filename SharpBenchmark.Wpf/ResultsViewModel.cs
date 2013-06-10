namespace SharpBenchmark.Wpf
{
    public class ResultsViewModel
    {
        public ResultsViewModel(string results)
        {
            Display = results;
        }

        public string Display { get; set; }
    }
}

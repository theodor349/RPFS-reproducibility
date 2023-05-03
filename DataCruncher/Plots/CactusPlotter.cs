using DataCruncher.Utilities;
using Models;
using System;

namespace DataCruncher.Plots
{
    internal class CactusPlotter : BasePlotter
    {
        private ListCutoffExtractor _cutoffer = new ListCutoffExtractor();

        public void Plot(List<Execution> executions, string name, string folder, List<string> strategies, Converter<Execution, double> converter, int cutoff, bool shrink, int shrinkDigits)
        {
            var solvedExecutions = executions.Where(x => x.Solved).ToList();
            var data = GenerateData(solvedExecutions, strategies, converter);
            int indexOffset = _cutoffer.GetCutoff(data.ToList().ConvertAll(x => x.Value), cutoff);
            Print(folder, $"{name}", GenerateCactusPlot("", data, indexOffset, true, shrink, shrinkDigits));
        }

        private Dictionary<string, List<double>> GenerateData(List<Execution> executions, List<string> strategies, Converter<Execution, double> converter)
        {
            var data = new Dictionary<string, List<double>>();
            foreach (var strat in strategies)
            {
                var targetExecutions = executions.Where(x => x.Strategy.Equals(strat)).ToList();
                var points = targetExecutions.ConvertAll(x => converter(x));
                data.Add(strat, points);
            }
            return data;
        }
    }
}

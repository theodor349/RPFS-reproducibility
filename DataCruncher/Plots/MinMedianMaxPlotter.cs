using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Plots
{
    internal class MinMedianMaxPlotter : BasePlotter
    {
        public void Plot(List<Execution> executions, string median, string folder, string plotName)
        {
            var medianExecutions = executions.Where(x => x.Strategy == median);

            var data = new Dictionary<string, List<double>>();
            data = GenerateData(executions, medianExecutions);
            Print(folder, $"{plotName}", GenerateCactusPlot("", data, 0, false, false, 0, true));
        }

        private Dictionary<string, List<double>> GenerateData(List<Execution> executions, IEnumerable<Execution> medianExecutions)
        {
            executions = executions.Where(x => x.Solved).ToList();
            var medianInstances = medianExecutions.Where(x => x.Solved).ToList().ConvertAll(x => x.Instance);
            var strategy = medianExecutions.First().Strategy;

            var executionLookup = new Dictionary<string, List<Execution>>();
            foreach (var execution in executions)
            {
                if (executionLookup.ContainsKey(execution.Instance))
                    executionLookup[execution.Instance].Add(execution);
                else
                    executionLookup.Add(execution.Instance, new List<Execution>() { execution });
            }

            var data = new List<Tuple<double, double, double>>();
            foreach (var instance in medianInstances)
            {
                var execs = executionLookup[instance];
                var min = execs.Min(x => x.Time);
                var median = execs.First(x => x.Strategy == strategy).Time;
                var max = execs.Max(x => x.Time);

                var t = new Tuple<double, double, double>(min, median, max);
                data.Add(t);
            }
            data.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            var diviation = new List<double>();
            var minList = new List<double>();
            var medianList = new List<double>();
            var maxList = new List<double>();
            foreach (var datapoint in data)
            {
                minList.Add(datapoint.Item1);
                medianList.Add(datapoint.Item2);
                maxList.Add(datapoint.Item3);
                diviation.Add(datapoint.Item3 - datapoint.Item2);
                diviation.Add(datapoint.Item2 - datapoint.Item1);
            }

            var avgDiviation = diviation.Average();
            Printer.PrintLine(" - Average Diviation from the median is for {} is {} seconds", medianExecutions.First().Strategy, avgDiviation);
            var res = new Dictionary<string, List<double>>()
            {
                { "Min", minList },
                { "Max", maxList },
                { "Median", medianList },
            };
            return res;
        }
    }
}

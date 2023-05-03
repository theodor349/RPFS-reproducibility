using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Plots
{
    internal class ThroughputPlotter : BasePlotter
    {
        public List<ThroughputStatistic> Plot(List<Execution> executions, string plotName, string folder, List<string> strategies, bool sortByInstance, bool shrink, int shrinkDigits)
        {
            executions = KeepSolvedExecutionsWhichEvaluatedToFalse(executions);
            executions = KeepInstancesAllStrategiesSolved(executions, strategies);

            var data = GenerateData(executions, strategies, sortByInstance);
            Print(folder, $"{plotName}_throughput", GenerateCactusPlot("", data.Item1, 0, !sortByInstance, shrink, shrinkDigits));

            return data.Item2;
        }

        private static List<Execution> KeepSolvedExecutionsWhichEvaluatedToFalse(List<Execution> executions)
        {
            return executions.Where(x => x.Solved && x.Result == ExecutionResult.False).ToList();
        }

        private Tuple<Dictionary<string, List<double>>, List<ThroughputStatistic>> GenerateData(List<Execution> executions, List<string> strategies, bool sortByInstance)
        {
            var throughputs = new Dictionary<string, Dictionary<string, double>>();
            var stats = new List<ThroughputStatistic>();
            foreach (var strategy in strategies)
            {
                var data = GenerateThroughput(executions.Where(x => x.Strategy == strategy).ToList());
                throughputs.Add(strategy, data);
                stats.Add(new ThroughputStatistic(strategy, new SortedDictionary<string, double>(data)));
            }

            var res = SortByInstance(throughputs);

            return new Tuple<Dictionary<string, List<double>>, List<ThroughputStatistic>>(res, stats);
        }

        private Dictionary<string, List<double>> SortByInstance(Dictionary<string, Dictionary<string, double>> throughputs)
        {
            var bestStrat = GetBestStrat(throughputs);
            Printer.PrintLine(" - Best Throughput strategy: {}", bestStrat);
            var sortedInstances = GetSortedInstances(throughputs[bestStrat]);
            var res = ConvertToListOfThroughputs(throughputs, sortedInstances);
            return res;
        }

        private static Dictionary<string, List<double>> ConvertToListOfThroughputs(Dictionary<string, Dictionary<string, double>> throughputs, List<string> sortedInstances)
        {
            var res = new Dictionary<string, List<double>>();
            foreach (var throughput in throughputs)
            {
                var values = new List<double>();
                foreach (var instance in sortedInstances)
                    values.Add(throughput.Value[instance]);
                res.Add(throughput.Key, values);
            }

            return res;
        }

        private List<string> GetSortedInstances(Dictionary<string, double> data)
        {
            var sortedData = new List<Tuple<string, double>>(data.Count);
            foreach (var entry in data)
                sortedData.Add(new Tuple<string, double>(entry.Key, entry.Value));
            sortedData.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            return sortedData.ToList().ConvertAll(x => x.Item1);
        }

        private string GetBestStrat(Dictionary<string, Dictionary<string, double>> throughputs)
        {
            string bestStrat = "";
            double maxThroughput = 0;
            foreach (var strat in throughputs)
            {
                var total = strat.Value.Sum(x => x.Value);
                if (total < maxThroughput)
                    continue;

                bestStrat = strat.Key;
                maxThroughput = total;
            }
            return bestStrat;
        }

        private Dictionary<string, double> GenerateThroughput(List<Execution> executions)
        {
            var data = new Dictionary<string, double>();
            foreach (var execution in executions)
            {
                var v = (execution.ExpandedStates.Value + 1) / execution.SearchTime.Value;
                data.Add(execution.Instance, v);
            }

            return data;
        }

        private static List<Execution> KeepInstancesAllStrategiesSolved(List<Execution> executions, List<string> strategies)
        {
            var instances = new Dictionary<string, int>();
            foreach (var execution in executions)
            {
                if (instances.ContainsKey(execution.Instance))
                    instances[execution.Instance]++;
                else
                    instances.Add(execution.Instance, 1);
            }
            var solvedInstances = instances.Where(x => x.Value == strategies.Count).ToList().ConvertAll(x => x.Key);
            executions = executions.Where(x => solvedInstances.Contains(x.Instance)).ToList();
            return executions;
        }
    }
}

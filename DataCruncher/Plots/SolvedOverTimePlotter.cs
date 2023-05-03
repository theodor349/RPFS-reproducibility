using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Plots
{
    internal class SolvedOverTimePlotter : BasePlotter
    {
        public void Plot(List<Execution> executions, List<string> strategies, int incrementInSeconds, string folder, string plotName)
        {
            executions = executions.Where(x => x.Solved).ToList();
            var data = GenerateData(executions, strategies, incrementInSeconds);
            Print(folder, $"{plotName}_solvedInTime_{incrementInSeconds}", GenerateCactusPlot("", data, 0, false, false, 0));
        }

        private Dictionary<string, List<Tuple<double, int>>> GenerateData(List<Execution> executions, List<string> strategies, int incrementInSeconds)
        {
            var res = new Dictionary<string, List<Tuple<double, int>>>();
            var times = GetTimes(executions, strategies);
            foreach (var strat in times)
            {
                var points = new List<Tuple<double, int>>();
                //points.Add(new Tuple<double, int>(0, 0));
                for (int i = 0; i < strat.Value.Count; i++)
                {
                    double time = strat.Value[i];
                    points.Add(new Tuple<double, int>(time, i + 1));
                }
                res.Add(strat.Key, points);
            }
            return res;
        }

        private Dictionary<string, List<double>> GetTimes(List<Execution> executions, List<string> strategies)
        {
            var data = new Dictionary<string, List<double>>();
            foreach (var strat in strategies)
            {
                var targetExecutions = executions.Where(x => x.Strategy.Equals(strat)).ToList();
                var points = targetExecutions.ConvertAll(x => x.Time);
                data.Add(strat, points);
            }
            return data;
        }
    }
}

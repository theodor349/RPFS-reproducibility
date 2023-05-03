using DataCruncher.Tables;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Utilities
{
    internal class ExecutionAggregator
    {
        public string Aggregate(ref List<Execution> executions, string newName, List<string> includes, List<string> excludes)
        {
            var statsEngine = new StatsGenerator();

            var temp = executions.Where(IsTargetStrat(includes, excludes)).ToList();
            var stats = statsEngine.GenerateStats(temp);
            executions.RemoveAll(x => IsTargetStrat(includes, excludes)(x));

            if (stats.Count == 0)
                return "";

            stats.Sort();
            string medianStrat = stats[(int)Math.Floor(stats.Count / 2.0)].Strategy;
            var median = temp.Where(x => x.Strategy.Equals(medianStrat)).ToList();
            UpdateName(newName, median);
            executions.AddRange(median);

            Printer.PrintLine(" - Median for {}  is {}", newName, medianStrat);
            return medianStrat;
        }

        public void UpdateName(string newName, IEnumerable<Execution> median)
        {
            foreach (var execution in median)
            {
                execution.Strategy = newName;
            }
        }

        private static Func<Execution, bool> IsTargetStrat(List<string> includes, List<string> excludes)
        {
            return x =>
                            includes.TrueForAll(n => x.Strategy.Contains(n)) &&
                            excludes.TrueForAll(n => !x.Strategy.Contains(n));
        }
    }
}

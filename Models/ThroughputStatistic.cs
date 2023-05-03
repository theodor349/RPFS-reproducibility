using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ThroughputStatistic
    {
        public string Strategy { get; set; }
        public SortedDictionary<string, double> Throughputs { get; set; }

        public double TotalThroughput => Throughputs.Sum(x => x.Value);
        public double AvgThroughput => TotalThroughput / Throughputs.Count;

        public ThroughputStatistic(string strategy, SortedDictionary<string, double> throughputs)
        {
            Strategy = strategy;
            Throughputs = throughputs;
        }
    }
}

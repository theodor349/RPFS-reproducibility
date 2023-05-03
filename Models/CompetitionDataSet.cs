using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CompetitionDataSet
    {
        public CompetitionEntry Cardinality { get; set; }
        public CompetitionEntry Fireability { get; set; }

        public double TotalFireability = 21136;
        public double TotalCardinality => TotalFireability;
        public double Total => TotalFireability + TotalCardinality;
    }

    public class CompetitionEntry
    {
        public CompetitionEntry(int bestFS, int rpfs)
        {
            BestFS = bestFS;
            Rpfs = rpfs;
        }

        public int BestFS { get; set; }
        public int Rpfs { get; set; }
    }
}

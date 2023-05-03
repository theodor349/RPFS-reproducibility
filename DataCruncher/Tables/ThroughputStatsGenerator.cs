using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Tables
{
    internal class ThroughputStatsGenerator
    {
        public void PrintStats(List<ThroughputStatsGenerator> stats, string folder, string tableName)
        {
            var table = GenerateTable(stats);
            File.WriteAllText(Path.Combine(folder, tableName + "_ThroughputStats.tab"), table);
        }

        private string GenerateTable(List<ThroughputStatsGenerator> stats)
        {

            return "";
        }
    }
}

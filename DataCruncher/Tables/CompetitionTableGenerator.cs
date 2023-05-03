using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Tables
{
    internal class CompetitionTableGenerator
    {
        public void Print(string folder, string tableName, CompetitionDataSet data) 
        {
            var table = new StringBuilder();
            PrintHeader(table);
            PrintValues(table, data);

            File.WriteAllText(Path.Combine(folder, tableName + ".tab"), table.ToString());
        }

        private void PrintValues(StringBuilder table, CompetitionDataSet data)
        {
            PrintHLine(table);
            PrintCardinality(table, data);
            PrintHLine(table);
            PrintFireability(table, data);
            PrintHLine(table);
            PrintTotal(table, data);
            PrintHLine(table);
        }

        private void PrintHLine(StringBuilder table)
        {
            table.AppendLine("\t\\hline");
        }

        private void PrintTotal(StringBuilder table, CompetitionDataSet data)
        {
            var bestFsTotal = data.Fireability.BestFS + data.Cardinality.BestFS;
            var rpfsTotal = data.Fireability.Rpfs + data.Cardinality.Rpfs;

            var values = new List<string>()
            {
                "Total",
                bestFsTotal.ToString(),
                rpfsTotal.ToString(),
                (bestFsTotal / data.Total * 100).ToString("0.##") + @" \%",
                (rpfsTotal / data.Total * 100).ToString("0.##") + @" \%",
            };
            PrintLine(table, values);
        }

        private void PrintFireability(StringBuilder table, CompetitionDataSet data)
        {
            var values = new List<string>()
            {
                "Fireability",
                data.Fireability.BestFS.ToString(),
                data.Fireability.Rpfs.ToString(),
                (data.Fireability.BestFS / data.TotalFireability * 100).ToString("0.##") + @" \%",
                (data.Fireability.Rpfs / data.TotalFireability * 100).ToString("0.##") + @" \%",
            };
            PrintLine(table, values);
        }

        private void PrintCardinality(StringBuilder table, CompetitionDataSet data)
        {
            var values = new List<string>()
            {
                "Cardinality",
                data.Cardinality.BestFS.ToString(),
                data.Cardinality.Rpfs.ToString(),
                (data.Cardinality.BestFS / data.TotalCardinality * 100).ToString("0.##") + @" \%",
                (data.Cardinality.Rpfs / data.TotalCardinality * 100).ToString("0.##") + @" \%",
            };
            PrintLine(table, values);
        }

        private static void PrintLine(StringBuilder table, List<string> values)
        {
            table.Append("\t");
            for (int i = 0; i < values.Count; i++)
            {
                if (i > 0)
                    table.Append(" & ");
                table.Append(values[i]);
            }
            table.AppendLine(" \\\\");
        }

        private void PrintHeader(StringBuilder table)
        {
            var headers = new List<string>()
            {
                "",
                "BestFS",
                "RPFS",
                @"BestFS \%",
                @"RPFS \%",
            };

            for (int i = 0; i < headers.Count; i++)
            {
                if (i > 0)
                    table.Append(" & ");
                table.Append("\\textbf{" + headers[i] + "}");
            }
            table.AppendLine(" \\\\");
        }
    }
}

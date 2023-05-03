using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Tables
{
    internal class StatsGenerator
    {
        public void PrintStats(List<Execution> executions, string folder, string tableName)
        {
            var stats = GenerateStats(executions);
            var table = GenerateTable(stats);
            File.WriteAllText(Path.Combine(folder, tableName + ".tab"), table);
        }
        public void PrintStats(List<Execution> executions, string folder, string tableName, List<Execution> uniqueExecutions)
        {
            var stats = GenerateStats(executions);
            var uStats = GenerateStats(uniqueExecutions);
            var table = GenerateTable(stats, uStats.ConvertAll(x => new Tuple<string, int>(x.Strategy, x.UniqueQueries)));
            File.WriteAllText(Path.Combine(folder, tableName + ".tab"), table);
        }

        private string GenerateTable(List<StatsEntry> stats, List<Tuple<string, int>> uniques = null)
        {
            if (uniques is null)
                uniques = new List<Tuple<string, int>>();
            var table = AddHeader(uniques.Count > 0);
            AddData(stats, table, uniques);
            return table.ToString();
        }

        private static void AddData(List<StatsEntry> stats, StringBuilder table, List<Tuple<string, int>> uniques)
        {
            bool printAllUnique = uniques.Count > 0;

            foreach (var stat in stats)
            {
                var us = uniques.FirstOrDefault(x => x.Item1.Equals(stat.Strategy));
                table.AppendLine("\t\\hline");
                table.Append(
                    $"\t" +
                    $"{stat.Strategy} & " +
                    $"{stat.TotalQueries} & " +
                    $"{stat.SolvedQueries} & " +
                    $@"{(stat.SolvedQueries / (stat.TotalQueries * 1d) * 100).ToString("0.#", CultureInfo.GetCultureInfo("en-US"))} \% & " +
                    $"{stat.TimeoutQueries} & ");
                if (stat.Strategy.Contains(Execution.VirtualBestName))
                {
                    table.AppendLine(
                        $"- & " +
                        $"- & " +
                        $"- " +
                        $"\\\\");
                }
                else
                {
                    table.Append(
                        $"{stat.TotalErrors} & " +
                        $"{stat.Fastestnstances} & ");
                    if(printAllUnique)
                    {
                        if(us is not null)
                            table.Append($"{us.Item2} & ");
                        else 
                            table.Append($"- & ");
                    }
                    table.AppendLine(
                        $"{stat.UniqueQueries} " +
                        $"\\\\");

                }
            }
            table.AppendLine("\t\\hline");
        }

        private static StringBuilder AddHeader(bool uniqueAll)
        {
            var headers = new List<string>()
            {
                "Strategy",
                "Total",
                "Solved",
                @"Solved \%",
                "Timeouts",
                "Errors",
                "Fastest",
                "Unique",
            };
            if (uniqueAll)
                headers.Add("Unique All");

            var table = new StringBuilder();
            for (int i = 0; i < headers.Count; i++)
            {
                if (i > 0)
                    table.Append(" & ");
                table.Append("\\textbf{" + headers[i] + "}");
            }
            table.AppendLine("\\\\");
            return table;
        }

        public List<StatsEntry> GenerateStats(List<Execution> executions)
        {
            var res = new List<StatsEntry>();
            var strats = Execution.GetStrategies(executions);
            GenerateBasicStats(executions, res, strats);
            AddUniqueToStats(executions, res);
            AddFastests(executions, res);
            return res;
        }

        private void AddFastests(List<Execution> executions, List<StatsEntry> res)
        {
            var instances = new Dictionary<string, Tuple<string, double>>();
            foreach (var execution in executions)
            {
                if (execution.Strategy.Contains(Execution.VirtualBestName))
                    continue;
                if (execution.TimedOut)
                    continue;
                if (!execution.Solved)
                    continue;

                string key = execution.Instance;
                if (instances.ContainsKey(key))
                {
                    var fastest = instances[key];
                    if (fastest.Item2 > execution.Time)
                        instances[key] = new(execution.Strategy, execution.Time);
                }
                else
                    instances.Add(key, new(execution.Strategy, execution.Time));
            }

            foreach (var q in instances)
            {
                res.First(x => x.Strategy.Equals(q.Value.Item1)).Fastestnstances++;
            }
        }

        private void AddUniqueToStats(List<Execution> executions, List<StatsEntry> res)
        {
            var instances = new Dictionary<string, List<string>>();
            foreach (var execution in executions)
            {
                if (!execution.Solved)
                    continue;
                if (execution.Strategy.Contains(Execution.VirtualBestName))
                    continue;

                string key = execution.Instance;
                if (instances.ContainsKey(key))
                    instances[key].Add(execution.Strategy);
                else
                    instances.Add(key, new List<string>() { execution.Strategy });
            }

            foreach (var q in instances)
            {
                if (q.Value.Count == 1)
                    res.First(x => x.Strategy.Equals(q.Value.First())).UniqueQueries++;
            }
        }

        private void GenerateBasicStats(List<Execution> entries, List<StatsEntry> res, List<string> strats)
        {
            foreach (var strat in strats)
            {
                res.Add(GetBasicStat(entries.Where(x => x.Strategy.Equals(strat)).ToList(), strat));
            }
        }

        private StatsEntry GetBasicStat(List<Execution> executions, string strategy)
        {
            var stat = new StatsEntry();
            stat.Strategy = strategy;
            stat.TotalQueries = executions.Count;
            stat.SolvedQueries = executions.Where(x => x.Solved).Count();
            stat.TimeoutQueries = executions.Where(x => x.TimedOut && !x.Solved).Count();
            var errors = executions.Where(x => x.ExitCode != 0 && !x.TimedOut && !x.Solved);
            stat.TotalErrors = errors.Count();

            foreach (var e in executions)
            {
                if (stat.Erros.ContainsKey(e.ExitCode))
                    stat.Erros[e.ExitCode]++;
                else
                    stat.Erros.Add(e.ExitCode, 1);
            }

            if(stat.TotalErrors > 0)
            {
                Printer.PrintLine("Errors for Strat {}", strategy);
                foreach (var e in stat.Erros)
                {
                    Printer.PrintLine(" - Errorcode: {}, count: {}", e.Key, e.Value);
                }
                Printer.PrintSpace();
            }

            return stat;
        }
    }
}

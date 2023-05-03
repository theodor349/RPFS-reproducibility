using FileHelpers;

namespace Models
{
    public enum ExecutionResult { True, False, Null, Invalid }
    [DelimitedRecord("\t"), IgnoreFirst(1)]
    public class Execution : IComparable<Execution>, ICloneable
    {
        public static string VirtualBestName = "Virtual Best";

        public string Model { get; set; }
        public int Query { get; set; }
        public bool Solved { get; set; }
        public ExecutionResult Result { get; set; }
        public string Strategy { get; set; }
        public double Time { get; set; }
        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy HH.mm.ss")]
        public DateTime Date { get; set; }
        public int TimeLimit { get; set; }
        public int Memory { get; set; } // KB
        public int ExitCode { get; set; }
        public bool Formula { get; set; }
        public bool TimedOut { get; set; }
        public string? ErrorMessage { get; set; }
        public uint? DiscoveredStates { get; set; }
        public uint? ExploredStates { get; set; }
        public uint? ExpandedStates { get; set; }
        public uint? MaxTokens { get; set; }
        public double? SearchTime { get; set; }

        public bool IsSimple => Solved && (ExploredStates is null || ExploredStates == 0);
        public bool IsDifficult => !IsSimple;
        public string Instance => $"{Model}_{Query}";

        public Execution()
        {

        }

        public Execution(string model, int query, bool solved, ExecutionResult result, string strategy, double time, DateTime date, int timeLimit, int memory, int exitCode, bool formula, bool timedOut)
        {
            Model = model;
            Query = query;
            Solved = solved;
            Result = result;
            Strategy = strategy;
            Time = time;
            Date = date;
            TimeLimit = timeLimit;
            Memory = memory;
            ExitCode = exitCode;
            Formula = formula;
            TimedOut = timedOut;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static List<Execution> ParseCsv(string filepath)
        {
            var engine = new FileHelperEngine<Execution>();
            var result = engine.ReadFile(filepath);
            return result.ToList();
        }

        public static List<string> GetStrategies(List<Execution> entries)
        {
            var strats = new List<string>();
            foreach (var entry in entries)
            {
                if (!strats.Contains(entry.Strategy))
                    strats.Add(entry.Strategy);
            }
            return strats;
        }

        public int CompareTo(Execution? other)
        {
            if(Time == null)
            {
                if (other == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (other == null)
                    return 1;
                else 
                    return Time.CompareTo(other.Time);
            }
        }


        public static int GetCutoffIndex(List<Execution> entries, double cutoffValue, Converter<Execution, double> converter)
        {
            var strats = GetStrategies(entries);
            int cutoffIndex = int.MaxValue;
            foreach (var strat in strats)
            {
                cutoffIndex = GetCutoffIndexForStrat(cutoffIndex, entries.Where(x => x.Strategy.Equals(strat)).ToList(), cutoffValue, converter);
            }
            return cutoffIndex;
        }

        private static int GetCutoffIndexForStrat(int cutoffIndex, List<Execution> entries, double cutoffValue, Converter<Execution, double> converter)
        {
            var data = entries.ConvertAll(converter);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] > cutoffValue)
                {
                    if (i < cutoffIndex)
                        return i;
                    else
                        return cutoffIndex;
                }
            }

            return cutoffIndex;
        }
    }
}
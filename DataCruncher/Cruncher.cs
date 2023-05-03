using DataCruncher.Plots;
using DataCruncher.ResultTypes;
using DataCruncher.Tables;
using DataCruncher.Utilities;
using Models;
using System;
using System.Collections.Immutable;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace DataCruncher
{
    public partial class Cruncher
    {
        static bool Shrink = false;
        static int ShrinkDigits = 8;
        public Dataset ReducedData;
        public Dataset NonreducedData;
        public CompetitionDataSet CompetitionData;

        private StatsGenerator _statsGenerator = new StatsGenerator();
        private CompetitionTableGenerator _competitionTableGenerator = new CompetitionTableGenerator();
        private ExecutionAggregator _aggregator = new ExecutionAggregator();
        private RatioPlotter _ratioPlotter = new RatioPlotter();
        private CactusPlotter _cactusPlotter = new CactusPlotter();
        private VirtualBestAdder _virtualBestAdder = new VirtualBestAdder();
        private ThroughputPlotter _throughputPlotter = new ThroughputPlotter();
        private MinMedianMaxPlotter _minMedianMaxPlotter = new MinMedianMaxPlotter();
        private SolvedOverTimePlotter _solvedOverTimePlotter = new SolvedOverTimePlotter();

        public Cruncher(string nonreducedDataFilepath, string reducedDataFilepath, string truthFilepath, string competitionFilepath)
        {
            Printer.PrintLine("Loading Data");

            CompetitionData = LoadCompetitionData(competitionFilepath);
            ReducedData = LoadReducedData(reducedDataFilepath);
            NonreducedData = LoadNonreducedData(nonreducedDataFilepath);
            UpdateDatasetsWithDifficultExecutions(NonreducedData, ReducedData);
            Printer.PrintSpace();
            VerifyAgreement(NonreducedData);
            VerifyAgreement(ReducedData);
            VerifyResults(truthFilepath, NonreducedData);

            Printer.PrintSpace();

            Tables();
            Appendix_B();
        }

        private CompetitionDataSet LoadCompetitionData(string competitionFilepath)
        {
            var data = new CompetitionDataSet();
            var file = File.ReadAllText(competitionFilepath);
            var regex = new Regex("-> [0-9]+");

            var matches = regex.Matches(file);
            data.Cardinality = new CompetitionEntry(int.Parse(matches[0].Value.Substring(3)), int.Parse(matches[1].Value.Substring(3)));
            data.Fireability= new CompetitionEntry(int.Parse(matches[2].Value.Substring(3)), int.Parse(matches[3].Value.Substring(3)));
            return data;
        }

        private void AddVirtualBest(List<Execution> executions)
        {
            executions.AddRange(_virtualBestAdder.GetVirtualBest(executions));
        }

        private void PrintStats(string folder, List<Execution> executions)
        {
            Printer.Pause();
            _statsGenerator.PrintStats(Copy(executions), folder, "Stats");
            Printer.Start();
        }

        private void UpdateDatasetsWithDifficultExecutions(Dataset nonreducedData, Dataset reducedData)
        {
            var nonreducedDifficult = GetDifficultInstances(nonreducedData);
            var reducedDifficult = GetDifficultInstances(reducedData);
            nonreducedData.GenerateDeficult(nonreducedDifficult);
            reducedData.GenerateDeficult(reducedDifficult);
        }

        private List<Execution> GetBaseLineExecutions(List<Execution> executions)
        {
            Printer.Pause();
            executions = AggregateAllExecutions(executions);
            Printer.Start();
            return executions.Where(x =>
                x.Strategy == "BFS" ||
                x.Strategy == "DFS" ||
                x.Strategy == "RDFS" ||
                x.Strategy == "BestFS"
                ).ToList();
        }

        private List<Execution> GetBaselineAndRpfsExecutions(List<Execution> executions)
        {
            Printer.Pause();
            executions = AggregateAllExecutions(executions);
            Printer.Start();
            return executions.Where(x =>
                x.Strategy == "BFS" ||
                x.Strategy == "DFS" ||
                x.Strategy == "RDFS" ||
                x.Strategy == "BestFS" ||
                x.Strategy == "RPFS"
                ).ToList();
        }

        private List<Execution> GetBaselineAndRpfslpExecutions(List<Execution> executions)
        {
            Printer.Pause();
            executions = AggregateAllExecutions(executions);
            Printer.Start();
            return executions.Where(x =>
                x.Strategy == "BFS" ||
                x.Strategy == "DFS" ||
                x.Strategy == "RDFS" ||
                x.Strategy == "BestFS" ||
                x.Strategy == "RPFS-LP" 
                ).ToList();
        }

        private List<Execution> GetBaseLineExecutionsWithRPFS(List<Execution> executions)
        {
            Printer.Pause();
            executions = AggregateAllExecutions(executions);
            Printer.Start();
            return executions;
        }

        private List<Execution> AggregateAllExecutions(List<Execution> executions)
        {
            _aggregator.Aggregate(ref executions, "BFS", new List<string>() { "BFS" }, new List<string>() { });
            _aggregator.Aggregate(ref executions, "DFS", new List<string>() { "DFS" }, new List<string>() { "RDFS" });
            _aggregator.Aggregate(ref executions, "RDFS", new List<string>() { "RDFS" }, new List<string>() { });
            _aggregator.Aggregate(ref executions, "BestFS", new List<string>() { "BestFS" }, new List<string>() { });
            _aggregator.Aggregate(ref executions, "PFS", new List<string>() { "IPFS" }, new List<string>() { });
            _aggregator.Aggregate(ref executions, "DPFS", new List<string>() { "DPFS" }, new List<string>() { });
            _aggregator.Aggregate(ref executions, "RPFS", new List<string>() { "RPFS" }, new List<string>() { "--use-lp-potencies" });
            _aggregator.Aggregate(ref executions, "RPFS-LP", new List<string>() { "RPFS", "--use-lp-potencies" }, new List<string>() { });
            return executions;
        }

        private List<Execution> GetPFSExecutions(List<Execution> executions)
        {
            Printer.Pause();
            executions = AggregateAllExecutions(executions);
            Printer.Start();
            return executions.Where(x => 
                x.Strategy == "PFS" ||
                x.Strategy == "DPFS" ||
                x.Strategy == "RPFS" ||
                x.Strategy == "RPFS-LP" 
                ).ToList();
        }

        private List<Execution> GetRpfsAndBestfsExecutions(List<Execution> executions)
        {
            Printer.Pause();
            executions = AggregateAllExecutions(executions);
            Printer.Start();
            return executions.Where(x =>
                x.Strategy == "BestFS" ||
                x.Strategy == "RPFS"
                ).ToList();
        }

        private List<Execution> GetRpfslpAndBestfsExecutions(List<Execution> executions)
        {
            Printer.Pause();
            executions = AggregateAllExecutions(executions);
            Printer.Start();
            return executions.Where(x =>
                x.Strategy == "BestFS" ||
                x.Strategy == "RPFS-LP"
                ).ToList();
        }

        private SortedSet<string> GetDifficultInstances(Dataset dataset)
        {
            var simple = new SortedSet<string>();
            var total = new SortedSet<string>();
            foreach (var e in dataset.AllExecutions)
            {
                total.Add(e.Instance);
                if (e.IsSimple)
                    simple.Add(e.Instance);
            }
            Printer.PrintHeader("Instance Stats for " + dataset.Name);
            Printer.PrintLine("Total Models: {}", dataset.AllExecutions.ConvertAll(x=>x.Model).ToHashSet().Count());
            Printer.PrintLine("Total Instances: {}", total.Count());
            Printer.PrintLine("Simple Instances: {}", simple.Count());
            total.RemoveWhere(x => simple.Contains(x));
            Printer.PrintLine("Difficult Instances: {}", total.Count());
            return total;
        }

        private void VerifyResults(string truthFilepath, Dataset data)
        {
            var truthResult = new TruthResult(truthFilepath, data.AllExecutions, data.Name);
        }

        private void VerifyAgreement(Dataset data)
        {
            var consistencyResult = new ConsistencyResult(data.AllExecutions, data.Name);
        }

        private Dataset LoadReducedData(string dataFilepath)
        {
            var allExecutions = Execution.ParseCsv(dataFilepath);
            allExecutions = allExecutions.Where(x => x.ExitCode != 255).ToList();
            if (allExecutions is null)
                throw new Exception("Nonreduced executions was loaded incorrectly");
            return new Dataset("Reduced", allExecutions);
        }

        private Dataset LoadNonreducedData(string dataFilepath)
        {
            var allExecutions = Execution.ParseCsv(dataFilepath);
            if (allExecutions is null)
                throw new Exception("Reduced executions was loaded incorrectly");
            return new Dataset("Nonreduced", allExecutions);
        }

        public List<Execution> Copy(List<Execution> executions)
        {
            var res = new List<Execution>();
            foreach (var e in executions)
            {
                res.Add((Execution)e.Clone());
            }
            return res;
        }
    }
}
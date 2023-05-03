using Models;

namespace DataCruncher.ResultTypes
{
    internal class RpfsVsBestfs : BaseResultType
    {
        public RpfsVsBestfs(List<Execution> executions) : base("rpfs_vs_bestfs", executions)
        {
        }

        protected override void EvaluateAfterVirtualBest(List<Execution> executions, List<string> strategies)
        {
            _ratioPlotter.Plot(executions, ResultName, new List<string>() { "BestFS" }, "RPFS", Shrink, ShrinkDigits);
            _statsGenerator.PrintStats(executions, ResultName, "stats");
        }

        protected override void EvaluateBeforeVirtualBest(List<Execution> executions, List<string> strategies)
        {
        }

        protected override void PreProcessing(List<Execution> executions)
        {
            _aggregator.Aggregate(ref executions, "RPFS",
                new List<string>()
                {
                                "RPFS",
                },
                new List<string>()
                {
                                "--use-lp-potencies",
                }
                );
            _executions = executions.Where(x => x.Strategy.Contains("RPFS") || x.Strategy.Contains("BestFS")).ToList();
        }
    }

    internal class LastBaseline : BaseResultType
    {
        public LastBaseline(List<Execution> executions) : base("LastBaseline", executions)
        {
        }

        protected override void EvaluateAfterVirtualBest(List<Execution> executions, List<string> strategies)
        {
            _cactusPlotter.Plot(executions, "baseline", ResultName, strategies, x => x.Time, 0, Shrink, ShrinkDigits);
        }

        protected override void EvaluateBeforeVirtualBest(List<Execution> executions, List<string> strategies)
        {
        }

        protected override void PreProcessing(List<Execution> executions)
        {
            _aggregator.Aggregate(ref executions, "RPFS",
                new List<string>()
                {
                                "RPFS",
                },
                new List<string>()
                {
                                "--use-lp-potencies",
                }
                );
            _aggregator.Aggregate(ref executions, "RDFS",
                new List<string>()
                {
                                "RDFS",
                },
                new List<string>()
                {
                }
                );
            _executions = executions.Where(x => 
                x.Strategy.Contains("RPFS") || 
                x.Strategy.Contains("BestFS") ||
                x.Strategy.Contains("BFS") ||
                x.Strategy.Contains("DFS") ||
                x.Strategy.Contains("RDFS")
                ).ToList();
        }
    }
}

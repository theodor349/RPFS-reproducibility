using Models;

namespace DataCruncher.ResultTypes
{
    internal class BaselineResult : BaseResultType
    {
        public BaselineResult(List<Execution> executions) : base("baseline", executions)
        {
        }

        protected override void PreProcessing(List<Execution> executions)
        {
            _aggregator.Aggregate(ref executions, "RDFS",
                new List<string>()
                {
                    "RDFS",
                },
                new List<string>());
        }

        protected override void EvaluateBeforeVirtualBest(List<Execution> executions, List<string> strategies)
        {
            _ratioPlotter.Plot(executions, ResultName, strategies, "BestFS", Shrink, ShrinkDigits);
            var stats = _throughputPlotter.Plot(executions, "", ResultName, strategies, false, Shrink, ShrinkDigits);

        }

        protected override void EvaluateAfterVirtualBest(List<Execution> executions, List<string> strategies)
        {
            _cactusPlotter.Plot(executions, "cutoff0", ResultName, strategies, x => x.Time, 0, Shrink, ShrinkDigits);
            _statsGenerator.PrintStats(executions, ResultName, "");
        }
    }
}

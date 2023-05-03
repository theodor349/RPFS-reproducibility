using Models;

namespace DataCruncher.ResultTypes
{
    internal class OtherResult : BaseResultType
    {
        public OtherResult(List<Execution> executions) : base("other", executions.Where(x =>
        {
            bool res = false;
            res |= x.Strategy.Contains("IPFS");
            res |= x.Strategy.Contains("DPFS");
            res |= x.Strategy.Contains("RPFS") && !x.Strategy.Contains("-lp");
            return res;
        }).ToList())
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
        }

        protected override void EvaluateBeforeVirtualBest(List<Execution> executions, List<string> strategies)
        {
        }

        protected override void EvaluateAfterVirtualBest(List<Execution> executions, List<string> strategies)
        {
            _cactusPlotter.Plot(executions, "cutoff0", ResultName, strategies, x => x.Time, 0, Shrink, ShrinkDigits);
            _statsGenerator.PrintStats(executions, ResultName, "");
        }
    }
}

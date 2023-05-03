using Models;

namespace DataCruncher.ResultTypes
{
    internal class RawResult : BaseResultType
    {
        public RawResult(List<Execution> executions) : base("raw", executions)
        {
        }

        protected override void PreProcessing(List<Execution> executions)
        {
        }

        protected override void EvaluateBeforeVirtualBest(List<Execution> executions, List<string> strategies)
        {
        }

        protected override void EvaluateAfterVirtualBest(List<Execution> executions, List<string> strategies)
        {
            _cactusPlotter.Plot(executions, "cutoff0", ResultName, strategies, x => x.Time, 0, Shrink, ShrinkDigits);
            _statsGenerator.PrintStats(executions, ResultName, "cutoff0");
        }
    }
}

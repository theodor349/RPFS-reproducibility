using Models;
using System.Security.Cryptography.X509Certificates;

namespace DataCruncher.ResultTypes
{
    internal class DefaultResult : BaseResultType
    {
        public DefaultResult(List<Execution> executions) : base("default", executions)
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
            _aggregator.Aggregate(ref executions, "RPFS-LP",
                new List<string>()
                {
                    "RPFS",
                    "--use-lp-potencies",
                },
                new List<string>());
            _aggregator.Aggregate(ref executions, "RDFS",
                new List<string>()
                {
                    "RDFS",
                },
                new List<string>());
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

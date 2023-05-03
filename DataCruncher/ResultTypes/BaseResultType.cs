using DataCruncher.Plots;
using DataCruncher.Tables;
using DataCruncher.Utilities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.ResultTypes
{
    internal abstract class BaseResultType
    {
        public string ResultName { get; }
        public bool Shrink { get; private set; } = false;
        public int ShrinkDigits { get; private set; } = 16;

        protected List<Execution> _executions;
        private VirtualBestAdder _virtualBestAdder = new VirtualBestAdder();

        protected StatsGenerator _statsGenerator = new StatsGenerator();
        protected ExecutionAggregator _aggregator = new ExecutionAggregator();
        protected RatioPlotter _ratioPlotter = new RatioPlotter();
        protected CactusPlotter _cactusPlotter = new CactusPlotter();
        protected ThroughputPlotter _throughputPlotter = new ThroughputPlotter();
        protected MinMedianMaxPlotter _minMedianMaxPlotter = new MinMedianMaxPlotter();
        protected SolvedOverTimePlotter _solvedOverTimePlotter = new SolvedOverTimePlotter();

        public BaseResultType(string name, List<Execution> executions)
        {
            ResultName = name;
            _executions = executions;
        }

        public void Evaluate()
        {
            Printer.PrintHeader($"Evaluating: {ResultName}");
            Directory.CreateDirectory(ResultName);

            PreProcessing(_executions);

            var executions = Copy(_executions);
            EvaluateBeforeVirtualBest(executions, Execution.GetStrategies(executions));

            executions = Copy(_executions);
            executions.AddRange(_virtualBestAdder.GetVirtualBest(_executions));
            EvaluateAfterVirtualBest(executions, Execution.GetStrategies(executions));
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

        protected abstract void PreProcessing(List<Execution> executions);
        protected abstract void EvaluateBeforeVirtualBest(List<Execution> executions, List<string> strategies);
        protected abstract void EvaluateAfterVirtualBest(List<Execution> executions, List<string> strategies);
    }
}

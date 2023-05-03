using FileHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Utilities
{
    internal class TruthResult
    {
        public int Correct { get; set; }
        public int Incorrect { get; set; }
        public int Unknown { get; set; }
        public List<Execution> IncorrectExecutions { get; private set; } = new List<Execution>();
        public string Name { get; }

        private Dictionary<string, InstanceTruth> _truth = new Dictionary<string, InstanceTruth>();

        public TruthResult(string truthFilePath, List<Execution> allExecutions, string name)
        {
            Name = name;
            GenerateTruthLookup(truthFilePath);
            CheckExecutions(allExecutions);

            PrintStats();
        }

        private void PrintStats()
        {
            Printer.PrintHeader("Truth Results for " + Name);
            Printer.PrintLine(" - Unknown:   {}", Unknown);
            Printer.PrintLine(" - Correct:   {}", Correct);
            Printer.PrintLine(" - Incorrect: {}", Incorrect);
        }

        private void CheckExecutions(List<Execution> allExecutions)
        {
            foreach (var execution in allExecutions)
            {
                CheckExecution(execution);
            }
        }

        private void CheckExecution(Execution execution)
        {
            var expected = _truth[execution.Instance];
            if (expected.Result == ExecutionResult.Null ||
                !execution.Solved)
            {
                Unknown++;
            }
            else if (expected.Result == execution.Result)
            {
                Correct++;
            }
            else
            {
                Incorrect++;
                IncorrectExecutions.Add(execution);
            }
        }

        private void GenerateTruthLookup(string truthFilePath)
        {
            var engine = new FileHelperEngine<InstanceTruth>();
            var rows = engine.ReadFile(truthFilePath);
            int undefined = 0;
            foreach (var t in rows)
            {
                _truth.Add(t.Key, t);
                if (t.Result == ExecutionResult.Null)
                    {
                    undefined++;
                }
            }
        }
    }
}

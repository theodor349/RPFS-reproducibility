using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Utilities
{
    internal class ConsistencyResult
    {
        public int Agreements { get; set; }
        public int Disagreements { get; set; }
        public Dictionary<string, List<Execution>> InconsistentInstances { get; set; } = new Dictionary<string, List<Execution>>();
        public string Name { get; }

        private Dictionary<string, List<Execution>> InstanceOverview = new Dictionary<string, List<Execution>>();


        public ConsistencyResult(List<Execution> executions, string name)
        {
            Name = name;
            GenerateInstanceOverview(executions);
            CheckConsistency();

            PrintResults();
        }

        private void PrintResults()
        {
            Printer.PrintHeader("Consistency Results for " + Name);
            Printer.PrintLine(" - Agreements:    {}", Agreements);
            Printer.PrintLine(" - Disagreements: {}", Disagreements);

            if(Disagreements > 0)
            {
                foreach (var incosistency in InconsistentInstances)
                {
                    Printer.PrintLine("Instance {} is inconsistent!", incosistency.Key);
                    foreach (var execution in incosistency.Value)
                    {
                        Printer.PrintLine(" - {}: {} - Is difficult: {} - Total time {}", execution.Strategy, execution.Result, execution.IsDifficult, execution.Time);
                    }
                }
            }
        }

        private void CheckConsistency()
        {
            foreach (var instance in InstanceOverview)
            {
                var result = ExecutionResult.Null;
                bool agrees = true;
                foreach (var e in instance.Value)
                {
                    if (!e.Solved)
                        continue;
                    if (result == ExecutionResult.Null)
                        result = e.Result;

                    agrees &= e.Result == result;
                }

                if (agrees)
                {
                    Agreements++;
                }
                else
                {
                    Disagreements++;
                    InconsistentInstances.Add(instance.Key, instance.Value);
                }
            }
        }

        private void GenerateInstanceOverview(List<Execution> executions)
        {
            foreach (var e in executions)
            {
                if (InstanceOverview.ContainsKey(e.Instance))
                    InstanceOverview[e.Instance].Add(e);
                else 
                    InstanceOverview.Add(e.Instance, new List<Execution>() { e });
            }
        }
    }
}

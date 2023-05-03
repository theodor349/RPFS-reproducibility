using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Utilities
{
    internal class VirtualBestAdder
    {
        public List<Execution> GetVirtualBest(List<Execution> executions)
        {
            var instances = new Dictionary<string, Execution>();
            foreach (var entry in executions)
            {
                var key = entry.Instance;
                if (instances.ContainsKey(key))
                {
                    if (!entry.Solved)
                        continue;
                    var fastest = instances[key];
                    if (!fastest.Solved || fastest.Time > entry.Time)
                        instances[key] = entry;
                }
                else
                    instances.Add(key, entry);
            }

            var res = new List<Execution>();
            foreach (var q in instances)
            {
                res.Add(new Execution(q.Value.Model, q.Value.Query, q.Value.Solved, q.Value.Result, Execution.VirtualBestName, q.Value.Time, DateTime.UtcNow, q.Value.TimeLimit, q.Value.Memory, q.Value.ExitCode, q.Value.Formula, q.Value.TimedOut));
            }

            res.Sort();
            return res;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Dataset
    {
        public string Name { get; }
        public List<Execution> AllExecutions { get; private set; }
        public List<Execution> DifficultExecutions { get; private set; }

        public Dataset(string name, List<Execution> allExecutions)
        {
            Name = name;
            AllExecutions = allExecutions;
        }

        public void GenerateDeficult(SortedSet<string> difficultInstances)
        {
            DifficultExecutions = new List<Execution>();
            foreach (var e in AllExecutions)
            {
                if(difficultInstances.Contains(e.Instance))
                    DifficultExecutions.Add(e);
            }
        }
    }
}

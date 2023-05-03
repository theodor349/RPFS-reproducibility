using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class StrategyComparison
    {
        public string Model { get; set; }
        public int Query { get; set; }
        public List<string> Strategies { get; set; } = new List<string>();
        public List<bool> Results { get; set; } = new List<bool>();

        public StrategyComparison(string model, int query, string strategy, bool result)
        {
            Model = model;
            Query = query;
            Strategies.Add(strategy);
            Results.Add(result);
        }

        public bool StrategiesAgrees 
        { 
            get 
            {
                var count = Results.Count(x => x);
                if (count == 0) return true;
                else return count == Results.Count;
            } 
        }

        public StringBuilder PrintDisagreement()
        {
            var s = new StringBuilder();
            var teamSatisfied = new List<string>();
            var teamUnsatisfied = new List<string>();
            for (int i = 0; i < Strategies.Count(); i++)
            {
                if (Results[i])
                    teamSatisfied.Add(Strategies[i]);
                else
                    teamUnsatisfied.Add(Strategies[i]);
            }

            if(teamSatisfied.Count == 0 || teamUnsatisfied.Count == 0)
                s.Append("No disagreement");
            else
            {
                s.Append($"{Model}\t{Query}\t");
                for (int i = 0; i < teamSatisfied.Count; i++)
                {
                    string? strat = teamSatisfied[i];
                    if (i == 0)
                        s.Append($"{strat}");
                    else
                        s.Append($"|{strat}");
                }

                s.Append($"\t");
                for (int i = 0; i < teamUnsatisfied.Count; i++)
                {
                    string? strat = teamUnsatisfied[i];
                    if (i == 0)
                        s.Append($"{strat}");
                    else
                        s.Append($"|{strat}");
                }
            }
            return s;
        }
    }
}

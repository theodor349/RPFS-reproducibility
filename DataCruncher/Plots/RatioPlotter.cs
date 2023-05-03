using Models;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace DataCruncher.Plots
{
    internal class RatioPlotter : BasePlotter
    {
        public void Plot(List<Execution> executions, string folder, List<string> strategies, string targetStrategy, bool shrink, int shrinkDigits, string name = "")
        {
            var targetValues = GetTargetValues(executions.Where(x => x.Strategy == targetStrategy));
            var data = GenerateData(targetValues, strategies.Where(x => x != targetStrategy), executions.Where(x => x.Strategy != targetStrategy));
            var slow = GetSubset(data, "{0}", x => x < 0, x => -x);
            var fast = GetSubset(data, "{0}", x => x > 0, x => x);

            Print(folder, $"{name}_Ratio_{targetStrategy}_Slow", GenerateCactusPlot("% Slow", slow, 0, true, shrink, shrinkDigits));
            Print(folder, $"{name}_Ratio_{targetStrategy}_Fast", GenerateCactusPlot("% Fast", fast, 0, true, shrink, shrinkDigits));
        }

        private Dictionary<string, List<double>> GetSubset(Dictionary<string, List<double>> data, string keyFormat, Func<double, bool> selector, Converter<double, double> converter)
        {
            var res = new Dictionary<string, List<double>>();
            foreach (var pair in data)
            {
                res.Add(string.Format(keyFormat, pair.Key), pair.Value.Where(selector).ToList().ConvertAll(converter));
            }
            return res;
        }

        private Dictionary<string, List<double>> GenerateData(Dictionary<string, double> targetValues, IEnumerable<string> strategies, IEnumerable<Execution> executions)
        {
            var res = new Dictionary<string, List<double>>();
            foreach (var strategy in strategies)
            {
                res.Add(strategy, CalculateRatio(executions.Where(x => x.Strategy == strategy), targetValues));
            }
            return res;
        }

        private List<double> CalculateRatio(IEnumerable<Execution> executions, Dictionary<string, double> targetValues)
        {
            var dic = new List<RatioEntry>();
            foreach (var execution in executions)
            {
                if (targetValues.ContainsKey(execution.Instance))
                {
                    var target = targetValues[execution.Instance];
                    if (execution.Solved)
                        dic.Add(new RatioEntry(target, execution.Time));
                    else
                        dic.Add(new RatioEntry(target, null));
                }
                else
                {
                    if (execution.Solved)
                        dic.Add(new RatioEntry(null, execution.Time));
                    else
                        dic.Add(new RatioEntry(null, null));
                }
            }

            var temp = dic.ConvertAll(x => x.Ratio);
            temp = temp.Where(x => x.HasValue).ToList();
            var res = temp.ConvertAll(x => x.Value);
            var max = res.Where(x => x != double.MaxValue).Max();
            var min = res.Where(x => x != double.MinValue).Min();
            for (int i = 0; i < res.Count; i++)
            {
                double x = res[i];
                if (x == double.MaxValue)
                    res[i] = max;
                if (x == double.MinValue)
                    res[i] = min;
            }

            return res;
        }

        private Dictionary<string, double> GetTargetValues(IEnumerable<Execution> executions)
        {
            var res = new Dictionary<string, double>();
            foreach (var execution in executions)
            {
                if (execution.Solved)
                    res.Add(execution.Instance, execution.Time);
            }
            return res;
        }
    }

    internal class RatioEntry
    {
        public double? Target { get; set; }
        public double? Value { get; set; }
        public double? Ratio
        {
            get
            {
                if (Target is null && Value is null)
                    return null;
                if (Target is null)
                    return double.MinValue;
                if (Value is null)
                    return double.MaxValue;
                if (Target > Value)
                    return -(Target / Value);
                else
                    return Value / Target;
            }
        }

        public RatioEntry(double? target, double? value)
        {
            Target = target;
            Value = value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher.Plots
{
    internal class BasePlotter
    {
        protected string GenerateCactusPlot(string title, Dictionary<string, List<Tuple<double, int>>> data, int indexOffset, bool sort, bool shrink, int shrinkDigits)
        {
            var plot = new StringBuilder();
            plot.AppendLine(title);
            foreach (var legend in data)
            {
                plot.Append(GenerateLegend(legend.Key, legend.Value, indexOffset, sort, shrink, shrinkDigits));
                plot.AppendLine();
                plot.AppendLine();
                plot.AppendLine();
            }
            return plot.ToString();
        }

        protected string GenerateCactusPlot(string title, Dictionary<string, List<double>> data, int indexOffset, bool sort, bool shrink, int shrinkDigits, bool minMax = false)
        {
            var plot = new StringBuilder();
            plot.AppendLine(title);
            int i = 1;
            foreach (var legend in data)
            {
                plot.Append(GenerateLegend(legend.Key, legend.Value, indexOffset, sort, shrink, shrinkDigits, minMax && data.Count() - i > 0));
                plot.AppendLine();
                plot.AppendLine();
                plot.AppendLine();
                i++;
            }
            return plot.ToString();
        }

        private StringBuilder GenerateLegend(string name, List<double> values, int indexOffset, bool sort, bool shrink, int shrinkDigits, bool minMax)
        {
            if (sort)
                values.Sort();
            if (shrink)
                values = values.ConvertAll(x => Math.Round(x, shrinkDigits));

            var legend = new StringBuilder();
            legend.AppendLine("% Legend: " + name);
            if(minMax)
                legend.AppendLine("\\addplot+[only marks, mark size=.8pt]  coordinates {");
            else
                legend.AppendLine("\\addplot+[mark=none]  coordinates {");

            if (shrink)
                GenerateShrinkedLegend(values, indexOffset, legend);
            else
                GenerateExpandedLegend(values, indexOffset, legend);
            legend.AppendLine("};");
            legend.AppendLine("\\addlegendentry{" + name + "}");
            return legend;
        }

        private StringBuilder GenerateLegend(string name, List<Tuple<double, int>> values, int indexOffset, bool sort, bool shrink, int shrinkDigits)
        {
            if (sort)
                values.Sort();

            var legend = new StringBuilder();
            legend.AppendLine("% Legend: " + name);
            legend.AppendLine("\\addplot+[mark=none]  coordinates {");
            if (shrink)
                throw new NotImplementedException();
            else
                GenerateExpandedLegend(values, indexOffset, legend);
            legend.AppendLine("};");
            legend.AppendLine("\\addlegendentry{" + name + "}");
            return legend;
        }

        private void GenerateShrinkedLegend(List<double> values, int indexOffset, StringBuilder legend)
        {
            if (values.Count == 0)
                return;

            int count = 1;
            var lastValue = values[indexOffset];
            legend.AppendLine($"\t({indexOffset},{lastValue.ToString(CultureInfo.GetCultureInfo("en-US"))})");
            for (int i = indexOffset + 1; i < values.Count; i++)
            {
                var v = values[i];
                if (v == lastValue)
                {
                    count++;
                    continue;
                }
                if(count > 1)
                    legend.AppendLine($"\t({i-1},{values[i-1].ToString(CultureInfo.GetCultureInfo("en-US"))})");
                legend.AppendLine($"\t({i},{v.ToString(CultureInfo.GetCultureInfo("en-US"))})");
                count = 1;
                lastValue = v;
            }
            if(count > 1)
                legend.AppendLine($"\t({values.Count - 1},{values[values.Count - 1].ToString(CultureInfo.GetCultureInfo("en-US"))})");
        }

        private static void GenerateExpandedLegend(List<double> values, int indexOffset, StringBuilder legend)
        {
            for (int i = indexOffset; i < values.Count; i++)
            {
                var v = values[i];
                legend.AppendLine($"\t({i},{v.ToString(CultureInfo.GetCultureInfo("en-US"))})");
            }
        }

        private static void GenerateExpandedLegend(List<Tuple<double, int>> values, int indexOffset, StringBuilder legend)
        {
            for (int i = indexOffset; i < values.Count; i++)
            {
                var v = values[i];
                legend.AppendLine($"\t({v.Item1.ToString(CultureInfo.GetCultureInfo("en-US"))},{v.Item2.ToString(CultureInfo.GetCultureInfo("en-US"))})");
            }
        }

        protected void Print(string folder, string filename, string data)
        {
            File.WriteAllText(Path.Combine(folder, filename + ".plot"), data);
        }
    }
}

using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCruncher
{
    public partial class Cruncher
    {
        private void Appendix_B()
        {
            Printer.PrintSpace();
            Printer.PrintHeader("Appendix B");
            string folder = "Appendix_B";
            Directory.CreateDirectory(folder);
            Fig3(folder);
            Fig4(folder);
        }

        private void Fig3(string folder)
        {
            Printer.PrintHeader("Fig 3");
            var executions = NonreducedData.DifficultExecutions.Where(x => x.Strategy.Contains("RPFS")).ToList();
            var strats = Execution.GetStrategies(executions);

            var temp = Copy(executions);
            AddVirtualBest(executions);
            _cactusPlotter.Plot(Copy(executions), "Fig_3", folder, Execution.GetStrategies(executions), x => x.Time, 0, true, 3);
        }

        private void Fig4(string folder)
        {
            Printer.PrintHeader("Fig 4");

            var executions = GetBaselineAndRpfsExecutions(Copy(NonreducedData.DifficultExecutions));

            AddVirtualBest(executions);
            var strats = Execution.GetStrategies(executions);
            _cactusPlotter.Plot(Copy(executions), "Fig 4", folder, Execution.GetStrategies(executions), x => x.Time, 0, true, 3);
        }
    }
}

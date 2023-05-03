using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [DelimitedRecord("\t"), IgnoreFirst(1)]
    public class InstanceTruth
    {
        public string Model { get; set; }
        public int Query { get; set; }
        public ExecutionResult Result { get; set; }

        public string Key => GetKey(Model, Query);
        public static string GetKey(string model, int query) => $"{model}_{query}";
    }
}

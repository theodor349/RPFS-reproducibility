namespace Models
{
    public class ErrorStat
    {

        public ErrorStat(string strategy)
        {
            Strategy = strategy;
        }

        public string Strategy { get; set; }
        public int TotalErrors { get; set; }
        public int OurNoFormula { get; set; }
        public int OurTimeout { get; set; }
        public int Timeout { get; set; }            // 124
        public int FailedAssertion { get; set; }    // 134
        public int OutOffMemory { get; set; }       // 137
        public int MainTryCatch { get; set; }       // 255 or -1
        public int Segfault { get; set; }           // 139
        public int Unknown { get; set; }
    }
}
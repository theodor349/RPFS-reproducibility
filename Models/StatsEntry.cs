namespace Models
{
    public class StatsEntry : IComparable<StatsEntry>
    {
        public string Strategy { get; set; }
        public int TotalQueries { get; set; }
        public int SolvedQueries { get; set; }
        public int TimeoutQueries { get; set; }
        public int Fastestnstances { get; set; }
        public int UniqueQueries { get; set; }
        public int TotalErrors { get; set; }

        public Dictionary<int, int> Erros { get; set; } = new Dictionary<int, int>();

        public int CompareTo(StatsEntry other)
        {
            if(UniqueQueries == other.UniqueQueries)
            {
                if (Fastestnstances == other.Fastestnstances)
                {
                    return SolvedQueries.CompareTo(other.SolvedQueries);
                }
                else
                    return Fastestnstances.CompareTo(other.Fastestnstances);
            }
            else 
                return UniqueQueries.CompareTo(other.UniqueQueries);
        }
    }
}
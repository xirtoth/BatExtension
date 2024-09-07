namespace BatExtension
{
    public class CombatStat
    {
        public int Round { get; set; }
        public List<string> HitTypes { get; set; }
        public string? EnemyName { get; set; }

        public DateTime Time { get; set; }

        public int TotalHits { get; set; }
        public int Misses { get; set; }

        public CombatStat(int Round, string EnemyName)
        {
            this.Round = Round;
            HitTypes = new List<string>();
            this.EnemyName = EnemyName;
            TotalHits = 0;
        }

        public CombatStat()
        {
            HitTypes = new List<string>();
        }

        public void IncreaseRound()
        {
            Round++;
        }

        public void AddHitType(string HitType)
        {
            HitTypes.Add(HitType);
        }

        public void IncreaseHits()
        {
            TotalHits++;
        }

        public void IncreaseMisses()
        {
            Misses++;
        }
    }
}
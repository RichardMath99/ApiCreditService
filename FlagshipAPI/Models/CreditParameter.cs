namespace FlagshipAPI.Models
{
    public class CreditParameter
    {
        public int Id { get; set; }
        public required string Product { get; set; }
        public required string Key { get; set; }
        public decimal Value { get; set; }
        public DateTime EffectiveStart { get; set; }
        public DateTime? EffectiveEnd { get; set; }
    }
}

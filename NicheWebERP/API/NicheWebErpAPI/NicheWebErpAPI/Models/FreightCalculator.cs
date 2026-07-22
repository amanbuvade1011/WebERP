namespace NicheWebErpAPI.Models
{
    // Maps to dbo.FreightCalculator - named freight-calculation method, referenced from
    // CompanyLocation.FreightID. CalculationMethod1 has no lookup table; both live calculators
    // (FreightPerItem=1, Price=3) are handled uniformly via FreightItem rate-band matching - see
    // docs/ai-plan/sprints/sprint-07-promotions-freight.md.
    public class FreightCalculator
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string Description { get; set; } = null!;
        public Guid CurrencyID { get; set; }
        public int CalculationMethod1 { get; set; }
        public bool? TaxInclusive { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

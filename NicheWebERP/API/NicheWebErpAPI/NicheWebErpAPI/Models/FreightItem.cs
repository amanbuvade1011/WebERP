namespace NicheWebErpAPI.Models
{
    // Maps to dbo.FreightItem - a rate-band row for a FreightCalculator. CountryID is the
    // zero-guid ("any country") on every live row; RangeStart is the metric threshold (order item
    // count - inferred from the calculator's own "FreightPerItem" description and the real
    // banding pattern, see docs/ai-plan/sprints/sprint-07-promotions-freight.md), matched as
    // "largest RangeStart <= metric".
    public class FreightItem
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid FreightID { get; set; }
        public decimal? RangeStart { get; set; }
        public decimal? Price { get; set; }
        public Guid CountryID { get; set; }
        public Guid? TaxJurisdictionCategoryID { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

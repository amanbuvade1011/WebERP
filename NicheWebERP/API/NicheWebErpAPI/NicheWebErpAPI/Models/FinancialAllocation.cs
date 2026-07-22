namespace NicheWebErpAPI.Models
{
    // Maps to dbo.FinancialAllocation - allocates an amount from one transaction to another
    // (e.g. a Payment allocated against an Invoice). 0 live rows, confirmed 2026-07-20 - fully
    // unused by the legacy system, no existing semantics to preserve.
    public class FinancialAllocation
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid TransactionFromID { get; set; }
        public Guid TransactionToID { get; set; }
        public decimal Amount { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

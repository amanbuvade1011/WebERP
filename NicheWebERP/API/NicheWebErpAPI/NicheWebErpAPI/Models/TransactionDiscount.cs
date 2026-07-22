namespace NicheWebErpAPI.Models
{
    // Maps to dbo.TransactionDiscount - a discount application on a transaction (parent of one or
    // more DiscountLine rows in TransactionLine). DiscountRuleID is nullable and always null here
    // - DiscountRule has 0 live rows and no rule-definition columns at all (dead end), see
    // docs/ai-plan/sprints/sprint-07-promotions-freight.md. This row exists purely as the
    // legacy-shaped record that a discount was applied to this transaction.
    public class TransactionDiscount
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid AppliesToTransactionID { get; set; }
        public Guid? DiscountRuleID { get; set; }
        public int Sequence { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

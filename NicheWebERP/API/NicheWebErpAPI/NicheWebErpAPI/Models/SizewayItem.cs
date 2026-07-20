namespace NicheWebErpAPI.Models
{
    // Maps to dbo.SizewayItem - one size slot within a Sizeway, in Sequence order.
    public class SizewayItem
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        // Logical links (same CompanyID) - no DB foreign keys exist for these
        public Guid SizeID { get; set; }
        public Guid SizewayID { get; set; }

        public int Sequence { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

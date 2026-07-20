namespace NicheWebErpAPI.Models
{
    // Maps to dbo.StyleSellLocation - per-location, per-style selling rules.
    public class StyleSellLocation
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid StyleID { get; set; }
        public Guid LocationID { get; set; }

        public byte AllowRetail { get; set; }
        public byte AllowWebRetail { get; set; }
        public byte AllowRental { get; set; }
        public byte AllowWholesaleIndent { get; set; }
        public byte AllowWebWholesaleIndent { get; set; }
        public byte AllowWebWholesaleStock { get; set; }

        public Guid PricePointRRPID { get; set; }
        public Guid PricePointWebID { get; set; }
        public Guid PricePointRetailID { get; set; }

        public bool AllowRetailPreorder { get; set; }
        public bool AllowRetailLayBy { get; set; }
        public byte AllowWebRetailCatalogue { get; set; }
        public bool AllowWebRetailStock { get; set; }
        public bool AllowWebRetailPreOrder { get; set; }
        public bool AllowWebRetailLayBy { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}

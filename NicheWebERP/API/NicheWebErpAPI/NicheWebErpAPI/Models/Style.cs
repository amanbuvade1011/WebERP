namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Style
    // Primary key is composite: (CompanyID, EntityID) - configured in ERPDbContext.OnModelCreating
    public class Style
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? WebDescription { get; set; }
        public decimal Weight { get; set; }

        // Logical links to other tables (same CompanyID) - no DB foreign key exists for these
        public Guid SizewayID { get; set; }
        public Guid? CategoryID { get; set; }
        public Guid? LabelID { get; set; }
        public Guid? WebMainPictureID { get; set; }
        public Guid RangeID { get; set; }

        public bool Inactive { get; set; }
        public int SortRankMajor { get; set; }
        public int SortRankMinor { get; set; }
        public string? DeliveryPeriod { get; set; }

        public bool NonStock { get; set; }
        public bool AllowManualPrice { get; set; }
        public bool AllowManualDescription { get; set; }
        public bool DoPopUp { get; set; }

        public string? MetaTagTitle { get; set; }
        public string? MetaTagDescription { get; set; }
        public string? AdditionalInfo1 { get; set; }
        public string? AdditionalInfo2 { get; set; }
        public string? AdditionalInfo3 { get; set; }
        public string? AdditionalInfo4 { get; set; }
        public string? AdditionalInfoVideo { get; set; }
        public string? AdditionalInfoComments { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}

namespace NicheWebErpAPI.Models
{
    // Maps to dbo.StyleColor
    // Primary key is composite: (CompanyID, EntityID). Unique on (CompanyID, StyleID, Color).
    public class StyleColor
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        // Logical link to Style.EntityID (same CompanyID) - no DB foreign key exists for this
        public Guid StyleID { get; set; }

        public string Color { get; set; } = null!;
        public bool? Inactive { get; set; }
        public string? RgbValue { get; set; }
        public bool? HasSwatchThumbnail { get; set; }
        public bool? HasSwatchImage { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

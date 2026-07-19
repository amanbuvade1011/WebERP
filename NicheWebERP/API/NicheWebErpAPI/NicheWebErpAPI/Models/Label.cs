namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Label
    public class Label
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string Description { get; set; } = null!;
        public bool Inactive { get; set; }
        public string? WebPageName { get; set; }
        public string? MetaTagTitle { get; set; }
        public string? MetaTagDescription { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}

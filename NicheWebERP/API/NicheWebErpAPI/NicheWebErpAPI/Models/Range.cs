namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Range - merchandise range/collection tree (nested set), separate from Category.
    public class Range
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string? Description { get; set; }
        public Guid? ParentRangeID { get; set; }
        public bool Inactive { get; set; }
        public int Depth { get; set; }
        public int Sequence { get; set; }
        public string? WebPageName { get; set; }
        public string? MetaTagTitle { get; set; }
        public string? MetaTagDescription { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}

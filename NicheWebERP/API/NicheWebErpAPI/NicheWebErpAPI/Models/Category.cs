namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Category
    public class Category
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string Description { get; set; } = null!;
        public Guid? ParentCategoryID { get; set; }
        public int LeftTag { get; set; }
        public int RightTag { get; set; }
        public int Depth { get; set; }
        public bool Inactive { get; set; }
        public int Sequence { get; set; }
        public string? WebPageName { get; set; }
        public string? MetaTagTitle { get; set; }
        public string? MetaTagDescription { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}

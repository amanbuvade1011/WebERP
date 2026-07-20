namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Season
    public class Season
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string Description { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool? Inactive { get; set; }
        public DateTime? IndentEarliestDueDate { get; set; }
        public DateTime? IndentLatestDueDate { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

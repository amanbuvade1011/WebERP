namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Size - master list of size labels (S, M, L, 32, 34...), shared across sizeways.
    public class Size
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string Description { get; set; } = null!;

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

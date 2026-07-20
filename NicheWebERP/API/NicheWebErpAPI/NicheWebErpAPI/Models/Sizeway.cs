namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Sizeway - a named size-run/size-curve (e.g. "S-M-L-XL"), assigned to Style.SizewayID.
    public class Sizeway
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string Description { get; set; } = null!;
        public bool ExcludeRetailSearch { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}

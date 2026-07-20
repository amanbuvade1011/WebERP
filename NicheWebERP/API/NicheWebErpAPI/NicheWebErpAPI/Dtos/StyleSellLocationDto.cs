namespace NicheWebErpAPI.Dtos
{
    public class StyleSellLocationDto
    {
        public Guid LocationId { get; set; }
        public string? LocationName { get; set; }
        public bool AllowRetail { get; set; }
        public bool AllowWebRetail { get; set; }
        public bool AllowRental { get; set; }
        public bool AllowWholesaleIndent { get; set; }
    }

    public class UpdateSellLocationsDto
    {
        public List<SellLocationLineDto> Locations { get; set; } = new();
    }

    public class SellLocationLineDto
    {
        public Guid LocationId { get; set; }
        public bool AllowRetail { get; set; }
        public bool AllowWebRetail { get; set; }
        public bool AllowRental { get; set; }
        public bool AllowWholesaleIndent { get; set; }
    }
}

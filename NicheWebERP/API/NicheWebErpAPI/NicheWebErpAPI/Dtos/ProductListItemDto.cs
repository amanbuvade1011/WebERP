namespace NicheWebErpAPI.Dtos
{
    // Shape returned to the frontend for the product listing page
    public class ProductListItemDto
    {
        public string StyleCode { get; set; } = null!;
        public string Garment { get; set; } = null!;
        public string? Category { get; set; }
        public string? Label { get; set; }
    }
}

namespace NicheWebErpAPI.Dtos
{
    public class ProductStockLocationDto
    {
        public Guid LocationId { get; set; }
        public string? LocationName { get; set; }
        public int Held { get; set; }
        public int Allocated { get; set; }
        public int Available { get; set; }
    }

    public class ProductStockDto
    {
        public Guid ProductId { get; set; }
        public string StyleCode { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string SizeDescription { get; set; } = null!;
        public string? Barcode { get; set; }
        public List<ProductStockLocationDto> Locations { get; set; } = new();
    }

    // One row per (size, location) cell - the frontend pivots this into a grid.
    public class StockGridRowDto
    {
        public Guid ProductId { get; set; }
        public Guid SizeId { get; set; }
        public string SizeDescription { get; set; } = null!;
        public Guid LocationId { get; set; }
        public string? LocationName { get; set; }
        public int Held { get; set; }
        public int Allocated { get; set; }
        public int Available { get; set; }
    }
}

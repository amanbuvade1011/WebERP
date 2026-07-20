namespace NicheWebErpAPI.Dtos
{
    public class StyleDetailDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? WebDescription { get; set; }
        public decimal Weight { get; set; }

        public Guid SizewayId { get; set; }
        public string? SizewayDescription { get; set; }

        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public Guid? LabelId { get; set; }
        public string? LabelName { get; set; }

        public Guid RangeId { get; set; }
        public string? RangeName { get; set; }

        public bool Inactive { get; set; }
        public bool NonStock { get; set; }
        public bool AllowManualPrice { get; set; }
        public string? DeliveryPeriod { get; set; }

        public List<StyleColorDto> Colors { get; set; } = new();
    }
}

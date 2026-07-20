namespace NicheWebErpAPI.Dtos
{
    public class UpdateStyleDto
    {
        public string Description { get; set; } = null!;
        public string? WebDescription { get; set; }
        public decimal Weight { get; set; }
        public Guid SizewayId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? LabelId { get; set; }
        public Guid RangeId { get; set; }
        public string? DeliveryPeriod { get; set; }
        public bool Inactive { get; set; }
    }
}

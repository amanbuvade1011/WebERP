namespace NicheWebErpAPI.Dtos
{
    public class StyleListItemDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? CategoryName { get; set; }
        public string? LabelName { get; set; }
        public string? RangeName { get; set; }
        public bool Inactive { get; set; }
    }
}

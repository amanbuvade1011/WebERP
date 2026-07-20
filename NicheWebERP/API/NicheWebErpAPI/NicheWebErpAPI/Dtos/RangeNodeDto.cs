namespace NicheWebErpAPI.Dtos
{
    public class RangeNodeDto
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public List<RangeNodeDto> Children { get; set; } = new();
    }
}

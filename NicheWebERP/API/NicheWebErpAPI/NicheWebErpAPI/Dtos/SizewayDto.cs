namespace NicheWebErpAPI.Dtos
{
    public class SizewayDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public bool ExcludeRetailSearch { get; set; }
        public List<SizewayItemDto> Sizes { get; set; } = new();
    }

    public class SizewayItemDto
    {
        public Guid SizeId { get; set; }
        public string SizeDescription { get; set; } = null!;
        public int Sequence { get; set; }
    }
}

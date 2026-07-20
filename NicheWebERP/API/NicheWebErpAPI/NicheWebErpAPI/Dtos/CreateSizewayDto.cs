namespace NicheWebErpAPI.Dtos
{
    public class CreateSizewayDto
    {
        public string Description { get; set; } = null!;
        public bool ExcludeRetailSearch { get; set; }

        // Ordered - the position in this list becomes the SizewayItem.Sequence.
        public List<Guid> SizeIds { get; set; } = new();
    }
}

namespace NicheWebErpAPI.Dtos
{
    public class UpdateSizeSequenceDto
    {
        // Full ordered replacement - position in this list becomes the new Sequence.
        public List<Guid> SizeIds { get; set; } = new();
    }
}

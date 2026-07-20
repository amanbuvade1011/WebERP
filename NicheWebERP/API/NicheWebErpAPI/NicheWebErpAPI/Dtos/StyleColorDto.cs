namespace NicheWebErpAPI.Dtos
{
    public class StyleColorDto
    {
        public Guid Id { get; set; }
        public string Color { get; set; } = null!;
        public string? RgbValue { get; set; }
        public bool Inactive { get; set; }
    }
}

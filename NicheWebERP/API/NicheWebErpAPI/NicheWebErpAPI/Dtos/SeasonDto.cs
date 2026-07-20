namespace NicheWebErpAPI.Dtos
{
    public class SeasonDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}

namespace NicheWebErpAPI.Dtos
{
    public class CreateLocationDto
    {
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public Guid ParentId { get; set; }
        public Guid? CountryId { get; set; }
    }
}

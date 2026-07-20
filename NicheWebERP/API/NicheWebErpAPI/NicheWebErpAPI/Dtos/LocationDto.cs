namespace NicheWebErpAPI.Dtos
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public Guid? ParentId { get; set; }
        public bool Inactive { get; set; }
    }
}

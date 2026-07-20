namespace NicheWebErpAPI.Dtos
{
    public class CategoryNodeDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public bool Inactive { get; set; }
        public List<CategoryNodeDto> Children { get; set; } = new();
    }
}

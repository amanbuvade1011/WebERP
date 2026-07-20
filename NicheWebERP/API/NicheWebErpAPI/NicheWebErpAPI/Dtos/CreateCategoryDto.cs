namespace NicheWebErpAPI.Dtos
{
    public class CreateCategoryDto
    {
        public string Description { get; set; } = null!;
        public Guid? ParentId { get; set; }
    }
}

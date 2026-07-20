namespace NicheWebErpAPI.Dtos
{
    public class UpdateErpUserDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public Guid? LocationId { get; set; }
        public bool IsActive { get; set; }
    }
}

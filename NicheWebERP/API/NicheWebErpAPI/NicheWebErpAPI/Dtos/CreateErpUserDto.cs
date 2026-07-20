namespace NicheWebErpAPI.Dtos
{
    public class CreateErpUserDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public Guid? LocationId { get; set; }
    }
}

namespace NicheWebErpAPI.Dtos
{
    // Shape returned by GET api/Auth/Me and embedded in the login response.
    public class CurrentUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public Guid? LocationId { get; set; }
        public Guid CompanyId { get; set; }
    }
}

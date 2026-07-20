namespace NicheWebErpAPI.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public CurrentUserDto User { get; set; } = null!;
    }
}

using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IAuthService
    {
        // Returns null when the username doesn't exist, the password is wrong, or the user is
        // inactive - callers must not distinguish these cases in the response (don't leak which
        // one it was).
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        Task<CurrentUserDto?> GetCurrentUserAsync(int userId);
    }
}

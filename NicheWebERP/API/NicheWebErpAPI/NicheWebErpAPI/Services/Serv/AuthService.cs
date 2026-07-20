using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NicheWebErpAPI.Services.Serv
{
    public class AuthService : IAuthService
    {
        private readonly IErpUserRepository _erpUserRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IErpUserRepository erpUserRepository, IConfiguration configuration)
        {
            _erpUserRepository = erpUserRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _erpUserRepository.GetByUsernameAsync(request.Username);
            if (user is null || !user.IsActive)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            _erpUserRepository.Update(user);
            await _erpUserRepository.SaveChangesAsync();

            var (token, expiresAtUtc) = BuildToken(user);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                User = ToCurrentUserDto(user)
            };
        }

        public async Task<CurrentUserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _erpUserRepository.GetByIdAsync(userId);
            return user is null ? null : ToCurrentUserDto(user);
        }

        private (string token, DateTime expiresAtUtc) BuildToken(ErpUser user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"]!;
            var issuer = jwtSection["Issuer"]!;
            var audience = jwtSection["Audience"]!;
            var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"] ?? "480");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role?.Name ?? string.Empty),
                new("legacyPersonId", user.LegacyPersonId.ToString()),
                new("companyId", user.CompanyId.ToString()),
                new("roleId", user.RoleId.ToString())
            };

            if (user.LocationId.HasValue)
            {
                claims.Add(new Claim("locationId", user.LocationId.Value.ToString()));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
        }

        private static CurrentUserDto ToCurrentUserDto(ErpUser user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name ?? string.Empty,
            LocationId = user.LocationId,
            CompanyId = user.CompanyId
        };
    }
}

using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class ErpUserService : IErpUserService
    {
        private readonly IErpUserRepository _erpUserRepository;
        private readonly IErpRoleRepository _erpRoleRepository;
        private readonly ICompanyLocationRepository _companyLocationRepository;
        private readonly ICurrentUserService _currentUserService;

        public ErpUserService(
            IErpUserRepository erpUserRepository,
            IErpRoleRepository erpRoleRepository,
            ICompanyLocationRepository companyLocationRepository,
            ICurrentUserService currentUserService)
        {
            _erpUserRepository = erpUserRepository;
            _erpRoleRepository = erpRoleRepository;
            _companyLocationRepository = companyLocationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ErpUserDto>> GetAllUsersAsync()
        {
            var users = await _erpUserRepository.GetAllAsync();
            return users.Select(ToDto);
        }

        public async Task<ErpUserDto> CreateUserAsync(CreateErpUserDto dto)
        {
            if (await _erpUserRepository.UsernameExistsAsync(dto.Username))
            {
                throw new InvalidOperationException($"Username '{dto.Username}' is already taken.");
            }

            if (await _erpUserRepository.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");
            }

            if (!await _erpRoleRepository.ExistsAsync(dto.RoleId))
            {
                throw new InvalidOperationException($"Role {dto.RoleId} does not exist.");
            }

            var companyId = await _companyLocationRepository.GetMasterCompanyIdAsync();

            var user = new ErpUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                LocationId = dto.LocationId,
                CompanyId = companyId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                // Generated once, never reused - see docs/ai-plan/01-database-map.md.
                LegacyPersonId = Guid.NewGuid(),
                UpdatedByUserId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null
            };

            await _erpUserRepository.AddAsync(user);
            await _erpUserRepository.SaveChangesAsync();

            var created = await _erpUserRepository.GetByIdAsync(user.Id)
                ?? throw new InvalidOperationException("User was created but could not be re-read.");
            return ToDto(created);
        }

        public async Task<ErpUserDto> UpdateUserAsync(int id, UpdateErpUserDto dto)
        {
            var user = await _erpUserRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"ErpUser {id} not found.");

            if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase)
                && await _erpUserRepository.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");
            }

            if (!await _erpRoleRepository.ExistsAsync(dto.RoleId))
            {
                throw new InvalidOperationException($"Role {dto.RoleId} does not exist.");
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.RoleId = dto.RoleId;
            user.LocationId = dto.LocationId;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null;

            _erpUserRepository.Update(user);
            await _erpUserRepository.SaveChangesAsync();

            var updated = await _erpUserRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("User was updated but could not be re-read.");
            return ToDto(updated);
        }

        public async Task ResetPasswordAsync(int id, string newPassword)
        {
            var user = await _erpUserRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"ErpUser {id} not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null;

            _erpUserRepository.Update(user);
            await _erpUserRepository.SaveChangesAsync();
        }

        private static ErpUserDto ToDto(ErpUser user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name ?? string.Empty,
            LocationId = user.LocationId,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt
        };
    }
}

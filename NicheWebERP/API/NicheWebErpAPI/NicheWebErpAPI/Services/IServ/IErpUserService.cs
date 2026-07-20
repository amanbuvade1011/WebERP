using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IErpUserService
    {
        Task<IEnumerable<ErpUserDto>> GetAllUsersAsync();
        Task<ErpUserDto> CreateUserAsync(CreateErpUserDto dto);
        Task<ErpUserDto> UpdateUserAsync(int id, UpdateErpUserDto dto);
        Task ResetPasswordAsync(int id, string newPassword);
    }
}

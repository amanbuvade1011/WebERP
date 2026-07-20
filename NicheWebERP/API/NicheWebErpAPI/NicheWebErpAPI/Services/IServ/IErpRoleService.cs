using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IErpRoleService
    {
        Task<IEnumerable<ErpRoleDto>> GetAllRolesAsync();
    }
}

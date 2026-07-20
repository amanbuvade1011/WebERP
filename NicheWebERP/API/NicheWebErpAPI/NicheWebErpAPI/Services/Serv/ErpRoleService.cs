using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class ErpRoleService : IErpRoleService
    {
        private readonly IErpRoleRepository _erpRoleRepository;

        public ErpRoleService(IErpRoleRepository erpRoleRepository)
        {
            _erpRoleRepository = erpRoleRepository;
        }

        public async Task<IEnumerable<ErpRoleDto>> GetAllRolesAsync()
        {
            var roles = await _erpRoleRepository.GetAllAsync();
            return roles.Select(r => new ErpRoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });
        }
    }
}

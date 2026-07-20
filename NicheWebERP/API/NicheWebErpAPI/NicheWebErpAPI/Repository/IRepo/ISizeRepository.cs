using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface ISizeRepository
    {
        Task<List<SizeDto>> GetAllAsync(Guid companyId);
    }
}

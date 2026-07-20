using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface ISeasonRepository
    {
        Task<List<SeasonDto>> GetAllAsync(Guid companyId);
    }
}

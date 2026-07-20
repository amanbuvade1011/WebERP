using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface ITradingTermsRepository
    {
        Task<List<TradingTermsDto>> GetAllAsync(Guid companyId);
    }
}

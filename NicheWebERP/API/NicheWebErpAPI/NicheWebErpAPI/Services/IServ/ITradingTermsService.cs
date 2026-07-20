using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface ITradingTermsService
    {
        Task<List<TradingTermsDto>> GetAllAsync();
    }
}

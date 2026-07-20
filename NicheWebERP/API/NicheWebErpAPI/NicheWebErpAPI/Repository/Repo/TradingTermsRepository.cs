using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class TradingTermsRepository : ITradingTermsRepository
    {
        private readonly IEfCoreService<Models.TradingTerms> _termsService;

        public TradingTermsRepository(IEfCoreService<Models.TradingTerms> termsService)
        {
            _termsService = termsService;
        }

        public async Task<List<TradingTermsDto>> GetAllAsync(Guid companyId) =>
            await _termsService.Query()
                .Where(t => t.CompanyID == companyId)
                .OrderBy(t => t.Description)
                .Select(t => new TradingTermsDto
                {
                    Id = t.EntityID,
                    Description = t.Description,
                    NumberOfDays = t.NumberOfDays,
                    DiscountDays = t.DiscountDays,
                    SettlementDiscountPercent = t.SettlementDiscountPercent
                })
                .ToListAsync();
    }
}

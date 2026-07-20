using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class FirmRepository : IFirmRepository
    {
        private readonly IEfCoreService<Firm> _firmService;
        private readonly IEfCoreService<TradingTerms> _termsService;
        private readonly IEfCoreService<PricePoint> _pricePointService;

        public FirmRepository(
            IEfCoreService<Firm> firmService,
            IEfCoreService<TradingTerms> termsService,
            IEfCoreService<PricePoint> pricePointService)
        {
            _firmService = firmService;
            _termsService = termsService;
            _pricePointService = pricePointService;
        }

        public async Task<PagedResultDto<FirmListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, string? search, string? entityClassName)
        {
            var query = _firmService.Query().Where(f => f.CompanyID == companyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(f => f.TradingName.Contains(term) || (f.Code != null && f.Code.Contains(term)));
            }
            if (!string.IsNullOrWhiteSpace(entityClassName))
            {
                query = query.Where(f => f.EntityClassName == entityClassName);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(f => f.TradingName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FirmListItemDto
                {
                    Id = f.EntityID,
                    Code = f.Code,
                    TradingName = f.TradingName,
                    EntityClassName = f.EntityClassName,
                    CreditLimit = f.CreditLimit,
                    Inactive = f.Inactive
                })
                .ToListAsync();

            return new PagedResultDto<FirmListItemDto> { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
        }

        public Task<Firm?> GetEntityByIdAsync(Guid companyId, Guid id) =>
            _firmService.Query().FirstOrDefaultAsync(f => f.CompanyID == companyId && f.EntityID == id);

        public async Task<FirmDetailDto?> GetDetailByIdAsync(Guid companyId, Guid id)
        {
            var firm = await GetEntityByIdAsync(companyId, id);
            if (firm is null)
            {
                return null;
            }

            var terms = await _termsService.Query().FirstOrDefaultAsync(t => t.CompanyID == companyId && t.EntityID == firm.TermsID);
            var pricePoint = await _pricePointService.Query().FirstOrDefaultAsync(p => p.CompanyID == companyId && p.EntityID == firm.PricePointID);

            return new FirmDetailDto
            {
                Id = firm.EntityID,
                Code = firm.Code,
                TradingName = firm.TradingName,
                CompanyName = firm.CompanyName,
                EntityClassName = firm.EntityClassName,
                Address = firm.Address,
                Suburb = firm.Suburb,
                State = firm.State,
                Postcode = firm.Postcode,
                CountryId = firm.CountryID,
                Phone1 = firm.Phone1,
                Phone2 = firm.Phone2,
                GeneralEmail = firm.GeneralEmail,
                Fax = firm.Fax,
                TermsId = firm.TermsID,
                TermsName = terms?.Description,
                PricePointId = firm.PricePointID,
                PricePointName = pricePoint?.Name,
                CreditLimit = firm.CreditLimit,
                DiscountPercent1 = firm.DiscountPercent1,
                AllowOrder = firm.AllowOrder,
                AllowInvoice = firm.AllowInvoice,
                DepositPercent = firm.DepositPercent,
                Inactive = firm.Inactive
            };
        }

        public Task AddAsync(Firm firm) => _firmService.AddAsync(firm);

        public void Update(Firm firm) => _firmService.Update(firm);

        public Task<int> SaveChangesAsync() => _firmService.SaveChangesAsync();
    }
}

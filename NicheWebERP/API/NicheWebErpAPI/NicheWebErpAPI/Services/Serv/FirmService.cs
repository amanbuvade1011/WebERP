using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class FirmService : IFirmService
    {
        private readonly IFirmRepository _firmRepository;
        private readonly ICurrentUserService _currentUserService;

        public FirmService(IFirmRepository firmRepository, ICurrentUserService currentUserService)
        {
            _firmRepository = firmRepository;
            _currentUserService = currentUserService;
        }

        public Task<PagedResultDto<FirmListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? entityClassName)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;
            return _firmRepository.GetPagedAsync(_currentUserService.CompanyId, page, pageSize, search, entityClassName);
        }

        public async Task<FirmDetailDto> GetByIdAsync(Guid id) =>
            await _firmRepository.GetDetailByIdAsync(_currentUserService.CompanyId, id)
                ?? throw new KeyNotFoundException($"Firm {id} not found.");

        public async Task<FirmDetailDto> CreateAsync(CreateFirmDto dto)
        {
            var companyId = _currentUserService.CompanyId;

            var firm = new Firm
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                EntityClassName = dto.EntityClassName,
                TradingName = dto.TradingName,
                CompanyName = dto.CompanyName,
                Code = string.IsNullOrWhiteSpace(dto.Code) ? Guid.NewGuid().ToString("N")[..8].ToUpperInvariant() : dto.Code,
                Address = dto.Address,
                Suburb = dto.Suburb,
                State = dto.State,
                Postcode = dto.Postcode,
                CountryID = dto.CountryId ?? Guid.Empty,
                Phone1 = dto.Phone1,
                Phone2 = dto.Phone2,
                GeneralEmail = dto.GeneralEmail,
                TermsID = dto.TermsId ?? Guid.Empty,
                PricePointID = dto.PricePointId ?? Guid.Empty,
                CreditLimit = dto.CreditLimit,
                DiscountPercent1 = dto.DiscountPercent1,
                AllowOrder = dto.AllowOrder,
                AllowInvoice = dto.AllowInvoice,
                DepositPercent = dto.DepositPercent,
                Inactive = false,
                LastUpdated = DateTime.UtcNow,
                UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty
            };

            await _firmRepository.AddAsync(firm);
            await _firmRepository.SaveChangesAsync();

            return await GetByIdAsync(firm.EntityID);
        }

        public async Task<FirmDetailDto> UpdateAsync(Guid id, UpdateFirmDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var firm = await _firmRepository.GetEntityByIdAsync(companyId, id)
                ?? throw new KeyNotFoundException($"Firm {id} not found.");

            firm.TradingName = dto.TradingName;
            firm.CompanyName = dto.CompanyName;
            firm.Address = dto.Address;
            firm.Suburb = dto.Suburb;
            firm.State = dto.State;
            firm.Postcode = dto.Postcode;
            firm.CountryID = dto.CountryId ?? firm.CountryID;
            firm.Phone1 = dto.Phone1;
            firm.Phone2 = dto.Phone2;
            firm.GeneralEmail = dto.GeneralEmail;
            firm.TermsID = dto.TermsId ?? firm.TermsID;
            firm.PricePointID = dto.PricePointId ?? firm.PricePointID;
            firm.CreditLimit = dto.CreditLimit;
            firm.DiscountPercent1 = dto.DiscountPercent1;
            firm.AllowOrder = dto.AllowOrder;
            firm.AllowInvoice = dto.AllowInvoice;
            firm.DepositPercent = dto.DepositPercent;
            firm.Inactive = dto.Inactive;
            firm.LastUpdated = DateTime.UtcNow;
            firm.UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : firm.UpdatedByID;

            _firmRepository.Update(firm);
            await _firmRepository.SaveChangesAsync();

            return await GetByIdAsync(id);
        }
    }
}

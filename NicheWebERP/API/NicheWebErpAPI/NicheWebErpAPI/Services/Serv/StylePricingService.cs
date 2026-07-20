using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class StylePricingService : IStylePricingService
    {
        private readonly IStylePricingRepository _pricingRepository;
        private readonly ICompanyLocationRepository _companyLocationRepository;
        private readonly ICurrentUserService _currentUserService;

        public StylePricingService(
            IStylePricingRepository pricingRepository,
            ICompanyLocationRepository companyLocationRepository,
            ICurrentUserService currentUserService)
        {
            _pricingRepository = pricingRepository;
            _companyLocationRepository = companyLocationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<PricePointDto>> GetAllPricePointsAsync()
        {
            var points = await _pricingRepository.GetAllPricePointsAsync(_currentUserService.CompanyId);
            return points.Select(p => new PricePointDto { Id = p.EntityID, Name = p.Name }).ToList();
        }

        public async Task<List<StylePriceDto>> GetPricesAsync(Guid styleId)
        {
            var companyId = _currentUserService.CompanyId;
            var prices = await _pricingRepository.GetStylePricesAsync(companyId, styleId);
            var pricePoints = await _pricingRepository.GetAllPricePointsAsync(companyId);
            var nameById = pricePoints.ToDictionary(p => p.EntityID, p => p.Name);

            return prices.Select(p => ToDto(p, nameById.GetValueOrDefault(p.PricePointID))).ToList();
        }

        public async Task<List<StylePriceDto>> UpdatePricesAsync(Guid styleId, UpdateStylePricesDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            foreach (var line in dto.Prices)
            {
                if (!await _pricingRepository.PricePointExistsAsync(companyId, line.PricePointId))
                {
                    throw new InvalidOperationException($"Price point {line.PricePointId} does not exist.");
                }

                var existing = await _pricingRepository.GetStylePriceAsync(companyId, styleId, line.PricePointId);
                if (existing is null)
                {
                    await _pricingRepository.AddStylePriceAsync(new StylePrice
                    {
                        CompanyID = companyId,
                        EntityID = Guid.NewGuid(),
                        StyleID = styleId,
                        PricePointID = line.PricePointId,
                        LocalUnitPriceExTax1 = line.LocalUnitPriceExTax1,
                        LocalUnitPriceTax1 = line.LocalUnitPriceTax1,
                        InternationalUnitPriceExTax1 = line.InternationalUnitPriceExTax1,
                        InternationalUnitPriceTax1 = line.InternationalUnitPriceTax1,
                        LastUpdated = DateTime.UtcNow,
                        UpdatedByID = updatedById
                    });
                }
                else
                {
                    existing.LocalUnitPriceExTax1 = line.LocalUnitPriceExTax1;
                    existing.LocalUnitPriceTax1 = line.LocalUnitPriceTax1;
                    existing.InternationalUnitPriceExTax1 = line.InternationalUnitPriceExTax1;
                    existing.InternationalUnitPriceTax1 = line.InternationalUnitPriceTax1;
                    existing.LastUpdated = DateTime.UtcNow;
                    existing.UpdatedByID = updatedById;
                    _pricingRepository.UpdateStylePrice(existing);
                }
            }

            await _pricingRepository.SaveChangesAsync();
            return await GetPricesAsync(styleId);
        }

        public async Task<List<StyleSellLocationDto>> GetSellLocationsAsync(Guid styleId)
        {
            var companyId = _currentUserService.CompanyId;
            var locations = await _pricingRepository.GetSellLocationsAsync(companyId, styleId);
            var allLocations = await _companyLocationRepository.GetAllByCompanyAsync(companyId);
            var nameById = allLocations.ToDictionary(l => l.EntityID, l => l.Name);

            return locations.Select(l => ToDto(l, nameById.GetValueOrDefault(l.LocationID))).ToList();
        }

        public async Task<List<StyleSellLocationDto>> UpdateSellLocationsAsync(Guid styleId, UpdateSellLocationsDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            foreach (var line in dto.Locations)
            {
                var existing = await _pricingRepository.GetSellLocationAsync(companyId, styleId, line.LocationId);
                if (existing is null)
                {
                    await _pricingRepository.AddSellLocationAsync(new StyleSellLocation
                    {
                        CompanyID = companyId,
                        EntityID = Guid.NewGuid(),
                        StyleID = styleId,
                        LocationID = line.LocationId,
                        AllowRetail = BoolToByte(line.AllowRetail),
                        AllowWebRetail = BoolToByte(line.AllowWebRetail),
                        AllowRental = BoolToByte(line.AllowRental),
                        AllowWholesaleIndent = BoolToByte(line.AllowWholesaleIndent),
                        LastUpdated = DateTime.UtcNow,
                        UpdatedByID = updatedById
                    });
                }
                else
                {
                    existing.AllowRetail = BoolToByte(line.AllowRetail);
                    existing.AllowWebRetail = BoolToByte(line.AllowWebRetail);
                    existing.AllowRental = BoolToByte(line.AllowRental);
                    existing.AllowWholesaleIndent = BoolToByte(line.AllowWholesaleIndent);
                    existing.LastUpdated = DateTime.UtcNow;
                    existing.UpdatedByID = updatedById;
                    _pricingRepository.UpdateSellLocation(existing);
                }
            }

            await _pricingRepository.SaveChangesAsync();
            return await GetSellLocationsAsync(styleId);
        }

        private static byte BoolToByte(bool value) => (byte)(value ? 1 : 0);

        private static StylePriceDto ToDto(StylePrice p, string? pricePointName) => new()
        {
            PricePointId = p.PricePointID,
            PricePointName = pricePointName,
            LocalUnitPriceExTax1 = p.LocalUnitPriceExTax1,
            LocalUnitPriceTax1 = p.LocalUnitPriceTax1,
            InternationalUnitPriceExTax1 = p.InternationalUnitPriceExTax1,
            InternationalUnitPriceTax1 = p.InternationalUnitPriceTax1
        };

        private static StyleSellLocationDto ToDto(StyleSellLocation l, string? locationName) => new()
        {
            LocationId = l.LocationID,
            LocationName = locationName,
            AllowRetail = l.AllowRetail != 0,
            AllowWebRetail = l.AllowWebRetail != 0,
            AllowRental = l.AllowRental != 0,
            AllowWholesaleIndent = l.AllowWholesaleIndent != 0
        };
    }
}

using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyLocationRepository _companyLocationRepository;
        private readonly ICurrentUserService _currentUserService;

        public CompanyService(
            ICompanyLocationRepository companyLocationRepository,
            ICurrentUserService currentUserService)
        {
            _companyLocationRepository = companyLocationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<CompanyDto> GetCurrentCompanyAsync()
        {
            var company = await _companyLocationRepository.GetMasterCompanyAsync()
                ?? throw new InvalidOperationException("No master company is configured.");
            return ToDto(company);
        }

        public async Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyDto dto)
        {
            var company = await _companyLocationRepository.GetMasterCompanyAsync()
                ?? throw new InvalidOperationException("No master company is configured.");

            company.Name = dto.Name;
            company.Address = dto.Address;
            company.Suburb = dto.Suburb;
            company.State = dto.State;
            company.Postcode = dto.Postcode;
            if (dto.CountryId.HasValue)
            {
                company.CountryID = dto.CountryId.Value;
            }
            company.Phone1 = dto.Phone1;
            company.Phone2 = dto.Phone2;
            company.Fax = dto.Fax;
            company.GeneralEmail = dto.GeneralEmail;
            company.CompanyNumber1 = dto.CompanyNumber1;
            company.CompanyNumber2 = dto.CompanyNumber2;
            company.LastUpdated = DateTime.UtcNow;
            if (_currentUserService.IsAuthenticated)
            {
                company.UpdatedByID = _currentUserService.LegacyPersonId;
            }

            _companyLocationRepository.Update(company);
            await _companyLocationRepository.SaveChangesAsync();

            return ToDto(company);
        }

        private static CompanyDto ToDto(CompanyLocation c) => new()
        {
            CompanyId = c.CompanyID,
            EntityId = c.EntityID,
            Name = c.Name,
            Code = c.Code,
            Address = c.Address,
            Suburb = c.Suburb,
            State = c.State,
            Postcode = c.Postcode,
            CountryId = c.CountryID,
            Phone1 = c.Phone1,
            Phone2 = c.Phone2,
            Fax = c.Fax,
            GeneralEmail = c.GeneralEmail,
            CompanyNumber1 = c.CompanyNumber1,
            CompanyNumber2 = c.CompanyNumber2
        };
    }
}

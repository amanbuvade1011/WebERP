using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class RetailCustomerService : IRetailCustomerService
    {
        private readonly IRetailCustomerRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public RetailCustomerService(IRetailCustomerRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public Task<PagedResultDto<RetailCustomerListItemDto>> GetPagedAsync(int page, int pageSize, string? search)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;
            return _repository.GetPagedAsync(_currentUserService.CompanyId, page, pageSize, search);
        }

        public async Task<RetailCustomerDetailDto> CreateAsync(CreateRetailCustomerDto dto)
        {
            var companyId = _currentUserService.CompanyId;

            var person = new Person
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                EntityClassName = "RetailCustomer",
                // Never surfaced in the API - just satisfies the unique index on UserName
                // (see Person.cs). Retail customers don't log in through this record.
                UserName = $"retail-{Guid.NewGuid():N}",
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Suburb = dto.Suburb,
                State = dto.State,
                PostCode = dto.PostCode,
                CountryId = dto.CountryId,
                IsSuspended = false,
                DiscountPercent = 0,
                CreationDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty
            };

            await _repository.AddAsync(person);
            await _repository.SaveChangesAsync();

            return new RetailCustomerDetailDto
            {
                Id = person.EntityID,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.Email,
                PhoneNumber = person.PhoneNumber,
                Address = person.Address,
                Suburb = person.Suburb,
                State = person.State,
                PostCode = person.PostCode,
                CountryId = person.CountryId,
                DiscountPercent = person.DiscountPercent,
                CreditLimit = person.CreditLimit,
                AllowCredit = person.AllowCredit,
                IsSuspended = person.IsSuspended
            };
        }
    }
}

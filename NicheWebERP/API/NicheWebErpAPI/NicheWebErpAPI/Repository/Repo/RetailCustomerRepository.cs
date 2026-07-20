using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    // Only ever touches Person rows where EntityClassName = RetailCustomer - see
    // docs/ai-plan/01-database-map.md for why Person is shared with Representative/Employee.
    public class RetailCustomerRepository : IRetailCustomerRepository
    {
        private const string RetailCustomerClass = "RetailCustomer";

        private readonly IEfCoreService<Person> _personService;

        public RetailCustomerRepository(IEfCoreService<Person> personService)
        {
            _personService = personService;
        }

        public async Task<PagedResultDto<RetailCustomerListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, string? search)
        {
            var query = _personService.Query()
                .Where(p => p.CompanyID == companyId && p.EntityClassName == RetailCustomerClass);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(p =>
                    (p.FirstName != null && p.FirstName.Contains(term)) ||
                    (p.LastName != null && p.LastName.Contains(term)) ||
                    (p.Email != null && p.Email.Contains(term)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new RetailCustomerListItemDto
                {
                    Id = p.EntityID,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    IsSuspended = p.IsSuspended
                })
                .ToListAsync();

            return new PagedResultDto<RetailCustomerListItemDto> { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
        }

        public Task<Person?> GetEntityByIdAsync(Guid companyId, Guid id) =>
            _personService.Query().FirstOrDefaultAsync(p => p.CompanyID == companyId && p.EntityID == id && p.EntityClassName == RetailCustomerClass);

        public Task AddAsync(Person person) => _personService.AddAsync(person);

        public Task<int> SaveChangesAsync() => _personService.SaveChangesAsync();
    }
}

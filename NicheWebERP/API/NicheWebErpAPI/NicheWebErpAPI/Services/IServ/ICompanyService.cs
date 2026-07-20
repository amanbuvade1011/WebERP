using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface ICompanyService
    {
        Task<CompanyDto> GetCurrentCompanyAsync();
        Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyDto dto);
    }
}

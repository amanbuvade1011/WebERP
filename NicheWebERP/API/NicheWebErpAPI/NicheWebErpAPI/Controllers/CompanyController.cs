using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // GET api/Company/GetCurrentCompany
        [HttpGet("GetCurrentCompany")]
        public async Task<ActionResult<CompanyDto>> GetCurrentCompany()
        {
            var company = await _companyService.GetCurrentCompanyAsync();
            return Ok(company);
        }

        // PUT api/Company/UpdateCompany
        [HttpPut("UpdateCompany")]
        public async Task<ActionResult<CompanyDto>> UpdateCompany(UpdateCompanyDto dto)
        {
            var updated = await _companyService.UpdateCompanyAsync(dto);
            return Ok(updated);
        }
    }
}

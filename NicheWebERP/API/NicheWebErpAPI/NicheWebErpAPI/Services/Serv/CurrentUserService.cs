using Microsoft.AspNetCore.Http;
using NicheWebErpAPI.Services.IServ;
using System.Security.Claims;

namespace NicheWebErpAPI.Services.Serv
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public int UserId => int.Parse(RequireClaim(ClaimTypes.NameIdentifier));

        public Guid LegacyPersonId => Guid.Parse(RequireClaim("legacyPersonId"));

        public Guid CompanyId => Guid.Parse(RequireClaim("companyId"));

        public int RoleId => int.Parse(RequireClaim("roleId"));

        public Guid? LocationId
        {
            get
            {
                var value = User?.FindFirstValue("locationId");
                return string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
            }
        }

        private string RequireClaim(string claimType)
        {
            var value = User?.FindFirstValue(claimType);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(
                    $"No authenticated user in the current request context (missing claim '{claimType}').");
            }
            return value;
        }
    }
}

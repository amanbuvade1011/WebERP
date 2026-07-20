namespace NicheWebErpAPI.Services.IServ
{
    // Reads the authenticated ErpUser's identity out of the current request's JWT claims.
    // Every write endpoint in every module (Product, Sales, Manufacturing, Finance...) should
    // use LegacyPersonId (not UserId) to populate that table's UpdatedByID column - see
    // docs/ai-plan/01-database-map.md for why ErpUser.Id (int) can't be used there directly.
    public interface ICurrentUserService
    {
        bool IsAuthenticated { get; }
        int UserId { get; }
        Guid LegacyPersonId { get; }
        Guid CompanyId { get; }
        int RoleId { get; }
        Guid? LocationId { get; }
    }
}

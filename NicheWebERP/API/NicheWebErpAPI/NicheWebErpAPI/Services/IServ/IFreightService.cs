namespace NicheWebErpAPI.Services.IServ
{
    public interface IFreightService
    {
        Task<decimal> CalculateAsync(Guid locationId, int quantity, Guid? countryId);
    }
}

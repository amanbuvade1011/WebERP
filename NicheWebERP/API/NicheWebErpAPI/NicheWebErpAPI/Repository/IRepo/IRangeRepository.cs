namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IRangeRepository
    {
        Task<List<Models.Range>> GetAllAsync(Guid companyId);
    }
}

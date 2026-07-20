using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IRangeService
    {
        Task<List<RangeNodeDto>> GetTreeAsync();
    }
}

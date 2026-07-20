using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface ISeasonService
    {
        Task<List<SeasonDto>> GetAllAsync();
    }
}

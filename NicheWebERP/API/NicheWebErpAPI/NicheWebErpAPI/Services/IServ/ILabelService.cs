using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface ILabelService
    {
        Task<List<LabelDto>> GetAllAsync();
    }
}

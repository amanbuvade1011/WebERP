using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface ILabelRepository
    {
        Task<List<LabelDto>> GetAllAsync(Guid companyId);
    }
}

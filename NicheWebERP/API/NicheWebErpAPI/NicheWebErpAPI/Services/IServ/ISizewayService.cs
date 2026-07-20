using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface ISizewayService
    {
        Task<List<SizewayDto>> GetAllAsync();
        Task<SizewayDto> CreateAsync(CreateSizewayDto dto);
        Task<SizewayDto> UpdateSizeSequenceAsync(Guid sizewayId, UpdateSizeSequenceDto dto);
    }
}

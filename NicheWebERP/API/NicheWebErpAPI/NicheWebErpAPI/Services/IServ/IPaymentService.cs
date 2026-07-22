using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IPaymentService
    {
        Task<InvoiceDetailDto> RecordPaymentAsync(RecordPaymentDto dto);
    }
}

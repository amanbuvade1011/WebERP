using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IPaymentMethodRepository
    {
        Task<List<PaymentMethodDto>> GetAllWholesaleAsync(Guid companyId);
    }
}

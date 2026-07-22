namespace NicheWebErpAPI.Dtos
{
    public class PaymentMethodDto
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public int PaymentMethodType { get; set; }
        public decimal SurchargePercentage { get; set; }
    }
}

namespace NicheWebErpAPI.Dtos
{
    public class StylePriceDto
    {
        public Guid PricePointId { get; set; }
        public string? PricePointName { get; set; }
        public decimal LocalUnitPriceExTax1 { get; set; }
        public decimal LocalUnitPriceTax1 { get; set; }
        public decimal InternationalUnitPriceExTax1 { get; set; }
        public decimal InternationalUnitPriceTax1 { get; set; }
    }

    public class UpdateStylePricesDto
    {
        public List<StylePriceLineDto> Prices { get; set; } = new();
    }

    public class StylePriceLineDto
    {
        public Guid PricePointId { get; set; }
        public decimal LocalUnitPriceExTax1 { get; set; }
        public decimal LocalUnitPriceTax1 { get; set; }
        public decimal InternationalUnitPriceExTax1 { get; set; }
        public decimal InternationalUnitPriceTax1 { get; set; }
    }
}

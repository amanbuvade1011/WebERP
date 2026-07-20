namespace NicheWebErpAPI.Dtos
{
    public class RetailCustomerListItemDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsSuspended { get; set; }
    }

    public class RetailCustomerDetailDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public Guid? CountryId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool? AllowCredit { get; set; }
        public bool IsSuspended { get; set; }
    }

    public class CreateRetailCustomerDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public Guid? CountryId { get; set; }
    }
}

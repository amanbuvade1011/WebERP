namespace NicheWebErpAPI.Dtos
{
    public class UpdateCompanyDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public Guid? CountryId { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Fax { get; set; }
        public string? GeneralEmail { get; set; }
        public string? CompanyNumber1 { get; set; }
        public string? CompanyNumber2 { get; set; }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesCustomerMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Firm",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TradingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainContactID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    PricePointID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suburb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyNumber1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyNumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParentFirmID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowOrder = table.Column<bool>(type: "bit", nullable: false),
                    AllowInvoice = table.Column<bool>(type: "bit", nullable: false),
                    DepositPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firm", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suburb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AllowCredit = table.Column<bool>(type: "bit", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "TradingTerms",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfDays = table.Column<int>(type: "int", nullable: false),
                    DaysFrom1 = table.Column<int>(type: "int", nullable: false),
                    DiscountDays = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SettlementDiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingTerms", x => new { x.CompanyID, x.EntityID });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Firm");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "TradingTerms");
        }
    }
}

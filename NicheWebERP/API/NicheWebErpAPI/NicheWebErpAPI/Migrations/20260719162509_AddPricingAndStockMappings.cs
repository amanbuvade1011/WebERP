using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingAndStockMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricePoint",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxJurisdictionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxInclusive = table.Column<bool>(type: "bit", nullable: true),
                    DefaultLocalTaxJurisdictionCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultInternationalTaxJurisdictionCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultLocalSalesRevenueGeneralLedgerAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultInternationalSalesRevenueGeneralLedgerAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePoint", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "StylePrice",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PricePointID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StyleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalTaxJurisdictionCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InternationalTaxJurisdictionCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocalSalesRevenueGeneralLedgerAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InternationalSalesRevenueGeneralLedgerAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocalUnitPriceExTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocalUnitPriceTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InternationalUnitPriceExTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InternationalUnitPriceTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StylePrice", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "StyleSellLocation",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StyleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowRetail = table.Column<byte>(type: "tinyint", nullable: false),
                    AllowWebRetail = table.Column<byte>(type: "tinyint", nullable: false),
                    AllowRental = table.Column<byte>(type: "tinyint", nullable: false),
                    AllowWholesaleIndent = table.Column<byte>(type: "tinyint", nullable: false),
                    AllowWebWholesaleIndent = table.Column<byte>(type: "tinyint", nullable: false),
                    AllowWebWholesaleStock = table.Column<byte>(type: "tinyint", nullable: false),
                    PricePointRRPID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PricePointWebID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PricePointRetailID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowRetailPreorder = table.Column<bool>(type: "bit", nullable: false),
                    AllowRetailLayBy = table.Column<bool>(type: "bit", nullable: false),
                    AllowWebRetailCatalogue = table.Column<byte>(type: "tinyint", nullable: false),
                    AllowWebRetailStock = table.Column<bool>(type: "bit", nullable: false),
                    AllowWebRetailPreOrder = table.Column<bool>(type: "bit", nullable: false),
                    AllowWebRetailLayBy = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StyleSellLocation", x => new { x.CompanyID, x.EntityID });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricePoint");

            migrationBuilder.DropTable(
                name: "StylePrice");

            migrationBuilder.DropTable(
                name: "StyleSellLocation");
        }
    }
}

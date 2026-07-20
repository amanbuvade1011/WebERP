using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesOrderTransactionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionBase",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaneID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtherPartyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentNumber1 = table.Column<int>(type: "int", nullable: false),
                    LocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePointID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepresentativeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    TaxAmount1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumLines1 = table.Column<int>(type: "int", nullable: false),
                    TotalQuantities1 = table.Column<int>(type: "int", nullable: false),
                    SubTotalStyles1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotalStylesTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status1 = table.Column<int>(type: "int", nullable: true),
                    Status2 = table.Column<int>(type: "int", nullable: true),
                    CurrencyID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TermsID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerReferenceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionBase", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "TransactionLine",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineAmountExTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntityClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineTaxAmount1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StyleSellLocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StylePriceExTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PricePointID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StylePriceTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LineQuantity1 = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLine", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "TransactionQuantity",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionLineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductLocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Held = table.Column<int>(type: "int", nullable: false),
                    RequiredOut = table.Column<int>(type: "int", nullable: false),
                    Allocated = table.Column<int>(type: "int", nullable: false),
                    SalesOrderFirm = table.Column<int>(type: "int", nullable: false),
                    SalesOrderPerson = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionQuantity", x => new { x.CompanyID, x.EntityID });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionBase");

            migrationBuilder.DropTable(
                name: "TransactionLine");

            migrationBuilder.DropTable(
                name: "TransactionQuantity");
        }
    }
}

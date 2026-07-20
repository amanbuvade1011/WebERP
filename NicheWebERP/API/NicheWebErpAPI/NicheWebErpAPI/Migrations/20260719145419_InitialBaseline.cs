using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeftTag = table.Column<int>(type: "int", nullable: false),
                    RightTag = table.Column<int>(type: "int", nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    WebPageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTagTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTagDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "Label",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    WebPageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTagTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTagDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StyleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StyleColorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SizewayItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inactive = table.Column<bool>(type: "bit", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "ProductLocation",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StyleSellLocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Held = table.Column<int>(type: "int", nullable: false),
                    Allocated = table.Column<int>(type: "int", nullable: false),
                    TransitIn = table.Column<int>(type: "int", nullable: false),
                    RequiredOut = table.Column<int>(type: "int", nullable: false),
                    TransitOut = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrder = table.Column<int>(type: "int", nullable: false),
                    RentalHeld = table.Column<int>(type: "int", nullable: false),
                    RentalOrder = table.Column<int>(type: "int", nullable: false),
                    RentalOut = table.Column<int>(type: "int", nullable: false),
                    SalesOrderFirm = table.Column<int>(type: "int", nullable: false),
                    SalesOrderPerson = table.Column<int>(type: "int", nullable: false),
                    AvailableFirm1 = table.Column<int>(type: "int", nullable: false),
                    AvailablePerson1 = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLocation", x => new { x.CompanyID, x.EntityID });
                });

            migrationBuilder.CreateTable(
                name: "Style",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SizewayID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LabelID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebMainPictureID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RangeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    SortRankMajor = table.Column<int>(type: "int", nullable: false),
                    SortRankMinor = table.Column<int>(type: "int", nullable: false),
                    DeliveryPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonStock = table.Column<bool>(type: "bit", nullable: false),
                    AllowManualPrice = table.Column<bool>(type: "bit", nullable: false),
                    AllowManualDescription = table.Column<bool>(type: "bit", nullable: false),
                    DoPopUp = table.Column<bool>(type: "bit", nullable: false),
                    MetaTagTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTagDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInfo1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInfo2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInfo3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInfo4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInfoVideo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInfoComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Style", x => new { x.CompanyID, x.EntityID });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Label");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductLocation");

            migrationBuilder.DropTable(
                name: "Style");
        }
    }
}

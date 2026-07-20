using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    /// <inheritdoc />
    public partial class ExpandCompanyLocationMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyNumber1",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyNumber2",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPricePointRRPID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPricePointRetailID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPricePointWebID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPricePointWholesaleID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultTaxJurisdictionID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FreightID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "GeneralEmail",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LanguageID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "NumLicenses",
                table: "CompanyLocation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerID",
                table: "CompanyLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone1",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone2",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suburb",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "CompanyNumber1",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "CompanyNumber2",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "DefaultPricePointRRPID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "DefaultPricePointRetailID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "DefaultPricePointWebID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "DefaultPricePointWholesaleID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "DefaultTaxJurisdictionID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "FreightID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "GeneralEmail",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "NumLicenses",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "Phone1",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "Phone2",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "Suburb",
                table: "CompanyLocation");
        }
    }
}

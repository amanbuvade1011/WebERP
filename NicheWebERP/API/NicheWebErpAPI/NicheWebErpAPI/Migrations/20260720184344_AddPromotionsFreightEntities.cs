using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    // All 5 tables EF generated CreateTable operations for here (CouponPerson, FreightCalculator,
    // FreightItem, Promotion, TransactionDiscount) already exist physically in this database -
    // confirmed live via INFORMATION_SCHEMA.TABLES on 2026-07-21, same situation as Sprint 06's
    // migration. Emptied both Up() and Down() accordingly - this migration only exists to record
    // that the EF model changed, not to change the database.
    /// <inheritdoc />
    public partial class AddPromotionsFreightEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

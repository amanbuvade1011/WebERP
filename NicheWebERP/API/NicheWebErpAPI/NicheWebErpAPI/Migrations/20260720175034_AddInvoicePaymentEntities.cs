using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    // All 5 operations EF generated here (TransactionBase.PaymentDueDate/PaymentMethodID/
    // SalesOrderID, and the PaymentMethod/FinancialAllocation tables) already exist physically in
    // this database - confirmed live via INFORMATION_SCHEMA.COLUMNS and direct row-count queries
    // on 2026-07-20 (PaymentMethod has 4 real rows, FinancialAllocation has 0). This is the same
    // situation as every other legacy table this project maps onto: the physical schema already
    // has them, EF's migration history just needed to catch up to a C# model that now also knows
    // about them. Running the original generated Up() failed with "Column names in each table
    // must be unique" / would have failed with "table already exists" on the CreateTable calls.
    // Emptied both Up() and Down() accordingly - this migration only exists to record that the EF
    // model changed, not to change the database.
    /// <inheritdoc />
    public partial class AddInvoicePaymentEntities : Migration
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

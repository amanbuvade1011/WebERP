using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NicheWebErpAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddErpUserAndErpRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErpRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErpRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErpUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true),
                    LegacyPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErpUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErpUser_ErpRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ErpRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ErpUser_ErpUser_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "ErpUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErpRole_Name",
                table: "ErpRole",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErpUser_Email",
                table: "ErpUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErpUser_LegacyPersonId",
                table: "ErpUser",
                column: "LegacyPersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErpUser_RoleId",
                table: "ErpUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ErpUser_UpdatedByUserId",
                table: "ErpUser",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ErpUser_Username",
                table: "ErpUser",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErpUser");

            migrationBuilder.DropTable(
                name: "ErpRole");
        }
    }
}

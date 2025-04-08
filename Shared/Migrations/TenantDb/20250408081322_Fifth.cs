using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class Fifth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "T_CODE",
                table: "Tenant",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "A_EMAIL",
                table: "ACCOUNT",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_T_CODE",
                table: "Tenant",
                column: "T_CODE",
                unique: true,
                filter: "[T_CODE] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ACCOUNT_A_EMAIL",
                table: "ACCOUNT",
                column: "A_EMAIL",
                unique: true,
                filter: "[A_EMAIL] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenant_T_CODE",
                table: "Tenant");

            migrationBuilder.DropIndex(
                name: "IX_ACCOUNT_A_EMAIL",
                table: "ACCOUNT");

            migrationBuilder.AlterColumn<string>(
                name: "T_CODE",
                table: "Tenant",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "A_EMAIL",
                table: "ACCOUNT",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}

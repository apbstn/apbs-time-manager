using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenant_T_CODE",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "T_CODE",
                table: "Tenant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "T_CODE",
                table: "Tenant",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_T_CODE",
                table: "Tenant",
                column: "T_CODE",
                unique: true);
        }
    }
}

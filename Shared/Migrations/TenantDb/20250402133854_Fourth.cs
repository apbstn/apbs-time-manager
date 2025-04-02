using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class Fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenant_ACCOUNT_OwnerId",
                table: "Tenant");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Tenant",
                newName: "OWNER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Tenant_OwnerId",
                table: "Tenant",
                newName: "IX_Tenant_OWNER_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenant_ACCOUNT_OWNER_ID",
                table: "Tenant",
                column: "OWNER_ID",
                principalTable: "ACCOUNT",
                principalColumn: "A_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenant_ACCOUNT_OWNER_ID",
                table: "Tenant");

            migrationBuilder.RenameColumn(
                name: "OWNER_ID",
                table: "Tenant",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tenant_OWNER_ID",
                table: "Tenant",
                newName: "IX_Tenant_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenant_ACCOUNT_OwnerId",
                table: "Tenant",
                column: "OwnerId",
                principalTable: "ACCOUNT",
                principalColumn: "A_ID");
        }
    }
}

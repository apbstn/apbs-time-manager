using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "A_TENANT_ROLE",
                table: "ACCOUNT",
                newName: "A_ROLE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "A_ROLE",
                table: "ACCOUNT",
                newName: "A_TENANT_ROLE");
        }
    }
}

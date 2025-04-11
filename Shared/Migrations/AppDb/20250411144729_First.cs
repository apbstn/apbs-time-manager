using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACCOUNT",
                columns: table => new
                {
                    A_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    A_EMAIL = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    A_USERNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_TENANT_ROLE = table.Column<int>(type: "int", nullable: false),
                    A_PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACCOUNT", x => x.A_ID);
                });

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
            migrationBuilder.DropTable(
                name: "ACCOUNT");
        }
    }
}

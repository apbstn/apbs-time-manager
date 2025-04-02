using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACCOUNT",
                columns: table => new
                {
                    A_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    A_EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_USERNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_PASSWORD_HASH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_TENANT_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_ROLE = table.Column<int>(type: "int", nullable: false),
                    A_SEED = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
                    A_REGISTRED = table.Column<bool>(type: "bit", nullable: false),
                    A_PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACCOUNT", x => x.A_ID);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    T_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    T_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T_CONNECTION_STRING = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.T_ID);
                    table.ForeignKey(
                        name: "FK_Tenant_ACCOUNT_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "ACCOUNT",
                        principalColumn: "A_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_OwnerId",
                table: "Tenant",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "ACCOUNT");
        }
    }
}

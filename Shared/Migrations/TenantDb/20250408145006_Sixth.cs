using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class Sixth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "A_ROLE",
                table: "ACCOUNT");

            migrationBuilder.DropColumn(
                name: "A_TENANT_ID",
                table: "ACCOUNT");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "ACCOUNT",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "JOIN_TENANT_USER",
                columns: table => new
                {
                    J_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    J_USER_TENANT_ROLE = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOIN_TENANT_USER", x => x.J_ID);
                    table.ForeignKey(
                        name: "FK_JOIN_TENANT_USER_ACCOUNT_userId",
                        column: x => x.userId,
                        principalTable: "ACCOUNT",
                        principalColumn: "A_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JOIN_TENANT_USER_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "T_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JOIN_TENANT_USER_TenantId",
                table: "JOIN_TENANT_USER",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_JOIN_TENANT_USER_userId",
                table: "JOIN_TENANT_USER",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOIN_TENANT_USER");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "ACCOUNT");

            migrationBuilder.AddColumn<int>(
                name: "A_ROLE",
                table: "ACCOUNT",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "A_TENANT_ID",
                table: "ACCOUNT",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

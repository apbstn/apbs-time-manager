using System;
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
            migrationBuilder.CreateTable(
                name: "ACCOUNT",
                columns: table => new
                {
                    A_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    A_EMAIL = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    A_USERNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_PASSWORD_HASH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_SEED = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
                    A_REGISTRED = table.Column<bool>(type: "bit", nullable: false),
                    A_PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACCOUNT", x => x.A_ID);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    T_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    T_CODE = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    T_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T_CONNECTION_STRING = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    J_USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.T_ID);
                    table.ForeignKey(
                        name: "FK_Tenant_ACCOUNT_J_USER_ID",
                        column: x => x.J_USER_ID,
                        principalTable: "ACCOUNT",
                        principalColumn: "A_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitation",
                columns: table => new
                {
                    I_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    I_USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    I_EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    I_PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    I_TENANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    I_TOKEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    I_CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    I_EXPIRES_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    I_IS_USED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitation", x => x.I_ID);
                    table.ForeignKey(
                        name: "FK_Invitation_Tenant_I_TENANT_ID",
                        column: x => x.I_TENANT_ID,
                        principalTable: "Tenant",
                        principalColumn: "T_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JOIN_TENANT_USER",
                columns: table => new
                {
                    J_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    J_USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    J_TENANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    J_USER_TENANT_ROLE = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOIN_TENANT_USER", x => x.J_ID);
                    table.ForeignKey(
                        name: "FK_JOIN_TENANT_USER_ACCOUNT_J_USER_ID",
                        column: x => x.J_USER_ID,
                        principalTable: "ACCOUNT",
                        principalColumn: "A_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JOIN_TENANT_USER_Tenant_J_TENANT_ID",
                        column: x => x.J_TENANT_ID,
                        principalTable: "Tenant",
                        principalColumn: "T_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ACCOUNT_A_EMAIL",
                table: "ACCOUNT",
                column: "A_EMAIL",
                unique: true,
                filter: "[A_EMAIL] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_I_TENANT_ID",
                table: "Invitation",
                column: "I_TENANT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOIN_TENANT_USER_J_TENANT_ID",
                table: "JOIN_TENANT_USER",
                column: "J_TENANT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOIN_TENANT_USER_J_USER_ID",
                table: "JOIN_TENANT_USER",
                column: "J_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_J_USER_ID",
                table: "Tenant",
                column: "J_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_T_CODE",
                table: "Tenant",
                column: "T_CODE",
                unique: true,
                filter: "[T_CODE] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invitation");

            migrationBuilder.DropTable(
                name: "JOIN_TENANT_USER");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "ACCOUNT");
        }
    }
}

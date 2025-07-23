using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class eleven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LEAVEBALANCE",
                columns: table => new
                {
                    LB_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    LB_USER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    LB_BALANCE = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    LB_LAST_UPDATED = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LB_LAST_ALLOCATION_MONTH = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVEBALANCE", x => x.LB_ID);
                    table.ForeignKey(
                        name: "FK_LEAVEBALANCE_T_ACCOUNT_LB_USER_ID",
                        column: x => x.LB_USER_ID,
                        principalTable: "T_ACCOUNT",
                        principalColumn: "A_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LEAVEBALANCE_LB_USER_ID",
                table: "LEAVEBALANCE",
                column: "LB_USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LEAVEBALANCE");
        }
    }
}

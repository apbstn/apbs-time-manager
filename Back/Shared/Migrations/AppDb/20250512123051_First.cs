using System;
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
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeaveBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_ACCOUNT",
                columns: table => new
                {
                    A_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    A_EMAIL = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    A_USERNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    A_ROLE = table.Column<int>(type: "int", nullable: true),
                    A_PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ACCOUNT", x => x.A_ID);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Activ = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    TotalHours = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LEAVEREQUEST",
                columns: table => new
                {
                    L_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    L_USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    L_START = table.Column<DateTime>(type: "datetime2", nullable: false),
                    L_END = table.Column<DateTime>(type: "datetime2", nullable: false),
                    L_NUMBEROFDAYS = table.Column<int>(type: "int", nullable: false),
                    L_STATUS = table.Column<int>(type: "int", nullable: false),
                    L_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    L_REASON = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVEREQUEST", x => x.L_ID);
                    table.ForeignKey(
                        name: "FK_LEAVEREQUEST_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LEAVEREQUEST_T_ACCOUNT_L_USER_ID",
                        column: x => x.L_USER_ID,
                        principalTable: "T_ACCOUNT",
                        principalColumn: "A_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LEAVEREQUEST_EmployeeId",
                table: "LEAVEREQUEST",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVEREQUEST_L_USER_ID",
                table: "LEAVEREQUEST",
                column: "L_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_ACCOUNT_A_EMAIL",
                table: "T_ACCOUNT",
                column: "A_EMAIL",
                unique: true,
                filter: "[A_EMAIL] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LEAVEREQUEST");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "TimeLogs");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "T_ACCOUNT");
        }
    }
}

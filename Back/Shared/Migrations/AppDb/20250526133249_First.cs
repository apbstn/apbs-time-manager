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
                name: "Planners",
                columns: table => new
                {
                    P_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_Name = table.Column<string>(type: "text", nullable: false),
                    P_TYPE = table.Column<int>(type: "integer", nullable: false),
                    Monday = table.Column<bool>(type: "boolean", nullable: false),
                    Tuesday = table.Column<bool>(type: "boolean", nullable: false),
                    Wendsday = table.Column<bool>(type: "boolean", nullable: false),
                    Thursday = table.Column<bool>(type: "boolean", nullable: false),
                    Friday = table.Column<bool>(type: "boolean", nullable: false),
                    Saturday = table.Column<bool>(type: "boolean", nullable: false),
                    Sunday = table.Column<bool>(type: "boolean", nullable: false),
                    TotalWeeklyHours = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planners", x => x.P_ID);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DayPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NumberOfHours = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayPlan_Planners_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Planners",
                        principalColumn: "P_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_ACCOUNT",
                columns: table => new
                {
                    A_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    A_EMAIL = table.Column<string>(type: "text", nullable: true),
                    A_USERNAME = table.Column<string>(type: "text", nullable: true),
                    A_ROLE = table.Column<int>(type: "integer", nullable: true),
                    A_PHONE_NUMBER = table.Column<string>(type: "text", nullable: true),
                    A_TEAM = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ACCOUNT", x => x.A_ID);
                    table.ForeignKey(
                        name: "FK_T_ACCOUNT_Teams_A_TEAM",
                        column: x => x.A_TEAM,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LEAVEREQUEST",
                columns: table => new
                {
                    L_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    L_USER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    L_START = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    L_END = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    L_NUMBEROFDAYS = table.Column<int>(type: "integer", nullable: false),
                    L_STATUS = table.Column<int>(type: "integer", nullable: false),
                    L_TYPE = table.Column<string>(type: "text", nullable: false),
                    L_REASON = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVEREQUEST", x => x.L_ID);
                    table.ForeignKey(
                        name: "FK_LEAVEREQUEST_T_ACCOUNT_L_USER_ID",
                        column: x => x.L_USER_ID,
                        principalTable: "T_ACCOUNT",
                        principalColumn: "A_ID");
                });

            migrationBuilder.CreateTable(
                name: "TIMELOG",
                columns: table => new
                {
                    TM_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    TM_USER_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    TM_TIME = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TM_TYPE = table.Column<int>(type: "integer", nullable: false),
                    TM_ACTIV = table.Column<bool>(type: "boolean", nullable: false),
                    TM_TOTALHOURS = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIMELOG", x => x.TM_ID);
                    table.ForeignKey(
                        name: "FK_TIMELOG_T_ACCOUNT_TM_USER_ID",
                        column: x => x.TM_USER_ID,
                        principalTable: "T_ACCOUNT",
                        principalColumn: "A_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayPlan_PlanId",
                table: "DayPlan",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVEREQUEST_L_USER_ID",
                table: "LEAVEREQUEST",
                column: "L_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_ACCOUNT_A_EMAIL",
                table: "T_ACCOUNT",
                column: "A_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_ACCOUNT_A_TEAM",
                table: "T_ACCOUNT",
                column: "A_TEAM");

            migrationBuilder.CreateIndex(
                name: "IX_TIMELOG_TM_USER_ID",
                table: "TIMELOG",
                column: "TM_USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayPlan");

            migrationBuilder.DropTable(
                name: "LEAVEREQUEST");

            migrationBuilder.DropTable(
                name: "TIMELOG");

            migrationBuilder.DropTable(
                name: "Planners");

            migrationBuilder.DropTable(
                name: "T_ACCOUNT");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}

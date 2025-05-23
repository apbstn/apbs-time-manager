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
                name: "fixedDayPlanners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fixedDayPlanners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "flexibleDayPlanners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    NumberOfHours = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flexibleDayPlanners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Planners",
                columns: table => new
                {
                    P_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_TYPE = table.Column<int>(type: "integer", nullable: false)
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
                name: "FixedPlanners",
                columns: table => new
                {
                    P_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_MONDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_TUESDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_WENDSDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_THURSDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_FRIDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_SATURDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_SUNDAY_ID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedPlanners", x => x.P_ID);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_Planners_P_ID",
                        column: x => x.P_ID,
                        principalTable: "Planners",
                        principalColumn: "P_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_fixedDayPlanners_P_FRIDAY_ID",
                        column: x => x.P_FRIDAY_ID,
                        principalTable: "fixedDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_fixedDayPlanners_P_MONDAY_ID",
                        column: x => x.P_MONDAY_ID,
                        principalTable: "fixedDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_fixedDayPlanners_P_SATURDAY_ID",
                        column: x => x.P_SATURDAY_ID,
                        principalTable: "fixedDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_fixedDayPlanners_P_SUNDAY_ID",
                        column: x => x.P_SUNDAY_ID,
                        principalTable: "fixedDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_fixedDayPlanners_P_THURSDAY_ID",
                        column: x => x.P_THURSDAY_ID,
                        principalTable: "fixedDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_fixedDayPlanners_P_TUESDAY_ID",
                        column: x => x.P_TUESDAY_ID,
                        principalTable: "fixedDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPlanners_fixedDayPlanners_P_WENDSDAY_ID",
                        column: x => x.P_WENDSDAY_ID,
                        principalTable: "fixedDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlexiblePlanners",
                columns: table => new
                {
                    P_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_MONDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_TUESDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_WENDSDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_THURSDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_FRIDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_SATURDAY_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_SUNDAY_ID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlexiblePlanners", x => x.P_ID);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_Planners_P_ID",
                        column: x => x.P_ID,
                        principalTable: "Planners",
                        principalColumn: "P_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_flexibleDayPlanners_P_FRIDAY_ID",
                        column: x => x.P_FRIDAY_ID,
                        principalTable: "flexibleDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_flexibleDayPlanners_P_MONDAY_ID",
                        column: x => x.P_MONDAY_ID,
                        principalTable: "flexibleDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_flexibleDayPlanners_P_SATURDAY_ID",
                        column: x => x.P_SATURDAY_ID,
                        principalTable: "flexibleDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_flexibleDayPlanners_P_SUNDAY_ID",
                        column: x => x.P_SUNDAY_ID,
                        principalTable: "flexibleDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_flexibleDayPlanners_P_THURSDAY_ID",
                        column: x => x.P_THURSDAY_ID,
                        principalTable: "flexibleDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_flexibleDayPlanners_P_TUESDAY_ID",
                        column: x => x.P_TUESDAY_ID,
                        principalTable: "flexibleDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlexiblePlanners_flexibleDayPlanners_P_WENDSDAY_ID",
                        column: x => x.P_WENDSDAY_ID,
                        principalTable: "flexibleDayPlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyPlanners",
                columns: table => new
                {
                    P_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    P_WEEKLY_HOURS = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyPlanners", x => x.P_ID);
                    table.ForeignKey(
                        name: "FK_WeeklyPlanners_Planners_P_ID",
                        column: x => x.P_ID,
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
                    TM_TOTALHOURS = table.Column<TimeSpan>(type: "interval", nullable: false)
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
                name: "IX_FixedPlanners_P_FRIDAY_ID",
                table: "FixedPlanners",
                column: "P_FRIDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPlanners_P_MONDAY_ID",
                table: "FixedPlanners",
                column: "P_MONDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPlanners_P_SATURDAY_ID",
                table: "FixedPlanners",
                column: "P_SATURDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPlanners_P_SUNDAY_ID",
                table: "FixedPlanners",
                column: "P_SUNDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPlanners_P_THURSDAY_ID",
                table: "FixedPlanners",
                column: "P_THURSDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPlanners_P_TUESDAY_ID",
                table: "FixedPlanners",
                column: "P_TUESDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPlanners_P_WENDSDAY_ID",
                table: "FixedPlanners",
                column: "P_WENDSDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FlexiblePlanners_P_FRIDAY_ID",
                table: "FlexiblePlanners",
                column: "P_FRIDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FlexiblePlanners_P_MONDAY_ID",
                table: "FlexiblePlanners",
                column: "P_MONDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FlexiblePlanners_P_SATURDAY_ID",
                table: "FlexiblePlanners",
                column: "P_SATURDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FlexiblePlanners_P_SUNDAY_ID",
                table: "FlexiblePlanners",
                column: "P_SUNDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FlexiblePlanners_P_THURSDAY_ID",
                table: "FlexiblePlanners",
                column: "P_THURSDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FlexiblePlanners_P_TUESDAY_ID",
                table: "FlexiblePlanners",
                column: "P_TUESDAY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FlexiblePlanners_P_WENDSDAY_ID",
                table: "FlexiblePlanners",
                column: "P_WENDSDAY_ID");

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
                name: "FixedPlanners");

            migrationBuilder.DropTable(
                name: "FlexiblePlanners");

            migrationBuilder.DropTable(
                name: "LEAVEREQUEST");

            migrationBuilder.DropTable(
                name: "TIMELOG");

            migrationBuilder.DropTable(
                name: "WeeklyPlanners");

            migrationBuilder.DropTable(
                name: "fixedDayPlanners");

            migrationBuilder.DropTable(
                name: "flexibleDayPlanners");

            migrationBuilder.DropTable(
                name: "T_ACCOUNT");

            migrationBuilder.DropTable(
                name: "Planners");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}

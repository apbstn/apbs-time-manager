using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class Seventeen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayPlan");

            migrationBuilder.DropTable(
                name: "Planners");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Planners",
                columns: table => new
                {
                    P_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Friday = table.Column<bool>(type: "boolean", nullable: false),
                    Monday = table.Column<bool>(type: "boolean", nullable: false),
                    P_TYPE = table.Column<int>(type: "integer", nullable: false),
                    P_Name = table.Column<string>(type: "text", nullable: false),
                    Saturday = table.Column<bool>(type: "boolean", nullable: false),
                    Sunday = table.Column<bool>(type: "boolean", nullable: false),
                    Thursday = table.Column<bool>(type: "boolean", nullable: false),
                    TotalWeeklyHours = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Tuesday = table.Column<bool>(type: "boolean", nullable: false),
                    Wednesday = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planners", x => x.P_ID);
                });

            migrationBuilder.CreateTable(
                name: "DayPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NumberOfHours = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_DayPlan_PlanId",
                table: "DayPlan",
                column: "PlanId");
        }
    }
}

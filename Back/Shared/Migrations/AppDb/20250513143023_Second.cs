using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeLogs",
                table: "TimeLogs");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "TimeLogs");

            migrationBuilder.RenameTable(
                name: "TimeLogs",
                newName: "TIMELOG");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "TIMELOG",
                newName: "TM_TYPE");

            migrationBuilder.RenameColumn(
                name: "TotalHours",
                table: "TIMELOG",
                newName: "TM_TOTALHOURS");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "TIMELOG",
                newName: "TM_TIME");

            migrationBuilder.RenameColumn(
                name: "Activ",
                table: "TIMELOG",
                newName: "TM_ACTIV");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TIMELOG",
                newName: "TM_USER_ID");

            migrationBuilder.AddColumn<Guid>(
                name: "A_TEAM",
                table: "T_ACCOUNT",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TM_TOTALHOURS",
                table: "TIMELOG",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TM_ID",
                table: "TIMELOG",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TIMELOG",
                table: "TIMELOG",
                column: "TM_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_ACCOUNT_A_TEAM",
                table: "T_ACCOUNT",
                column: "A_TEAM");

            migrationBuilder.CreateIndex(
                name: "IX_TIMELOG_TM_USER_ID",
                table: "TIMELOG",
                column: "TM_USER_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_T_ACCOUNT_Teams_A_TEAM",
                table: "T_ACCOUNT",
                column: "A_TEAM",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TIMELOG_T_ACCOUNT_TM_USER_ID",
                table: "TIMELOG",
                column: "TM_USER_ID",
                principalTable: "T_ACCOUNT",
                principalColumn: "A_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_ACCOUNT_Teams_A_TEAM",
                table: "T_ACCOUNT");

            migrationBuilder.DropForeignKey(
                name: "FK_TIMELOG_T_ACCOUNT_TM_USER_ID",
                table: "TIMELOG");

            migrationBuilder.DropIndex(
                name: "IX_T_ACCOUNT_A_TEAM",
                table: "T_ACCOUNT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TIMELOG",
                table: "TIMELOG");

            migrationBuilder.DropIndex(
                name: "IX_TIMELOG_TM_USER_ID",
                table: "TIMELOG");

            migrationBuilder.DropColumn(
                name: "A_TEAM",
                table: "T_ACCOUNT");

            migrationBuilder.DropColumn(
                name: "TM_ID",
                table: "TIMELOG");

            migrationBuilder.RenameTable(
                name: "TIMELOG",
                newName: "TimeLogs");

            migrationBuilder.RenameColumn(
                name: "TM_TYPE",
                table: "TimeLogs",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "TM_TOTALHOURS",
                table: "TimeLogs",
                newName: "TotalHours");

            migrationBuilder.RenameColumn(
                name: "TM_TIME",
                table: "TimeLogs",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "TM_ACTIV",
                table: "TimeLogs",
                newName: "Activ");

            migrationBuilder.RenameColumn(
                name: "TM_USER_ID",
                table: "TimeLogs",
                newName: "Id");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalHours",
                table: "TimeLogs",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "TimeLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeLogs",
                table: "TimeLogs",
                column: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangLang.Migrations
{
    public partial class AddScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduleDate",
                table: "ScheduleItems",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Date);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleItems_ScheduleDate",
                table: "ScheduleItems",
                column: "ScheduleDate");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleItems_Schedules_ScheduleDate",
                table: "ScheduleItems",
                column: "ScheduleDate",
                principalTable: "Schedules",
                principalColumn: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleItems_Schedules_ScheduleDate",
                table: "ScheduleItems");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleItems_ScheduleDate",
                table: "ScheduleItems");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "ScheduleItems");
        }
    }
}

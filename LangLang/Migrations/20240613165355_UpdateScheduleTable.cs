using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangLang.Migrations
{
    public partial class UpdateScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleItems_Schedules_ScheduleDate",
                table: "ScheduleItems");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleItems_ScheduleDate",
                table: "ScheduleItems");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "ScheduleItems");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleItems",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleItems",
                table: "Schedules");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduleDate",
                table: "ScheduleItems",
                type: "date",
                nullable: true);

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
    }
}

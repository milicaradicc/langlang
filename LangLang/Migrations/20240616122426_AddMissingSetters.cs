using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangLang.Migrations
{
    public partial class AddMissingSetters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "CourseIds",
                table: "Users",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateCreated",
                table: "Users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "ExamIds",
                table: "Users",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropOutRequests",
                table: "ScheduleItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Students",
                table: "ScheduleItems",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Dictionary<int, ApplicationStatus>",
                columns: table => new
                {
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Dictionary<int, string>",
                columns: table => new
                {
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dictionary<int, ApplicationStatus>");

            migrationBuilder.DropTable(
                name: "Dictionary<int, string>");

            migrationBuilder.DropColumn(
                name: "CourseIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExamIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DropOutRequests",
                table: "ScheduleItems");

            migrationBuilder.DropColumn(
                name: "Students",
                table: "ScheduleItems");
        }
    }
}

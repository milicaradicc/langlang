using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LangLang.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dictionary<int, bool>",
                columns: table => new
                {
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Dictionary<int, int>",
                columns: table => new
                {
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "List<Weekday>",
                columns: table => new
                {
                    Capacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Education = table.Column<int>(type: "integer", nullable: true),
                    PenaltyPoints = table.Column<int>(type: "integer", nullable: true),
                    ActiveCourseId = table.Column<int>(type: "integer", nullable: true),
                    LanguagePassFail = table.Column<string>(type: "text", nullable: true),
                    AppliedExams = table.Column<List<int>>(type: "integer[]", nullable: true),
                    ExamGradeIds = table.Column<string>(type: "text", nullable: true),
                    CourseGradeIds = table.Column<string>(type: "text", nullable: true),
                    TotalRating = table.Column<int>(type: "integer", nullable: true),
                    NumberOfReviews = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    MaxStudents = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    Held = table.Column<List<int>>(type: "integer[]", nullable: true),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: true),
                    IsFinished = table.Column<bool>(type: "boolean", nullable: true),
                    StudentsNotified = table.Column<bool>(type: "boolean", nullable: true),
                    CreatorId = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AreApplicationsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    TeacherGraded = table.Column<bool>(type: "boolean", nullable: true),
                    DirectorGraded = table.Column<bool>(type: "boolean", nullable: true),
                    StudentIds = table.Column<List<int>>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleItems_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageTeacher",
                columns: table => new
                {
                    QualificationsId = table.Column<int>(type: "integer", nullable: false),
                    TeachersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageTeacher", x => new { x.QualificationsId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_LanguageTeacher_Languages_QualificationsId",
                        column: x => x.QualificationsId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageTeacher_Users_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageTeacher_TeachersId",
                table: "LanguageTeacher",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleItems_LanguageId",
                table: "ScheduleItems",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dictionary<int, bool>");

            migrationBuilder.DropTable(
                name: "Dictionary<int, int>");

            migrationBuilder.DropTable(
                name: "LanguageTeacher");

            migrationBuilder.DropTable(
                name: "List<Weekday>");

            migrationBuilder.DropTable(
                name: "ScheduleItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}

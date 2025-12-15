using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTracker.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    BadgeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.BadgeID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ExperiencePoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    HabitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => x.HabitID);
                    table.ForeignKey(
                        name: "FK_Habits_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBadges",
                columns: table => new
                {
                    UserBadgeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateEarned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    BadgeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBadges", x => x.UserBadgeID);
                    table.ForeignKey(
                        name: "FK_UserBadges_Badges_BadgeID",
                        column: x => x.BadgeID,
                        principalTable: "Badges",
                        principalColumn: "BadgeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBadges_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabitCompletions",
                columns: table => new
                {
                    CompletionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HabitID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitCompletions", x => x.CompletionID);
                    table.ForeignKey(
                        name: "FK_HabitCompletions_Habits_HabitID",
                        column: x => x.HabitID,
                        principalTable: "Habits",
                        principalColumn: "HabitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    ReminderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReminderTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HabitID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.ReminderID);
                    table.ForeignKey(
                        name: "FK_Reminders_Habits_HabitID",
                        column: x => x.HabitID,
                        principalTable: "Habits",
                        principalColumn: "HabitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HabitCompletions_HabitID",
                table: "HabitCompletions",
                column: "HabitID");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserID",
                table: "Habits",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_HabitID",
                table: "Reminders",
                column: "HabitID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeID",
                table: "UserBadges",
                column: "BadgeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_UserID",
                table: "UserBadges",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HabitCompletions");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "UserBadges");

            migrationBuilder.DropTable(
                name: "Habits");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

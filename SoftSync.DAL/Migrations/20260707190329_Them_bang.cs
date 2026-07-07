using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftSync.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Them_bang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemedial",
                table: "RoadmapItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MiniGameId",
                table: "RoadmapItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TheoryLessonId",
                table: "RoadmapItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MentorConversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MentorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorConversations_Mentors_MentorId",
                        column: x => x.MentorId,
                        principalTable: "Mentors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentorConversations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TheoryLessonProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TheoryLessonId = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheoryLessonProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TheoryLessonProgresses_TheoryLessons_TheoryLessonId",
                        column: x => x.TheoryLessonId,
                        principalTable: "TheoryLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TheoryLessonProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentorMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    SenderType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorMessages_MentorConversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "MentorConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 3, 28, 615, DateTimeKind.Utc).AddTicks(7330), "100000.4Fke9u64NJ+Xjf9PoEk99w==.OfZK+vZ2w0l9MUFyDhm/85YEby9X5kTdMb2kTo+BYiA=" });

            migrationBuilder.CreateIndex(
                name: "IX_MentorConversations_MentorId",
                table: "MentorConversations",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorConversations_UserId_MentorId",
                table: "MentorConversations",
                columns: new[] { "UserId", "MentorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MentorMessages_ConversationId",
                table: "MentorMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_TheoryLessonProgresses_TheoryLessonId",
                table: "TheoryLessonProgresses",
                column: "TheoryLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_TheoryLessonProgresses_UserId_TheoryLessonId",
                table: "TheoryLessonProgresses",
                columns: new[] { "UserId", "TheoryLessonId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MentorMessages");

            migrationBuilder.DropTable(
                name: "TheoryLessonProgresses");

            migrationBuilder.DropTable(
                name: "MentorConversations");

            migrationBuilder.DropColumn(
                name: "IsRemedial",
                table: "RoadmapItems");

            migrationBuilder.DropColumn(
                name: "MiniGameId",
                table: "RoadmapItems");

            migrationBuilder.DropColumn(
                name: "TheoryLessonId",
                table: "RoadmapItems");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 16, 19, 37, 619, DateTimeKind.Utc).AddTicks(1343), "100000.dWcqqpmE7DUu0BDsGm6a/Q==.ulFl+DO1KnWnHf34E3bKOZrUc0WH20FhnAcu+Mqzqeg=" });
        }
    }
}

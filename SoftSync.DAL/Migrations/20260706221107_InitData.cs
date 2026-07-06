using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoftSync.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mentors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Expertise = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortBio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentQuestions_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseStudies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Scenario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseStudies_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryTestQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryTestQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryTestQuestions_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    QuestionsPerRound = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGames_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TheoryLessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContentMarkdown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheoryLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TheoryLessons_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentResults_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentResults_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Sender = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryTestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    OverallLevel = table.Column<int>(type: "int", nullable: false),
                    TimeManagementScore = table.Column<int>(type: "int", nullable: false),
                    TimeManagementLevel = table.Column<int>(type: "int", nullable: false),
                    CommunicationScore = table.Column<int>(type: "int", nullable: false),
                    CommunicationLevel = table.Column<int>(type: "int", nullable: false),
                    CriticalThinkingScore = table.Column<int>(type: "int", nullable: false),
                    CriticalThinkingLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryTestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryTestResults_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    PercentComplete = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressLogs_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgressLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSkillSelections",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkillSelections", x => new { x.UserId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_UserSkillSelections_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSkillSelections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScoreValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentOptions_AssessmentQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "AssessmentQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseStudyOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseStudyId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRecommended = table.Column<bool>(type: "bit", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseStudyOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseStudyOptions_CaseStudies_CaseStudyId",
                        column: x => x.CaseStudyId,
                        principalTable: "CaseStudies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryTestOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryTestQuestionId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryTestOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryTestOptions_EntryTestQuestions_EntryTestQuestionId",
                        column: x => x.EntryTestQuestionId,
                        principalTable: "EntryTestQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniGameAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MiniGameId = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnswersJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameAttempts_MiniGames_MiniGameId",
                        column: x => x.MiniGameId,
                        principalTable: "MiniGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiniGameAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniGameQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MiniGameId = table.Column<int>(type: "int", nullable: false),
                    ScenarioText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContextNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameQuestions_MiniGames_MiniGameId",
                        column: x => x.MiniGameId,
                        principalTable: "MiniGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniGameOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MiniGameQuestionId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameOptions_MiniGameQuestions_MiniGameQuestionId",
                        column: x => x.MiniGameQuestionId,
                        principalTable: "MiniGameQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Mentors",
                columns: new[] { "Id", "AvatarUrl", "Expertise", "Name", "ShortBio" },
                values: new object[,]
                {
                    { 1, "https://i.pravatar.cc/150?u=katherine", "Leadership & Soft Skills", "Dr. Katherine", "15 years of experience in corporate training." },
                    { 2, "https://i.pravatar.cc/150?u=john", "Problem Solving", "John Smith", "Former Google engineer, expert in algorithms and thinking." }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Description", "IconName", "Name" },
                values: new object[,]
                {
                    { 1, "Effective verbal and non-verbal interaction.", "bi-chat-dots", "Communication" },
                    { 2, "Collaborating effectively with others.", "bi-people", "Teamwork" },
                    { 3, "Organizing and planning your time.", "bi-clock", "Time Management" },
                    { 4, "Analyzing info to make judgments.", "bi-lightbulb", "Critical Thinking" },
                    { 5, "Finding solutions to complex issues.", "bi-tools", "Problem Solving" },
                    { 6, "Recognizing and managing emotions.", "bi-heart", "Emotional Management" },
                    { 7, "Adjusting to new conditions.", "bi-arrow-repeat", "Adaptability" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Age", "CreatedAt", "Email", "FullName", "Goal", "PasswordHash", "Role" },
                values: new object[] { 1, 20, new DateTime(2026, 6, 26, 22, 11, 6, 719, DateTimeKind.Utc).AddTicks(5166), "demo@softsync.vn", "Nguyễn Văn A", "Cải thiện kỹ năng giao tiếp", "100000.ia/YzWCIvvslvI9U5YNlkg==.LdOq2bhddth5faKFocvX+9T/WOo7V+7fYTpN8CuAZnk=", 0 });

            migrationBuilder.InsertData(
                table: "CaseStudies",
                columns: new[] { "Id", "Scenario", "SkillId", "Title" },
                values: new object[,]
                {
                    { 1, "Your team member is not contributing. What do you do?", 1, "Group Communication" },
                    { 2, "You realize you will miss a deadline tomorrow. What is your first action?", 3, "Missed Deadline" }
                });

            migrationBuilder.InsertData(
                table: "CaseStudyOptions",
                columns: new[] { "Id", "CaseStudyId", "Feedback", "IsRecommended", "OptionText" },
                values: new object[,]
                {
                    { 1, 1, "This leads to burnout and doesn't solve the team dynamic issue.", false, "Do their work myself." },
                    { 2, 1, "Direct, empathetic communication is key.", true, "Talk to them privately to understand their situation." },
                    { 3, 2, "Risky and doesn't manage expectations.", false, "Work all night and hope for the best." },
                    { 4, 2, "Transparency and proactive planning are essential.", true, "Inform the stakeholders immediately and propose a new timeline." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentOptions_QuestionId",
                table: "AssessmentOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestions_SkillId",
                table: "AssessmentQuestions",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_SkillId",
                table: "AssessmentResults",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_UserId",
                table: "AssessmentResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStudies_SkillId",
                table: "CaseStudies",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStudyOptions_CaseStudyId",
                table: "CaseStudyOptions",
                column: "CaseStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryTestOptions_EntryTestQuestionId",
                table: "EntryTestOptions",
                column: "EntryTestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryTestQuestions_SkillId",
                table: "EntryTestQuestions",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryTestResults_UserId",
                table: "EntryTestResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameAttempts_MiniGameId",
                table: "MiniGameAttempts",
                column: "MiniGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameAttempts_UserId",
                table: "MiniGameAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameOptions_MiniGameQuestionId",
                table: "MiniGameOptions",
                column: "MiniGameQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameQuestions_MiniGameId",
                table: "MiniGameQuestions",
                column: "MiniGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGames_SkillId",
                table: "MiniGames",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressLogs_SkillId",
                table: "ProgressLogs",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressLogs_UserId",
                table: "ProgressLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapItems_UserId",
                table: "RoadmapItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TheoryLessons_SkillId",
                table: "TheoryLessons",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillSelections_SkillId",
                table: "UserSkillSelections",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentOptions");

            migrationBuilder.DropTable(
                name: "AssessmentResults");

            migrationBuilder.DropTable(
                name: "CaseStudyOptions");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "EntryTestOptions");

            migrationBuilder.DropTable(
                name: "EntryTestResults");

            migrationBuilder.DropTable(
                name: "Mentors");

            migrationBuilder.DropTable(
                name: "MiniGameAttempts");

            migrationBuilder.DropTable(
                name: "MiniGameOptions");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "ProgressLogs");

            migrationBuilder.DropTable(
                name: "RoadmapItems");

            migrationBuilder.DropTable(
                name: "TheoryLessons");

            migrationBuilder.DropTable(
                name: "UserSkillSelections");

            migrationBuilder.DropTable(
                name: "AssessmentQuestions");

            migrationBuilder.DropTable(
                name: "CaseStudies");

            migrationBuilder.DropTable(
                name: "EntryTestQuestions");

            migrationBuilder.DropTable(
                name: "MiniGameQuestions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MiniGames");

            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoftSync.DAL.Migrations
{
    /// <inheritdoc />
    public partial class uuuu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "MiniGameAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "CaseStudies",
                columns: new[] { "Id", "Scenario", "SkillId", "Title" },
                values: new object[] { 3, "Bạn phải đưa ra một quyết định quan trọng nhưng thiếu thông tin. Bạn sẽ làm gì?", 3, "Critical Decision" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 22, 24, 12, 454, DateTimeKind.Utc).AddTicks(2395), "100000.8ZJ1ZpcNDaKMT9G0c6wkAw==.iVl5eScoFsLK0op9YR+AtUIiVGa6iSoWfjpkssI/mWA=" });

            migrationBuilder.InsertData(
                table: "CaseStudyOptions",
                columns: new[] { "Id", "CaseStudyId", "Feedback", "IsRecommended", "OptionText" },
                values: new object[,]
                {
                    { 5, 3, "Điều này có thể dẫn đến mất cơ hội.", false, "Đợi thêm thông tin trước khi hành động." },
                    { 6, 3, "Đánh giá rủi ro và đưa ra quyết định dựa trên dữ liệu hiện có là cách tiếp cận thực tế.", true, "Thu thập thông tin có sẵn và đưa ra quyết định tốt nhất có thể." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaseStudyOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CaseStudyOptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CaseStudies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "MiniGameAttempts");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 3, 28, 615, DateTimeKind.Utc).AddTicks(7330), "100000.4Fke9u64NJ+Xjf9PoEk99w==.OfZK+vZ2w0l9MUFyDhm/85YEby9X5kTdMb2kTo+BYiA=" });
        }
    }
}

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Data;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Seed;

public static class JsonContentSeeder
{
    private const int SKILL_TIME = 1, SKILL_COMM = 2, SKILL_CRITICAL = 3;

    private static readonly Dictionary<string, int> SkillMap = new()
    {
        ["TIME_MANAGEMENT"] = SKILL_TIME,
        ["COMMUNICATION"] = SKILL_COMM,
        ["CRITICAL_THINKING"] = SKILL_CRITICAL
    };

    public static async Task SeedAsync(SoftSyncDbContext db, string dataFolder)
    {
        if (!await db.EntryTestQuestions.AnyAsync())
            await SeedEntryTestAsync(db, Path.Combine(dataFolder, "entry-test-questions.json"));

        if (!await db.MiniGames.AnyAsync())
            await SeedMiniGamesAsync(db, Path.Combine(dataFolder, "practical-scenarios.json"));

        if (!await db.TheoryLessons.AnyAsync())
            await SeedTheoryAsync(db, dataFolder);
    }

    private static async Task SeedEntryTestAsync(SoftSyncDbContext db, string path)
    {
        var json = await File.ReadAllTextAsync(path);
        var raw = JsonSerializer.Deserialize<List<RawEntryTestQuestion>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        int order = 1;
        foreach (var q in raw)
        {
            var question = new EntryTestQuestion
            {
                OrderIndex = order++,
                SkillId = SkillMap[q.SkillId],
                QuestionText = q.QuestionText,
                Options = q.Options.Select(o => new EntryTestOption
                {
                    OptionText = o.Text,
                    Level = o.Score
                }).ToList()
            };
            db.EntryTestQuestions.Add(question);
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedMiniGamesAsync(SoftSyncDbContext db, string path)
    {
        var json = await File.ReadAllTextAsync(path);
        var raw = JsonSerializer.Deserialize<List<RawScenario>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Tạm gom theo skillId thành 1 MiniGame mặc định/kỹ năng — sau này tách nhiều game thì sửa lại đây
        var gamesByGroup = new Dictionary<string, MiniGame>();
        foreach (var s in raw)
        {
            if (!gamesByGroup.TryGetValue(s.GameGroup, out var game))
            {
                game = new MiniGame
                {
                    SkillId = SkillMap[s.SkillId],
                    Name = s.GameName ?? s.GameGroup,
                    Type = Common.Enums.MiniGameType.ChooseBestAnswer,
                    Description = "Chọn phương án phù hợp nhất cho tình huống.",
                    QuestionsPerRound = 5
                };
                gamesByGroup[s.GameGroup] = game;
                db.MiniGames.Add(game);
            }

            // Bỏ qua các dạng chưa map option (FACT_OR_OPINION, IDENTIFY_FALLACY) ở bước đầu này
            if (s.Options == null || s.Options.Count == 0) continue;

            game.Questions.Add(new MiniGameQuestion
            {
                ScenarioText = s.Context ?? s.QuestionText ?? "",
                ContextNote = s.QuestionText,
                Options = s.Options.Select(o => new MiniGameOption
                {
                    OptionText = o.Text,
                    Points = o.Points,
                    Feedback = o.Feedback ?? ""
                }).ToList()
            });
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedTheoryAsync(SoftSyncDbContext db, string folder)
    {
        var files = new[]
        {
            ("time-theory.json", SKILL_TIME, "Lý thuyết Quản lý thời gian"),
            ("communication-theory.json", SKILL_COMM, "Lý thuyết Giao tiếp"),
            ("critical-theory.json", SKILL_CRITICAL, "Lý thuyết Tư duy phản biện")
        };

        foreach (var (file, skillId, title) in files)
        {
            var path = Path.Combine(folder, file);
            var json = await File.ReadAllTextAsync(path);
            var lesson = JsonSerializer.Deserialize<RawTheoryLesson>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            db.TheoryLessons.Add(new TheoryLesson
            {
                SkillId = skillId,
                Title = title,
                ContentMarkdown = lesson.LessonBody,
                VideoUrl = lesson.VideoUrl,
                OrderIndex = 1
            });
        }
        await db.SaveChangesAsync();
    }

    // DTO tạm để deserialize đúng shape JSON gốc
    private class RawEntryTestQuestion
    {
        public string SkillId { get; set; } = "";
        public string QuestionText { get; set; } = "";
        public List<RawOption> Options { get; set; } = new();
    }
    private class RawOption
    {
        public string Text { get; set; } = "";
        public int Score { get; set; }
    }
    private class RawScenario
    {
        public string SkillId { get; set; } = "";
        public string GameGroup { get; set; } = "";   // MỚI
        public string? GameName { get; set; }          // MỚI
        public string? Context { get; set; }
        public string? QuestionText { get; set; }
        public List<RawScenarioOption>? Options { get; set; }
    }
    private class RawScenarioOption
    {
        public string Text { get; set; } = "";
        public int Points { get; set; }
        public string? Feedback { get; set; }
    }
    
    private class RawTheoryLesson
    {
        public string SkillId { get; set; } = "";
        public string SkillName { get; set; } = "";
        public string? VideoUrl { get; set; }
        public string LessonBody { get; set; } = "";
    }
}
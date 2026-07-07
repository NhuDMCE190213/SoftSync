using SoftSync.BLL.Interfaces;
using SoftSync.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoftSync.BLL.Services
{
    // SoftSync.BLL/Services/AnswerAnalysisService.cs
    public class AnswerAnalysisService : IAnswerAnalysisService
    {
        private readonly IMiniGameAttemptRepository _attemptRepo;
        private readonly IMiniGameRepository _miniGameRepo;

        public AnswerAnalysisService(IMiniGameAttemptRepository attemptRepo, IMiniGameRepository miniGameRepo)
        {
            _attemptRepo = attemptRepo;
            _miniGameRepo = miniGameRepo;
        }

        public async Task<List<string>> GetRecentMistakePatternsAsync(int userId, int skillId, int take = 20)
        {
            var attempts = await _attemptRepo.GetRecentByUserAndSkillAsync(userId, skillId, take);
            if (!attempts.Any()) return new List<string>();

            // gom QuestionId -> OptionId đã chọn từ AnswersJson của mọi attempt
            var wrongQuestionIds = new List<int>();
            foreach (var attempt in attempts)
            {
                var answersMap = JsonSerializer.Deserialize<Dictionary<int, int>>(attempt.AnswersJson) ?? new();
                var options = await _miniGameRepo.GetOptionsByIdsAsync(answersMap.Values.ToList());
                var lowScoreOptionIds = options.Where(o => o.Points < 5).Select(o => o.Id).ToHashSet();

                wrongQuestionIds.AddRange(
                    answersMap.Where(kv => lowScoreOptionIds.Contains(kv.Value)).Select(kv => kv.Key));
            }

            // câu hỏi bị chọn sai lặp lại từ 2 lần trở lên -> coi là "pattern"
            var questionIds = wrongQuestionIds.GroupBy(id => id)
                .Where(g => g.Count() >= 2)
                .Select(g => g.Key)
                .ToList();
            if (!questionIds.Any()) return new List<string>();

            var questions = await _miniGameRepo.GetQuestionsByIdsAsync(questionIds); // cần thêm method này
            return questions.Select(q => q.ScenarioText).ToList();
        }
    }
}

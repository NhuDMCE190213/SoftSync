using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IAssessmentRepository
{
    Task<IEnumerable<AssessmentQuestion>> GetQuestionsBySkillIdsAsync(List<int> skillIds);
    Task SaveResultAsync(AssessmentResult result);
    Task<IEnumerable<AssessmentResult>> GetResultsByUserIdAsync(int userId);
}
public class AssessmentRepository : IAssessmentRepository
{
    private readonly Data.SoftSyncDbContext _context;
    public AssessmentRepository(Data.SoftSyncDbContext context) => _context = context;

    public async Task<IEnumerable<AssessmentQuestion>> GetQuestionsBySkillIdsAsync(List<int> skillIds)
    {
        return await _context.AssessmentQuestions
            .Where(q => skillIds.Contains(q.SkillId))
            .Include(q => q.Options)
            .Include(q => q.Skill)
            .ToListAsync();
    }

    public async Task SaveResultAsync(AssessmentResult result)
    {
        await _context.AssessmentResults.AddAsync(result);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AssessmentResult>> GetResultsByUserIdAsync(int userId)
    {
        return await _context.AssessmentResults
            .Where(r => r.UserId == userId)
            .Include(r => r.Skill)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}

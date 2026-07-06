using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Roadmap;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class RoadmapService : IRoadmapService
{
    private readonly IRoadmapRepository _roadmapRepo;
    private readonly IAiRoadmapService _aiService;
    public RoadmapService(IRoadmapRepository roadmapRepo, IAiRoadmapService aiService)
    {
        _roadmapRepo = roadmapRepo;
        _aiService = aiService;
    }

    public async Task<RoadmapDto> GetUserRoadmapAsync(int userId)
    {
        var items = await _roadmapRepo.GetByUserIdAsync(userId);
        if (!items.Any())
        {
            var fakeRoadmap = await _aiService.GenerateRoadmapAsync(userId, new List<string> { "Communication" });
            foreach (var item in fakeRoadmap.Items)
            {
                await _roadmapRepo.AddAsync(new RoadmapItem {
                    UserId = userId,
                    SkillId = item.SkillId,
                    WeekNumber = item.WeekNumber,
                    Title = item.Title,
                    Description = item.Description
                });
            }
            await _roadmapRepo.SaveChangesAsync();
            items = await _roadmapRepo.GetByUserIdAsync(userId);
        }

        return new RoadmapDto
        {
            UserId = userId,
            Items = items.Select(i => new RoadmapItemDto {
                Id = i.Id,
                SkillId = i.SkillId,
                WeekNumber = i.WeekNumber,
                Title = i.Title,
                Description = i.Description,
                IsCompleted = i.IsCompleted
            }).ToList()
        };
    }

    public async Task MarkCompleteAsync(int itemId)
    {
        var item = await _roadmapRepo.GetByIdAsync(itemId);
        if (item != null)
        {
            item.IsCompleted = true;
            await _roadmapRepo.SaveChangesAsync();
        }
    }
}

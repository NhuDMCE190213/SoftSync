# SoftSync — Gap Analysis & Checklist Hoàn Thiện Dự Án

> Đã đối chiếu trực tiếp với mã nguồn (`repomix-output.xml`, kiến trúc 4 layer: DAL / BLL / Common / Presentation, .NET 8 + Blazor Server + EF Core). Toàn bộ 5 lỗi trong yêu cầu đề bài đều **được xác nhận đúng 100%** trong code hiện tại — chi tiết dẫn chứng ở từng module bên dưới.

---

## 0. Tóm tắt hiện trạng đã xác nhận

| # | Vấn đề | File bằng chứng | Dòng/đoạn code lỗi |
|---|--------|------------------|---------------------|
| 1 | Progress là tính năng chết | `ProgressService.cs` | Chỉ có `GetUserProgressAsync` (read), **không có bất kỳ hàm ghi nào** vào `ProgressLog` trong toàn repo |
| 2 | Roadmap hardcode "Communication" | `RoadmapService.cs` | `_aiService.GenerateRoadmapAsync(userId, new List<string> { "Communication" })` |
| 3 | CaseStudy sai SkillId | `CaseStudies.razor` | `GetCaseStudiesBySkillAsync(1) // Communication for demo` — nhưng `JsonContentSeeder.cs` định nghĩa `SKILL_TIME = 1`, `SKILL_COMM = 2` → SkillId 1 thực chất là **Time Management** |
| 4 | Mentor 1-1 là giả | `Community.razor` | Nút `Connect()` chỉ set `notification = "Feature coming soon!..."`, không gọi service, không có bảng lưu hội thoại 2 chiều User↔Mentor |
| 5 | Điểm mini-game không dùng vào đâu | `MiniGameService.cs`, `MiniGameAttemptRepository.cs` | `AddAsync(attempt)` là **thao tác duy nhất** từng chạm tới `MiniGameAttempt` — không service nào đọc lại `TotalScore`/`AnswersJson` |

---

## Module 1 — Tích hợp Roadmap ↔ Progress (đồng bộ tự động)

**Mục tiêu:** `IsCompleted` của `RoadmapItem` và `PercentComplete` của `ProgressLog` phải được cập nhật tự động từ kết quả học (lý thuyết + mini-game), không cho user tự tick khống.

- [x] Xoá quyền tick tay: bỏ `@onchange="() => ToggleComplete(item)"` (checkbox tự do) trong `Roadmap.razor`; chuyển checkbox thành **chỉ đọc** (hiển thị trạng thái), việc hoàn thành do hệ thống quyết định.
- [x] Thêm bảng liên kết `TheoryLessonProgress` (User đã đọc xong lý thuyết chưa) — hiện `TheoryLesson.razor` không ghi lại trạng thái đọc.
- [x] Viết `IProgressSyncService` (BLL mới) chịu trách nhiệm duy nhất: nhận sự kiện "hoàn thành 1 đơn vị học" (theory hoặc mini-game đạt điểm) → ghi/patch `ProgressLog` → kiểm tra điều kiện đủ để tick `RoadmapItem.IsCompleted`.
- [x] `RoadmapItem` cần thêm cột liên kết tới đơn vị học cụ thể (xem Module DB) để hệ thống biết "làm gì thì item này coi là xong" thay vì chỉ có `Title`/`Description` dạng text tự do.
- [x] `ProgressService` cần thêm `UpsertProgressAsync(userId, skillId, percentDelta)` — hiện interface `IProgressService` chỉ có 1 method đọc, cần bổ sung method ghi.
- [x] `IProgressRepository` cần thêm `GetOrCreateAsync(userId, skillId)` vì hiện `ProgressLog` không bao giờ được `AddAsync` ở đâu cả → trang `/progress` mãi mãi rỗng với user mới.
- [x] Viết migration EF Core cho các cột/bảng mới, chạy `dotnet ef migrations add SyncProgressRoadmap`.
- [ ] Unit test: hoàn thành lesson tuần 1 → `ProgressLog.PercentComplete` tăng đúng tỉ lệ → `RoadmapItem` tuần 1 tự tick `IsCompleted = true`.

---

## Module 2 — Sửa bug cá nhân hoá Roadmap bằng AI (bỏ hardcode)

**Mục tiêu:** Roadmap sinh ra phải phản ánh đúng (a) danh sách skill user chọn ở `/select-skills` và (b) điểm yếu thực tế từ `EntryTestResult` (Time/Comm/Critical).

- [x] **Sửa `RoadmapService.GetUserRoadmapAsync`**: bỏ literal `"Communication"`, lấy dữ liệu thật từ:
  - `UserSkillSelection` (đã có sẵn, ghi ở `SelectSkills.razor` → `UserService.AddSkillSelectionsAsync`) — nhưng hiện **không có repository/service nào đọc lại** bảng này để dùng cho roadmap → cần thêm `IUserRepository.GetSkillSelectionsAsync(userId)`.
  - `EntryTestResult` (đã có `TimeManagementScore/Level`, `CommunicationScore/Level`, `CriticalThinkingScore/Level`) — cần thêm `IEntryTestRepository.GetLatestResultAsync(userId)` nếu chưa có, để lấy pillar nào đang ở mức `Passive`/`Developing` (yếu) làm trọng số ưu tiên.
  - Xem code mẫu chi tiết ở mục III bên dưới.
- [x] **Sửa interface `IAiRoadmapService.GenerateRoadmapAsync`**: đổi tham số `List<string> weakSkills` (tên string rời rạc, dễ sai chính tả) → `List<SkillWeaknessDto>` gồm `SkillId`, `SkillName`, `Level` để AI/logic sinh roadmap có dữ liệu có cấu trúc, tránh lặp lại kiểu lỗi hardcode string như hiện tại.
- [x] `FakeAiRoadmapService` hiện trả về **danh sách cố định 4 item bất kể input** — cần sửa để tối thiểu tôn trọng `weakSkills` đầu vào (ngay cả bản fake cũng nên phản ánh input, để test luồng cá nhân hoá không bị che giấu bởi fake data).
- [x] **Luồng "user cũ không phải làm lại test"**: hiện `Home.razor` luôn điều hướng `Start Your Journey` → `/profile-setup` bất kể trạng thái user. Cần:
  - Thêm cột `HasCompletedEntryTest` (hoặc kiểm tra `EntryTestResults.Any(u => u.UserId == id)`) ở tầng `AuthService`/`UserService`.
  - Sau login (`Login.razor` / `AuthService`), nếu user đã có `EntryTestResult` → điều hướng thẳng `/roadmap`; nếu chưa → điều hướng `/profile-setup` → `/select-skills` → `/assessment`.
- [ ] Idempotency: `GetUserRoadmapAsync` chỉ generate khi `!items.Any()` — đúng hướng, nhưng cần thêm cơ chế **regenerate có kiểm soát** khi user làm lại test (nếu sản phẩm cho phép), tránh roadmap cũ và mới đá nhau.

---

## Module 3 — Sửa lỗi logic kỹ năng trong Case Study

**Mục tiêu:** Case study phải theo đúng skill user đang học (SkillId truyền từ context, không hardcode); tăng số lượng case study để có tính "ngẫu nhiên" thật.

- [x] **Sửa `CaseStudies.razor`**: bỏ `GetCaseStudiesBySkillAsync(1) // Communication for demo`. Route cần nhận `SkillId` từ tham số điều hướng, ví dụ đổi route thành `@page "/case-studies/{SkillId:int}"` và được gọi từ `Roadmap.razor`/`TheoryLesson.razor` với đúng `item.SkillId`, giống cách `Roadmap.razor` đã làm với `/skill/{item.SkillId}/theory`.
- [x] Xác nhận lại toàn bộ mapping `SkillId` trong `JsonContentSeeder.cs` (`SKILL_TIME = 1, SKILL_COMM = 2, SKILL_CRITICAL = 3`) khớp với `Skills` seed data thực tế trong DB — soát lại mọi nơi khác trong code có magic number `1`/`2`/`3` cho skill (ví dụ `FakeAiAssessmentService.EvaluateAsync` cũng đang hardcode `SkillId = 1`).
- [ ] Tăng data mẫu: hiện chỉ có case study cho 1–2 skill demo. Bổ sung case study cho đủ 3 skill (Time/Comm/Critical), tối thiểu 4–6 case/skill để việc `Random` trong `LoadNext()` có ý nghĩa thống kê thật.
- [x] Thêm cơ chế "không lặp lại case vừa làm" (hiện `Random` có thể ra lại đúng case cũ liên tiếp) — lưu `lastCaseStudyId` trong state hoặc loại trừ khỏi danh sách trước khi random.
- [x] Case study nên tự tính là "quiz củng cố" cho Cơ chế 1 ở Module 5 (đạt `IsRecommended = true` → tính là hoàn thành, tương tự mini-game).
    
---

## Module 4 — Xây dựng tính năng Chat Mentor 1-1 thực

**Mục tiêu:** Có luồng chat thực giữa User và Mentor (không phải AI), lưu 2 chiều, hiển thị lịch sử.

- [x] Thiết kế bảng mới `MentorMessages` (không tái sử dụng `ChatMessage` vì bảng đó gắn với `ChatSender` enum chỉ có `User`/`Assistant`, phục vụ riêng cho `Assistant.razor`) — chi tiết schema ở mục II.
- [x] Thêm bảng `MentorConversations` (1 user – 1 mentor – 1 thread) để group tin nhắn, tránh trộn lẫn khi 1 user chat nhiều mentor.
- [x] Backend: `IMentorChatService` (BLL mới) với `SendMessageAsync`, `GetConversationAsync`, `GetOrCreateConversationAsync`.
- [x] Repository: `IMentorChatRepository` (DAL mới) tương tự `ChatRepository.cs` hiện có nhưng cho entity mới.
- [x] UI: trang mới `/community/chat/{mentorId}` (Blazor Server nên có thể dùng `@rendermode InteractiveServer` + polling hoặc SignalR nếu cần real-time; MVP có thể chỉ cần polling `Timer` mỗi vài giây vì đã ở môi trường Blazor Server, không cần SignalR riêng vì Blazor Server vốn đã có kết nối liên tục qua SignalR circuit — có thể tận dụng `IHubContext` sau này để đẩy tin nhắn tức thời thay vì polling).
- [x] Sửa `Community.razor`: nút "Connect" đổi từ hiển thị `notification` text sang `Nav.NavigateTo($"/community/chat/{mentor.Id}")`.
- [ ] (Tuỳ chọn giai đoạn sau) Trạng thái mentor online/offline, đánh dấu đã đọc (`IsRead`), thông báo tin nhắn mới.

---

## Module 5 — Gamification: dùng điểm mini-game cho 4 cơ chế

**Mục tiêu:** `MiniGameAttempt.TotalScore` và `AnswersJson` không còn "nằm im" mà điều khiển tiến độ, roadmap, gating và input cho AI.

- [x] **Cơ chế 1 — Cộng % Progress**: trong `MiniGameService.SubmitAttemptAsync`, sau khi lưu `attempt`, nếu `totalScore >= ngưỡng đạt (vd 7/10)` → gọi `IProgressSyncService` (Module 1) để cộng % vào `ProgressLog` của đúng `SkillId` và tick `RoadmapItem` tương ứng.
- [x] **Cơ chế 2 — Chèn RoadmapItem ôn tập động**: nếu điểm dưới trung bình (`< 5/10` hoặc theo ngưỡng cấu hình) → tự tạo thêm 1 `RoadmapItem` mới với `WeekNumber = tuần hiện tại + 1`, `Title = "Ôn tập: {SkillName}"`, đánh dấu để phân biệt với item do AI sinh gốc (thêm cột `IsRemedial` — xem Module DB).
- [x] **Cơ chế 3 — Gating theo tuần**: `RoadmapService`/`Roadmap.razor` cần kiểm tra "tuần N+1 chỉ mở khi tất cả `RoadmapItem` gắn mini-game/quiz của tuần N đạt điểm tối thiểu" — thêm cờ `IsLocked` (tính toán, không lưu DB) khi trả `RoadmapDto`, disable nút "Bắt đầu học" nếu tuần đang bị khoá.
- [x] **Cơ chế 4 — Input cho AI cá nhân hoá sâu**: viết `IAnswerAnalysisService` đọc `AnswersJson` của các `MiniGameAttempt` gần nhất, tổng hợp dạng câu sai lặp lại theo `MiniGameQuestion`/`Option`, đưa vào `GenerateRoadmapAsync` (Module 2) như một nguồn dữ liệu bổ sung ngoài `EntryTestResult`, để mỗi lần roadmap được điều chỉnh (Cơ chế 2) sẽ "biết" chính xác user sai ở dạng câu hỏi nào chứ không chỉ sai skill nào.
- [x] Thêm cấu hình ngưỡng điểm (`PassingScoreThreshold`, `RemedialScoreThreshold`) vào `appsettings.json` thay vì hardcode số 7/10 rải rác trong code.
- [ ] Viết test cho toàn bộ 4 cơ chế, đặc biệt test race-condition khi user submit nhiều mini-game liên tiếp (đảm bảo `ProgressLog.PercentComplete` không vượt quá 100 và không cộng dồn trùng khi đã tick `RoadmapItem`).

---

## II. Phác thảo cấu trúc Database cần chỉnh sửa / thêm mới

### Bảng sửa đổi

**`RoadmapItem`** (thêm cột để liên kết được với đơn vị học cụ thể, phục vụ Module 1 & 5):
```
+ TheoryLessonId   int?     (FK → TheoryLesson, null nếu item không gắn lý thuyết)
+ MiniGameId       int?     (FK → MiniGame, null nếu item không gắn mini-game)
+ IsRemedial       bool     (default false — đánh dấu item do hệ thống tự chèn ôn tập, Cơ chế 2)
+ IsLocked         bool     (default false — hoặc tính runtime, không cần persist)
```

**`ProgressLog`**: giữ nguyên cấu trúc, chỉ cần đảm bảo có service ghi (không cần đổi schema).

### Bảng mới

**`MentorConversations`**
```
Id              int PK
UserId          int FK → User
MentorId        int FK → Mentor
CreatedAt       datetime
UNIQUE (UserId, MentorId)
```

**`MentorMessages`**
```
Id                  int PK
ConversationId      int FK → MentorConversations
SenderType          enum (User, Mentor)
Content             string (required)
CreatedAt           datetime
IsRead              bool default false
```

**`TheoryLessonProgress`** (để biết user đã học xong lý thuyết chưa, dùng ở Module 1)
```
Id              int PK
UserId          int FK → User
TheoryLessonId  int FK → TheoryLesson
CompletedAt     datetime
UNIQUE (UserId, TheoryLessonId)
```

### Migration cần chạy
```bash
dotnet ef migrations add SoftSync_Gamification_MentorChat_ProgressSync -p SoftSync.DAL -s SoftSync.Presentation
dotnet ef database update -p SoftSync.DAL -s SoftSync.Presentation
```

---

## III. Code mẫu — sửa Hardcode ở RoadmapService + Auto-sync điểm Mini-game → ProgressLog

### 1. Sửa `IUserRepository` / `IEntryTestRepository` (bổ sung method đọc dữ liệu thật)

```csharp
// SoftSync.DAL/Repositories/UserRepository.cs — bổ sung
public interface IUserRepository : IRepository<User>
{
    // ... các method hiện có
    Task<List<int>> GetSelectedSkillIdsAsync(int userId);
}

public class UserRepository : Repository<User>, IUserRepository
{
    // ... constructor hiện có
    public async Task<List<int>> GetSelectedSkillIdsAsync(int userId)
    {
        return await _context.Set<UserSkillSelection>()
            .Where(s => s.UserId == userId)
            .Select(s => s.SkillId)
            .ToListAsync();
    }
}
```

```csharp
// SoftSync.DAL/Repositories/EntryTestRepository.cs — bổ sung (nếu chưa có)
public interface IEntryTestRepository
{
    // ... các method hiện có
    Task<EntryTestResult?> GetLatestResultAsync(int userId);
}

public async Task<EntryTestResult?> GetLatestResultAsync(int userId)
{
    return await _context.EntryTestResults
        .Where(r => r.UserId == userId)
        .OrderByDescending(r => r.CreatedAt)
        .FirstOrDefaultAsync();
}
```

### 2. DTO có cấu trúc thay cho `List<string>` rời rạc

```csharp
// SoftSync.Common/Dtos/Roadmap/SkillWeaknessDto.cs
namespace SoftSync.Common.Dtos.Roadmap;

public class SkillWeaknessDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public AssessmentLevel Level { get; set; }   // Passive/Developing/Proactive/Exceptional
    public int Score { get; set; }
}
```

### 3. `RoadmapService.GetUserRoadmapAsync` — bỏ hardcode, dùng dữ liệu thật

```csharp
public class RoadmapService : IRoadmapService
{
    private readonly IRoadmapRepository _roadmapRepo;
    private readonly IAiRoadmapService _aiService;
    private readonly IUserRepository _userRepo;
    private readonly IEntryTestRepository _entryTestRepo;
    private readonly ISkillRepository _skillRepo;

    public RoadmapService(
        IRoadmapRepository roadmapRepo,
        IAiRoadmapService aiService,
        IUserRepository userRepo,
        IEntryTestRepository entryTestRepo,
        ISkillRepository skillRepo)
    {
        _roadmapRepo = roadmapRepo;
        _aiService = aiService;
        _userRepo = userRepo;
        _entryTestRepo = entryTestRepo;
        _skillRepo = skillRepo;
    }

    public async Task<RoadmapDto> GetUserRoadmapAsync(int userId)
    {
        var items = await _roadmapRepo.GetByUserIdAsync(userId);
        if (!items.Any())
        {
            var weaknesses = await BuildWeaknessListAsync(userId);
            var generated = await _aiService.GenerateRoadmapAsync(userId, weaknesses);

            foreach (var item in generated.Items)
            {
                await _roadmapRepo.AddAsync(new RoadmapItem
                {
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
            Items = items.Select(i => new RoadmapItemDto
            {
                Id = i.Id,
                SkillId = i.SkillId,
                WeekNumber = i.WeekNumber,
                Title = i.Title,
                Description = i.Description,
                IsCompleted = i.IsCompleted
            }).ToList()
        };
    }

    // Ghép: skill user tự chọn ở /select-skills + mức độ yếu thật từ EntryTestResult
    private async Task<List<SkillWeaknessDto>> BuildWeaknessListAsync(int userId)
    {
        var selectedSkillIds = await _userRepo.GetSelectedSkillIdsAsync(userId);
        var latestResult = await _entryTestRepo.GetLatestResultAsync(userId);
        var allSkills = (await _skillRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Name);

        var pillarScores = latestResult == null
            ? new Dictionary<int, (int Score, AssessmentLevel Level)>()
            : new Dictionary<int, (int, AssessmentLevel)>
            {
                [1] = (latestResult.TimeManagementScore, latestResult.TimeManagementLevel),   // SKILL_TIME = 1
                [2] = (latestResult.CommunicationScore, latestResult.CommunicationLevel),      // SKILL_COMM = 2
                [3] = (latestResult.CriticalThinkingScore, latestResult.CriticalThinkingLevel) // SKILL_CRITICAL = 3
            };

        // Nếu user chưa chọn skill cụ thể nào, fallback: lấy tất cả skill có kết quả test
        var targetSkillIds = selectedSkillIds.Any() ? selectedSkillIds : pillarScores.Keys.ToList();

        return targetSkillIds.Select(skillId => new SkillWeaknessDto
        {
            SkillId = skillId,
            SkillName = allSkills.TryGetValue(skillId, out var name) ? name : $"Skill {skillId}",
            Score = pillarScores.TryGetValue(skillId, out var p) ? p.Score : 0,
            Level = pillarScores.TryGetValue(skillId, out var p2) ? p2.Level : AssessmentLevel.Developing
        }).ToList();
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
```

> Lưu ý: `IAiRoadmapService.GenerateRoadmapAsync` cũng cần đổi chữ ký từ `List<string> weakSkills` → `List<SkillWeaknessDto> weaknesses`, và `FakeAiRoadmapService` cần sửa để phản ánh input thay vì trả cứng 4 item như hiện tại.

### 4. Auto-sync điểm Mini-game → ProgressLog + tick Roadmap (Cơ chế 1 & 3, Module 5 & 1)

```csharp
// SoftSync.BLL/Interfaces/IProgressSyncService.cs
public interface IProgressSyncService
{
    Task SyncFromMiniGameAsync(int userId, int skillId, int miniGameId, int totalScore, int maxScore);
}
```

```csharp
// SoftSync.BLL/Services/ProgressSyncService.cs
public class ProgressSyncService : IProgressSyncService
{
    private const double PassingRatio = 0.7; // >= 7/10

    private readonly IProgressRepository _progressRepo;
    private readonly IRoadmapRepository _roadmapRepo;

    public ProgressSyncService(IProgressRepository progressRepo, IRoadmapRepository roadmapRepo)
    {
        _progressRepo = progressRepo;
        _roadmapRepo = roadmapRepo;
    }

    public async Task SyncFromMiniGameAsync(int userId, int skillId, int miniGameId, int totalScore, int maxScore)
    {
        bool passed = maxScore > 0 && (double)totalScore / maxScore >= PassingRatio;
        if (!passed) return; // Cơ chế 2 (chèn ôn tập) xử lý riêng khi KHÔNG đạt — xem service khác

        // 1) Cộng % vào ProgressLog (Cơ chế 1)
        var log = await _progressRepo.GetOrCreateAsync(userId, skillId);
        log.PercentComplete = Math.Min(100, log.PercentComplete + 10); // bước cộng cấu hình được
        log.UpdatedAt = DateTime.UtcNow;
        _progressRepo.Update(log);

        // 2) Tick RoadmapItem gắn đúng mini-game này (Cơ chế 3 — mở khoá tuần sau)
        var items = await _roadmapRepo.GetByUserIdAsync(userId);
        var matched = items.FirstOrDefault(i => i.SkillId == skillId && !i.IsCompleted);
        // Điều kiện khớp chính xác cần dựa vào cột RoadmapItem.MiniGameId (xem Module DB)
        if (matched != null)
        {
            matched.IsCompleted = true;
            _roadmapRepo.Update(matched);
        }

        await _progressRepo.SaveChangesAsync();
    }
}
```

```csharp
// Gọi từ MiniGameService.SubmitAttemptAsync sau khi lưu attempt:
public async Task<int> SubmitAttemptAsync(int userId, int miniGameId, List<MiniGameAnswerDto> answers)
{
    var optionIds = answers.Select(a => a.OptionId).ToList();
    var options = await _miniGameRepo.GetOptionsByIdsAsync(optionIds);
    int totalScore = options.Sum(o => o.Points);

    var attempt = new MiniGameAttempt
    {
        UserId = userId,
        MiniGameId = miniGameId,
        TotalScore = totalScore,
        PlayedAt = DateTime.UtcNow,
        AnswersJson = JsonSerializer.Serialize(answers.ToDictionary(a => a.QuestionId, a => a.OptionId))
    };
    await _attemptRepo.AddAsync(attempt);
    await _attemptRepo.SaveChangesAsync();

    var miniGame = await _miniGameRepo.GetByIdAsync(miniGameId); // cần thêm nếu chưa có
    int maxScore = answers.Count * 10; // hoặc lấy điểm tối đa thật từ options
    await _progressSyncService.SyncFromMiniGameAsync(userId, miniGame.SkillId, miniGameId, totalScore, maxScore);

    return totalScore;
}
```

> Cần bổ sung `IProgressRepository.GetOrCreateAsync` (hiện repo chỉ có `GetByUserIdAsync`) và `IProgressRepository.Update` (interface `IRepository<T>` đã có sẵn `Update`, chỉ cần dùng đúng).

---

## IV. Thứ tự triển khai đề xuất (ưu tiên rủi ro & phụ thuộc)

1. **Module 2** (bỏ hardcode Roadmap) — vì Module 1 & 5 đều phụ thuộc vào Roadmap sinh đúng dữ liệu skill trước.
2. **Module 3** (sửa SkillId Case Study) — độc lập, sửa nhanh, giá trị cao/rủi ro thấp.
3. **Module 1 + Module 5** (Progress sync + Gamification 4 cơ chế) — làm song song vì dùng chung `IProgressSyncService`.
4. **Module 4** (Mentor chat) — độc lập hoàn toàn, có thể làm song song với các module trên bởi team khác, không block nhau.

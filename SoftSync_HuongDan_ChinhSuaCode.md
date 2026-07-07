# Hướng dẫn chỉnh sửa code — SoftSync
*(Chỉ liệt kê chỗ cần sửa + nội dung sửa. Không đụng phần AI: `FakeServices.cs`, `IAiAssessmentService`, `IAiAssistantService`, `IAiRoadmapService`.)*

---

## 1. Lý thuyết bị dump JSON thô → phải parse thành Markdown thật

**File:** `SoftSync.DAL/Seed/JsonContentSeeder.cs`
**Method:** `SeedTheoryAsync(...)`

**Vấn đề:** dòng
```csharp
ContentMarkdown = $"```json\n{raw}\n```",
```
đang nhét nguyên JSON vào làm nội dung bài học.

**Sửa:**
1. Đổi 3 file JSON lý thuyết (`time-theory.json`, `communication-theory.json`, `critical-theory.json`) — hoặc giữ nguyên cấu trúc key nhưng **thêm 1 field mới** `"lessonBody": "..."` chứa nội dung markdown viết tay hoàn chỉnh (đoạn văn, không phải object rời rạc) do các cô cung cấp.
2. Trong `SeedTheoryAsync`, thay vì serialize nguyên `raw`, deserialize vào 1 class model tương ứng từng skill (`CommunicationTheoryModel`, `TimeTheoryModel`, `CriticalTheoryModel`) rồi build chuỗi markdown bằng `StringBuilder`, ví dụ:
   ```csharp
   var model = JsonSerializer.Deserialize<CommunicationTheoryModel>(raw, options);
   var md = new StringBuilder();
   md.AppendLine($"## {model.SkillName}");
   md.AppendLine(model.LessonBody); // nội dung viết tay, không phải object
   ```
3. Xóa toàn bộ chuỗi `[cite: xxx]` còn sót trong 3 file JSON trước khi build markdown (dùng `Regex.Replace(text, @"\[cite:.*?\]", "")` tạm thời nếu chưa kịp làm sạch tay).
4. Thêm field `videoUrl` vào từng file theory JSON (`time-theory.json`, ...) và gán vào `TheoryLesson.VideoUrl` trong `SeedTheoryAsync` (hiện đang bỏ trống, chỉ có `Title` và `ContentMarkdown`).
5. Nếu muốn nhiều bài/kỹ năng thay vì 1 bài duy nhất: đổi file JSON đầu vào thành **mảng bài học** thay vì object đơn, và thêm vòng `foreach` khi seed thay vì add 1 `TheoryLesson` như hiện tại (đoạn `db.TheoryLessons.Add(new TheoryLesson {...})` chỉ chạy 1 lần/skill).

---

## 2. Mini game: tách nhiều game/kỹ năng + không bỏ sót loại câu hỏi

**File:** `SoftSync.DAL/Seed/JsonContentSeeder.cs`
**Method:** `SeedMiniGamesAsync(...)`

### 2a. Đang gộp tất cả câu hỏi 1 kỹ năng vào 1 game
Đoạn:
```csharp
if (!gamesBySkill.TryGetValue(skillId, out var game))
{
    game = new MiniGame { SkillId = skillId, Name = $"Tình huống thực hành - {s.SkillId}", ... };
    gamesBySkill[skillId] = game;
```
**Sửa:** thêm field `"gameGroup"` (hoặc `"gameId"`) vào từng object trong `practical-scenarios.json` để phân loại câu hỏi nào thuộc game nào (VD: `"gameGroup": "COM_GAME_1"`). Đổi key gom nhóm từ `skillId` sang `s.GameGroup` thay vì chỉ `skillId`:
```csharp
var gamesByGroup = new Dictionary<string, MiniGame>();
foreach (var s in raw)
{
    if (!gamesByGroup.TryGetValue(s.GameGroup, out var game))
    {
        game = new MiniGame { SkillId = SkillMap[s.SkillId], Name = s.GameName ?? $"Game - {s.GameGroup}", ... };
        gamesByGroup[s.GameGroup] = game;
        db.MiniGames.Add(game);
    }
    ...
}
```
Nhớ thêm `GameGroup`, `GameName` vào class `RawScenario` (cuối file `JsonContentSeeder.cs`).

### 2b. Bỏ sót `FACT_OR_OPINION` và `IDENTIFY_FALLACY`
Đoạn:
```csharp
// Bỏ qua các dạng chưa map option (FACT_OR_OPINION, IDENTIFY_FALLACY) ở bước đầu này
if (s.Options == null || s.Options.Count == 0) continue;
```
**Sửa:** chuẩn hóa schema JSON để 2 dạng này cũng có field `options[]` giống `MULTIPLE_CHOICE`, thay vì dùng field riêng `items[]` (cho FACT_OR_OPINION) hoặc `correctAnswer` (cho IDENTIFY_FALLACY). Ví dụ chuyển `CRIT_FALLACY_01` từ:
```json
"correctAnswer": "BANDWAGON",
"feedback": "..."
```
sang:
```json
"options": [
  { "text": "Ngụy biện Bandwagon (Ad Populum)", "points": 10, "isCorrect": true, "feedback": "..." },
  { "text": "Ngụy biện Straw Man", "points": 0, "feedback": "..." },
  { "text": "Ngụy biện Ad Hominem", "points": 0, "feedback": "..." }
]
```
Làm tương tự với `CRIT_SCENARIO_01` (FACT_OR_OPINION): mỗi `statement` trong `items[]` tách thành 1 `MiniGameQuestion` riêng, với 2 `options` là `FACT` / `OPINION` (điểm 10 cho đáp án đúng, 0 cho sai). Sau khi chuẩn hóa, dòng `continue` ở trên có thể xóa vì mọi record đều có `options`.

### 2c. Thiếu dữ liệu số lượng
Không phải chỗ sửa code — cần bổ sung thêm record vào `practical-scenarios.json` cho đủ ~20 câu/game (chỉ là thêm object vào mảng JSON theo đúng schema đã chuẩn hóa ở 2a/2b, không cần sửa code seeder thêm).

---

## 3. Case Study: sai `SkillId` + data tiếng Anh

**File:** `SoftSync.DAL/Data/SoftSyncDbContext.cs`
**Method:** `SeedData(...)`, mục `// 3. Case Studies`

Sửa trực tiếp 2 dòng:
```csharp
new CaseStudy { Id = 1, Title = "Group Communication", ..., SkillId = 1 }   // SkillId 1 = Time Management → phải đổi thành 2 (Communication)
new CaseStudy { Id = 2, Title = "Missed Deadline", ..., SkillId = 3 }       // phải đổi thành 1 (Time Management)
```
**Sửa:** đổi `SkillId = 1` → `SkillId = 2` ở case 1 (nội dung giao tiếp nhóm), và `SkillId = 3` → `SkillId = 1` ở case 2 (nội dung deadline/thời gian). Đồng thời dịch `Title`, `Scenario`, và 4 dòng trong `modelBuilder.Entity<CaseStudyOption>().HasData(...)` sang tiếng Việt khi có nội dung thật từ các cô.

> Vì đây là data qua Migration `HasData`, sau khi sửa cần tạo migration mới: `dotnet ef migrations add FixCaseStudySkillId` (không sửa trực tiếp migration cũ `20260706221107_InitData.cs`).

---

## 4. Thiếu "câu hỏi trắc nghiệm" sau lý thuyết

Hiện schema **chưa có bảng nào** cho loại này. Cần quyết định 1 trong 2 hướng — không phải chỗ sửa nhỏ mà là bổ sung tính năng:

**Hướng A (khuyến nghị, ít việc hơn):** tái sử dụng `MiniGame`/`MiniGameQuestion` sẵn có, thêm 1 field `IsTheoryQuiz` (bool) vào entity `MiniGame` (`SoftSync.DAL/Entities/MiniGame.cs`) để phân biệt "quiz kiểm tra lý thuyết" với "mini game luyện tập" — không cần entity mới.

**Hướng B:** tạo entity mới `TheoryQuizQuestion`/`TheoryQuizOption` tương tự cấu trúc `MiniGameQuestion`/`MiniGameOption`, thêm `DbSet` vào `SoftSyncDbContext.cs`, thêm repository/service/interface riêng (`ITheoryQuizService`) theo đúng pattern các service khác trong `SoftSync.BLL/Interfaces/`.

→ Cần chốt hướng với team trước khi code, vì ảnh hưởng tới cả DAL/BLL/Presentation.

---

## 5. Dọn code legacy (Assessment cũ)

Xóa các file sau (không còn được UI gọi, đã thay bằng `EntryTestService`):
- `SoftSync.BLL/Interfaces/IAssessmentService.cs`
- `SoftSync.BLL/Services/AssessmentService.cs`
- `SoftSync.Common/Dtos/Assessment/AssessmentQuestionDto.cs`, `AssessmentOptionDto.cs`, `AssessmentResultDto.cs` *(chỉ xóa nếu không dùng ở nơi khác — `AssessmentResultDto` đang được `IAiAssessmentService` dùng, **giữ lại file này**, chỉ xóa `AssessmentQuestionDto.cs` và `AssessmentOptionDto.cs`)*

Trong `SoftSync.DAL/Data/SoftSyncDbContext.cs`, xóa đoạn comment chết (mục `// 2. Assessment Questions`) đang để `//` che code cũ — không ảnh hưởng chạy nhưng nên dọn cho sạch.

Kiểm tra `Program.cs` (DI registration) xem có dòng `services.AddScoped<IAssessmentService, AssessmentService>()` không — nếu có, xóa luôn dòng đó khi xóa file.

---

## 6. Thứ tự ưu tiên đề xuất
1. Mục 1 (lý thuyết) — vì đây là thứ hiển thị lỗi rõ nhất cho người dùng ngay bây giờ.
2. Mục 2b (chuẩn hóa schema mini-game) — vì đang mất data thật.
3. Mục 3 (case study SkillId) — sửa nhanh, chỉ 2 dòng.
4. Mục 2a (tách nhiều game) + bổ sung số lượng câu hỏi — cần thời gian nhập liệu.
5. Mục 4 (quyết định thêm quiz) — cần họp chốt hướng trước.
6. Mục 5 (dọn legacy) — làm cuối cùng, không gấp.

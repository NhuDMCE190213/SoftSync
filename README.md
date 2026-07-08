# SoftSync

**SoftSync** là nền tảng web giúp sinh viên/người đi làm tự đánh giá và cải thiện các kỹ năng mềm (Quản lý thời gian, Giao tiếp, Tư duy phản biện...) thông qua: bài test đầu vào, lộ trình học cá nhân hoá, bài học lý thuyết, mini-game, case study tình huống, chat với AI Assistant, và kết nối với Mentor thật.

> Tài liệu này được viết cho người **chưa từng dùng .NET hay SQL Server** — cứ làm theo từng bước là chạy được dự án trên máy để test.

---

## 1. Trạng thái dự án (MVP)

Dự án đã ở mức **MVP chạy được đầy đủ luồng chính**, không chỉ là giao diện tĩnh:

- Đăng ký / đăng nhập / quên mật khẩu (gửi email thật qua SMTP) / đổi mật khẩu / cập nhật hồ sơ.
- Bài test đầu vào (Entry Test) chấm điểm theo 3 trụ kỹ năng, có seed sẵn câu hỏi.
- Sinh **lộ trình học cá nhân hoá theo đúng kỹ năng user chọn + điểm yếu thực tế** từ bài test (không còn hardcode "Communication" như bản nháp đầu).
- Bài học lý thuyết theo từng kỹ năng, có lưu tiến độ đã đọc (`TheoryLessonProgress`).
- Mini-game trắc nghiệm tình huống, chấm điểm và **tự động cộng % Progress + mở khoá tuần tiếp theo** trong roadmap khi đạt ngưỡng điểm.
- Case Study tình huống **theo đúng kỹ năng đang học** (đã sửa lỗi hardcode SkillId), có bộ chọn kỹ năng, không lặp lại case vừa làm.
- Trang Progress hiển thị % tiến độ thật (được ghi tự động, không còn là màn hình chết).
- Chat với **AI Assistant** (hiện đang dùng bản mock có kịch bản trả lời theo từ khoá — xem mục 6).
- **Chat Mentor 1-1 thật**: danh sách mentor → bấm "Connect" → vào phòng chat riêng, lưu lịch sử 2 chiều User ↔ Mentor (không còn là nút "Feature coming soon").
- Cơ chế gating theo tuần: tuần sau chỉ mở khi hoàn thành đủ điều kiện của tuần trước.
- Phân tích lỗi sai lặp lại từ mini-game (`AnswerAnalysisService`) để đưa vào roadmap cá nhân hoá sâu hơn.

Nói cách khác: 5 lỗi lớn được liệt kê trong `SoftSync.DAL/SoftSync_GapAnalysis_Checklist.md` (Progress chết, Roadmap hardcode, CaseStudy sai SkillId, Mentor giả, điểm mini-game vô nghĩa) **đều đã được xử lý trong code hiện tại**. File đó vẫn được giữ lại trong repo như nhật ký đối chiếu — không phải việc còn tồn đọng.

Phần còn thiếu để lên production thật sự nằm ở mục **7. Việc cần làm tiếp theo** bên dưới, chủ yếu là: AI thật (hiện đang mock), dữ liệu mẫu còn ít, một số polish và test tự động.

---

## 2. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ / Framework | C# / .NET 8, ASP.NET Core Blazor Server (Interactive Server Render Mode) |
| Kiến trúc | 4 layer: `Presentation` (UI) → `BLL` (business logic) → `DAL` (data access, EF Core) → `Common` (DTO/Enum dùng chung) |
| Cơ sở dữ liệu | SQL Server (qua Entity Framework Core 8, tự chạy migration khi khởi động — **không cần cài EF CLI tool**) |
| Xác thực | Cookie Authentication (không dùng JWT/API riêng) |
| Gửi email | SMTP (Gmail App Password) cho luồng quên mật khẩu |
| Seed dữ liệu | Tự động đọc file JSON trong `SeedData/` khi app khởi động lần đầu |

Vì đây là ứng dụng **Blazor Server** (không phải SPA + API tách rời), bạn chỉ cần chạy **một lệnh duy nhất** để có cả giao diện lẫn backend.

---

## 3. Yêu cầu cài đặt (chỉ 2 thứ)

Bạn không cần biết gì về .NET hay SQL Server trước đó, chỉ cần cài đúng 2 phần mềm sau:

1. **.NET 8 SDK** (miễn phí, có cho Windows/macOS/Linux): https://dotnet.microsoft.com/download/dotnet/8.0
   - Sau khi cài, mở terminal/cmd gõ `dotnet --version` → thấy số bắt đầu bằng `8.` là thành công.
2. **SQL Server** — chọn **1 trong 2 cách** dưới đây, cách A dễ nhất nếu bạn đã có Docker:

### Cách A — Dùng Docker (khuyên dùng, không cần cài SQL Server thật)

Cài Docker Desktop (https://www.docker.com/products/docker-desktop/), sau đó chạy lệnh sau trong terminal (chạy 1 lần):

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name softsync-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

> Máy Mac chip Apple Silicon (M1/M2/M3): image trên chưa hỗ trợ arm64 native, hãy đổi thành:
> `mcr.microsoft.com/azure-sql-edge` (tương thích cú pháp T-SQL tương tự, đủ dùng cho dự án này).

Kiểm tra container đang chạy: `docker ps` — thấy `softsync-sql` là được.

### Cách B — Cài SQL Server Express trực tiếp (nếu dùng Windows và không muốn dùng Docker)

Tải **SQL Server 2022 Express**: https://www.microsoft.com/sql-server/sql-server-downloads (chọn bản Express, miễn phí) và cài theo mặc định (chế độ "Basic"). Ghi nhớ tên instance hiển thị lúc cài xong (thường là `localhost` hoặc `.\SQLEXPRESS`).

---

## 4. Cài đặt & chạy dự án

### Bước 1 — Tải mã nguồn

```bash
git clone <URL_repo_cua_ban>
cd SoftSync
```

### Bước 2 — Cấu hình chuỗi kết nối database

Mở file `SoftSync.Presentation/appsettings.json`, sửa đoạn `ConnectionStrings`:

- **Nếu dùng Cách A (Docker)** ở trên, thay bằng:
  ```json
  "ConnectionStrings": {
    "SoftSyncDb": "Server=localhost,1433;Database=SoftSyncDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
  ```
- **Nếu dùng Cách B (SQL Server Express trên Windows)**, giữ nguyên giá trị mặc định đã có sẵn trong file:
  ```json
  "ConnectionStrings": {
    "SoftSyncDb": "Server=.;Database=SoftSyncDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
  ```
  (nếu tên instance của bạn là `.\SQLEXPRESS` thì sửa `Server=.` thành `Server=.\SQLEXPRESS`).

> Không cần tự tạo database `SoftSyncDb` bằng tay — ứng dụng sẽ tự tạo và tự chạy toàn bộ migration khi khởi động lần đầu (xem `Program.cs`, dòng `db.Database.MigrateAsync()`).

### Bước 3 — Chạy ứng dụng

```bash
cd SoftSync.Presentation
dotnet run
```

Lần chạy đầu sẽ hơi lâu vì .NET tải các gói phụ thuộc và EF Core tạo toàn bộ bảng + nạp dữ liệu mẫu (câu hỏi test, mini-game, bài lý thuyết, mentor...). Khi thấy dòng dạng:

```
Now listening on: http://localhost:5104
```

→ mở trình duyệt vào **http://localhost:5104** là dùng được.

### Bước 4 — Tạo tài khoản để test

Vào trang **Register** và tạo tài khoản mới (email bất kỳ dạng `a@b.com`, không cần email thật vì SMTP chỉ dùng cho chức năng quên mật khẩu). Sau khi đăng ký sẽ tự đăng nhập và được đưa qua luồng: chọn kỹ năng muốn cải thiện → làm bài test đầu vào → nhận lộ trình học cá nhân hoá.

> Repo có sẵn 1 user mẫu (`demo@softsync.vn`) từ migration ban đầu, nhưng mật khẩu của user này không được lưu ở dạng rõ trong mã nguồn nên **không dùng để đăng nhập được** — hãy tạo tài khoản mới qua Register.

---

## 5. Cấu trúc thư mục

```
SoftSync.Common/          DTOs, Enum dùng chung giữa các layer
SoftSync.DAL/             Entity (bảng DB), Repository, DbContext, Migrations, dữ liệu seed
SoftSync.BLL/             Business logic (Service), interface cho AI (đang có bản mock "Fake...")
SoftSync.Presentation/    Blazor Server: các trang .razor, Program.cs, appsettings, SeedData (json)
```

Một vài file đáng chú ý khi cần chỉnh sửa:

- `SoftSync.Presentation/Program.cs` — nơi đăng ký DI, migrate + seed DB, các endpoint đăng nhập/đăng ký (dùng HTML form POST thường, không qua SignalR, để `SignInAsync` hoạt động đúng trong Blazor Server).
- `SoftSync.BLL/Services/Fake/FakeServices.cs` — nơi đang mock 3 dịch vụ AI (`FakeAiAssessmentService`, `FakeAiAssistantService`, `FakeAiRoadmapService`).
- `SoftSync.DAL/Seed/JsonContentSeeder.cs` + `SoftSync.Presentation/SeedData/*.json` — dữ liệu mẫu câu hỏi test/mini-game/lý thuyết, chỉnh sửa file JSON là đủ, không cần viết migration mới cho nội dung.

---

## 6. Về phần AI — hiện đang là bản mock

3 dịch vụ AI đang chạy bằng logic giả lập (rule-based), **không gọi mô hình AI thật**:

| Interface | Bản đang dùng (Program.cs) | Ghi chú |
|---|---|---|
| `IAiAssessmentService` | `FakeAiAssessmentService` | Tính điểm theo công thức đơn giản, không phân tích ngữ nghĩa câu trả lời |
| `IAiAssistantService` | `FakeAiAssistantService` | Trả lời theo từ khoá cứng ("giao tiếp", "lộ trình"...) |
| `IAiRoadmapService` | `FakeAiRoadmapService` | Sinh roadmap dựa trên rule, có tôn trọng input thật (skill/điểm yếu) chứ không random |

Đã có sẵn khung `RealAiAssistantService.cs` gọi ra một API ngoài qua `HttpClient` đặt tên `"AiApi"` (base URL cấu hình ở `appsettings.json` → `AiApi:BaseUrl`), nhưng **chưa được đăng ký thay cho bản Fake trong `Program.cs`** và chưa có endpoint AI thật để trỏ tới. Đây là điểm cắm nối rõ ràng nhất khi có API AI thật (kể cả gọi thẳng Anthropic/OpenAI API).

---

## 7. Việc cần làm tiếp theo

### Ưu tiên cao (ảnh hưởng trải nghiệm demo/MVP)
- [ ] Nối AI thật cho 3 interface ở mục 6 (thay `Fake...` bằng implementation gọi LLM thật trong `Program.cs`).
- [ ] Bổ sung thêm dữ liệu mẫu: hiện case study/mini-game/câu hỏi lý thuyết còn ít cho mỗi kỹ năng, nên random/tiến độ chưa "thật" khi test với tài khoản mới trong thời gian dài.
- [ ] Cho phép user làm lại Entry Test có kiểm soát (regenerate roadmap) — hiện luồng "test lại" chưa được thiết kế rõ, roadmap cũ/mới có thể đá nhau nếu làm lại tuỳ tiện.

### Ưu tiên trung bình
- [ ] Mentor chat mới đang polling định kỳ (không dùng SignalR Hub riêng để đẩy tin nhắn realtime) — có thể nâng cấp lên đẩy tin nhắn tức thời qua `IHubContext` nếu cần trải nghiệm mượt hơn.
- [ ] Trạng thái online/offline của mentor, đánh dấu tin nhắn đã đọc — chưa có.
- [ ] Cấu hình ngưỡng điểm đạt/ôn tập (`Gamification:PassingScoreThreshold`, `RemedialScoreThreshold`) đã được đưa ra `appsettings.json`, nhưng nên rà lại toàn bộ code để chắc chắn không còn số hardcode (7/10, 5/10...) rải rác ở nơi khác.

### Ưu tiên thấp / kỹ thuật
- [ ] Chưa có file `.sln` — hiện dùng `ProjectReference` giữa các `.csproj` nên `dotnet run` vẫn build đủ cả chuỗi layer, nhưng nếu dùng Visual Studio thì nên tạo `.sln` cho tiện mở cả 4 project cùng lúc (`dotnet new sln && dotnet sln add **/*.csproj`).
- [ ] Chưa có test tự động (unit test/integration test) cho các luồng: sync Progress ↔ Roadmap, gating theo tuần, chấm điểm mini-game.
- [ ] Chưa có CI (GitHub Actions) để tự build/test khi có Pull Request.
- [ ] Nên thêm `.gitignore` (đã kèm sẵn trong repo này) để tránh commit nhầm `bin/`, `obj/`.

---

## 8. Lỗi thường gặp khi cài đặt

| Lỗi | Cách xử lý |
|---|---|
| `A network-related or instance-specific error...` khi chạy `dotnet run` | SQL Server chưa chạy hoặc sai `Server=` trong connection string. Kiểm tra `docker ps` (Cách A) hoặc tên instance SQL Express (Cách B). |
| `Login failed for user 'sa'` | Sai `Password` trong connection string so với `MSSQL_SA_PASSWORD` lúc `docker run`. Mật khẩu SQL Server yêu cầu đủ mạnh (chữ hoa, thường, số, ký tự đặc biệt, tối thiểu 8 ký tự). |
| Trang trắng / lỗi 500 khi mở `http://localhost:5104` | Xem log trong terminal đang chạy `dotnet run` — thường do migration lỗi vì DB chưa kết nối được, xử lý theo lỗi phía trên trước. |
| Cổng 1433 đã có ứng dụng khác dùng | Đổi cổng map khi `docker run`, ví dụ `-p 14330:1433`, rồi sửa connection string thành `Server=localhost,14330;...`. |
| Cổng 5104/7212 bị chiếm | Sửa `applicationUrl` trong `SoftSync.Presentation/Properties/launchSettings.json`. |

---

## 9. Đóng góp

Vì đây là dự án MVP đang trong giai đoạn hoàn thiện, khi thêm tính năng mới nên:

1. Không sửa trực tiếp entity đã có migration — tạo migration mới: `dotnet ef migrations add TenMigration --project SoftSync.DAL --startup-project SoftSync.Presentation` (cần cài công cụ 1 lần: `dotnet tool install --global dotnet-ef`).
2. Giữ nguyên convention hiện tại: Interface đặt ở `BLL/Interfaces`, implementation ở `BLL/Services`, Repository ở `DAL/Repositories`.
3. Cập nhật lại mục 7 (Việc cần làm) trong README này khi hoàn thành một hạng mục, để người sau nắm được tiến độ thật.

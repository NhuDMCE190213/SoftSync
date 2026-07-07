# SoftSync — Checklist rà soát Data & Tính năng
*(Không bao gồm phần AI — sẽ làm riêng ở giai đoạn sau)*

---

## 1. Đã làm (✅ Done)

- [x] Kiến trúc 4 lớp: Presentation (Blazor) / BLL / DAL / Common — đã dựng xong, chạy được.
- [x] Entity Framework Core + Migration + Seed pipeline (`JsonContentSeeder.cs`).
- [x] Bảng Skill: 3 kỹ năng cố định (Time Management / Communication / Critical Thinking) với Id 1-2-3 nhất quán xuyên suốt code.
- [x] **Entry Test (bài test đầu vào):**
  - [x] Đủ 24 câu hỏi (8 câu/kỹ năng), file `entry-test-questions.json`.
  - [x] Logic chấm điểm & quy đổi mức độ (Passive/Developing/Proactive/Exceptional) đã khớp đúng khoảng điểm, không có lỗ hổng.
- [x] Cơ chế mini-game random 5/20 câu mỗi lượt chơi (`MiniGameService.GetRandomQuestionsAsync`) — đã code đúng logic, chỉ đợi đủ data.
- [x] Roadmap, Progress, Mentor/Community, Auth (login/register/forgot password) — luồng cơ bản đã có UI + service.
- [x] Case Study — đã có entity, service, trang UI (`/case-studies`), gắn vào nav menu.

---

## 2. Chưa làm / Chưa đạt (❌ Not Done)

### 2.1 Lý thuyết + Video bài giảng — **mức độ ưu tiên cao nhất**
- [ ] Nội dung lý thuyết 3 kỹ năng hiện chỉ là JSON định nghĩa rời rạc (gạch đầu dòng khái niệm), **chưa phải bài học hoàn chỉnh**.
- [ ] Code hiện đang seed **nguyên văn JSON thô** vào `ContentMarkdown` (bọc trong code block) → khi lên web người dùng sẽ thấy JSON kỹ thuật, không đọc được.
- [ ] Còn sót dấu trích dẫn `[cite: 474]`, `[cite: 475]`... từ tài liệu gốc, chưa làm sạch.
- [ ] Chưa có link video nào (`VideoUrl` đang trống toàn bộ) — đợi các cô quay.
- [ ] Chưa xác định số lượng bài lý thuyết / kỹ năng (hiện code seed cứng 1 bài / kỹ năng).

### 2.2 Câu hỏi trắc nghiệm (quiz sau lý thuyết)
- [ ] **Chưa có entity/schema riêng** cho loại "câu hỏi trắc nghiệm kiểm tra hiểu bài" như mô tả trong yêu cầu (lý thuyết → video → trắc nghiệm → mini game). Hiện chỉ có Entry Test (1 lần duy nhất) và Mini Game (luyện tập), thiếu bước quiz trung gian.

### 2.3 Mini Game
- [ ] Data hiện chỉ có **6 tình huống cho cả 3 kỹ năng** (`practical-scenarios.json`), trong khi mục tiêu là **~20 câu/game × 3–5 game/kỹ năng** (~60–100 câu/kỹ năng).
- [ ] Code hiện đang **gộp toàn bộ câu hỏi của 1 kỹ năng vào đúng 1 mini-game** — chưa tách thành 3–5 game riêng biệt.
- [ ] 2 dạng câu hỏi `FACT_OR_OPINION` và `IDENTIFY_FALLACY` (Tư duy phản biện) **bị code bỏ qua khi seed** vì chưa map được sang format `options[]` chuẩn → mất dữ liệu, cần chuẩn hóa schema.

### 2.4 Case Study
- [ ] Data hiện tại là 2 case-study **tiếng Anh demo cũ**, chưa liên quan tới 3 kỹ năng target.
- [ ] Bị gán sai `SkillId`: case "Group Communication" (nội dung giao tiếp) đang gán `SkillId = 1` (Quản lý thời gian); case "Missed Deadline" (nội dung thời gian) đang gán `SkillId = 3` (Tư duy phản biện).

### 2.5 Dọn dẹp kỹ thuật (technical debt)
- [ ] `AssessmentService` / `AssessmentQuestion` / `AssessmentOption` là bộ code legacy trùng chức năng với `EntryTestService`, không còn được UI gọi tới — cần xóa để tránh nhầm lẫn khi bàn giao.

---

## 3. Cần chuẩn bị (🔜 To Prepare)

### Từ phía các cô / nội dung chuyên môn
- [ ] Bài giảng lý thuyết hoàn chỉnh cho từng kỹ năng (dạng văn bản/markdown, có mở đầu – nội dung – ví dụ – tổng kết), thay cho bản định nghĩa rời rạc hiện tại.
- [ ] Video bài giảng/demo (link hoặc file) cho từng bài lý thuyết.
- [ ] Duyệt lại nội dung, cách đặt câu hỏi và mức độ phù hợp của 24 câu Entry Test hiện có.
- [ ] Bổ sung ngân hàng câu hỏi mini-game: mục tiêu ~20 câu/game, 3–5 game/kỹ năng (ưu tiên format `MULTIPLE_CHOICE` có options + điểm để tương thích code hiện tại).
- [ ] Nội dung case study tiếng Việt phù hợp 3 kỹ năng (nếu giữ tính năng này trong bản demo).
- [ ] Tiêu chí đánh giá/chấm điểm mẫu cho từng loại câu hỏi (đặc biệt là 2 dạng chưa map được: Fact vs Opinion, nhận diện ngụy biện).

### Từ phía team dev
- [ ] Quyết định có cần tách riêng "câu hỏi trắc nghiệm" khỏi "mini game" hay gộp chung — cập nhật schema nếu cần.
- [ ] Refactor `JsonContentSeeder.SeedMiniGamesAsync` để tách nhiều mini-game/kỹ năng thay vì gộp 1 game duy nhất.
- [ ] Chuẩn hóa schema JSON cho scenario để không bỏ sót dạng `FACT_OR_OPINION` / `IDENTIFY_FALLACY`.
- [ ] Viết lại `ContentMarkdown` thành markdown thực (không phải JSON thô) khi có nội dung bài giảng từ các cô.
- [ ] Sửa lại data Case Study (nội dung + SkillId đúng) hoặc quyết định ẩn tính năng này khỏi bản demo nếu chưa kịp chuẩn bị data.
- [ ] Dọn code legacy (`AssessmentService` và liên quan).

---

*Ghi chú: Checklist này tổng hợp từ việc rà soát source code + seed data thực tế trong repomix export ngày 07/07/2026. Phần AI (chatbot, roadmap AI, assessment AI) chưa được đánh giá theo yêu cầu.*

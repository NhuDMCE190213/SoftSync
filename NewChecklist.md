[ ] BUG-01 (Critical — root cause): ProfileSetup.razor tạo user trùng lặp
    - Sửa HandleSubmit(): không gọi CreateUserAsync nữa.
    - Inject CurrentUserAccessor, lấy userId thật từ auth claim.
    - Gọi method MỚI: UserService.UpdateProfileAsync(userId, dto) 
      (cần thêm vào IUserService — hiện chưa có method update, chỉ có Create).
    - Prefill form bằng dữ liệu user thật (GetUserByIdAsync) thay vì để trống,
      để nếu Register đã có FullName/Age/Goal thì không bắt nhập lại.
    - Navigate: $"/select-skills/{userId}" dùng ID thật, không dùng created.Id.

[ ] BUG-02: UserService.GetUserByIdAsync thiếu map Email
    - Thêm Email = user.Email vào UserDto trả về.

[ ] BUG-03: CreateUserAsync không nên tồn tại nữa cho luồng này
    - Nếu không còn nơi nào khác dùng CreateUserAsync, cân nhắc xoá hẳn 
      để tránh bị gọi nhầm lần nữa; hoặc giữ lại chỉ dùng cho mục đích khác 
      (vd tạo user từ admin), đổi tên rõ ràng hơn.

[ ] BUG-04 (hệ quả của BUG-01, tự hết khi sửa xong BUG-01):
    Roadmap không hiện "Bắt đầu học" vì weaknesses rỗng do sai userId.
    → Verify lại sau khi sửa BUG-01: làm test thật từ đầu, roadmap phải 
      sinh ra RoadmapItem có SkillId cho từng skill yếu.

[ ] DESIGN-01: Entry test hỏi cả skill chưa chọn
    - Quyết định hướng: 
      (a) Lọc GetQuestionsAsync() theo selectedSkillIds trước khi trả về, hoặc 
      (b) Giữ nguyên (test toàn diện) nhưng thêm dòng giải thích trên UI 
          Assessment.razor: "Bài test đánh giá đủ 3 trụ kỹ năng để có điểm nền, 
          lộ trình sẽ chỉ tập trung vào skill bạn đã chọn."
    - Nếu chọn (a): cần EntryTestService.GetQuestionsAsync(int userId) nhận 
      thêm userId, join với UserSkillSelection để lọc.

[ ] REGRESSION CHECK sau khi sửa:
    - Test lại toàn bộ luồng: Register → (không phải nhập lại info) → 
      SelectSkills → Assessment → AssessmentResult → Roadmap (phải thấy 
      nút "Bắt đầu học" cho từng skill đã chọn).
    - Kiểm tra Profile.razor hiện đúng Email sau khi sửa BUG-02.
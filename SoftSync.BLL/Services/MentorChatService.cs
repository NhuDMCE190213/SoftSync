using SoftSync.BLL.Interfaces;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

// MVP: Mentor chưa có tài khoản/đăng nhập thật (xem Mentor.cs - chỉ là profile tĩnh).
// Thay vì xây auth riêng cho mentor + SignalR Hub + dashboard (quá nhiều cho MVP với team hiện tại),
// service này TỰ SINH phản hồi mô phỏng từ mentor ngay sau khi user gửi tin, để demo có "2 chiều"
// mà không cần người thật đăng nhập.
//
// TODO [Phase 2 - khi có thời gian/cần thật]: nếu muốn mentor là người thật trả lời:
//   1) Thêm Role vào User (Student/Mentor), liên kết Mentor.UserId -> User.Id, cho mentor login qua /login sẵn có.
//   2) Bỏ AddSimulatedMentorReplyAsync bên dưới.
//   3) Đổi MentorChat.razor thành trang dùng chung, render theo Role (student: 1 khung chat;
//      mentor: danh sách hội thoại - "inbox").
//   4) (Tuỳ chọn) Thay Timer polling bằng SignalR Hub để đẩy tin nhắn realtime thay vì chờ 3s.
public class MentorChatService : IMentorChatService
{
    private readonly IMentorChatRepository _repo;
    private readonly IMentorRepository _mentorRepo;
    public MentorChatService(IMentorChatRepository repo, IMentorRepository mentorRepo)
    {
        _repo = repo;
        _mentorRepo = mentorRepo;
    }

    public async Task<List<(string, string, DateTime)>> GetConversationAsync(int userId, int mentorId)
    {
        var conv = await _repo.GetOrCreateConversationAsync(userId, mentorId);
        var messages = await _repo.GetMessagesAsync(conv.Id);
        return messages.Select(m => (m.SenderType.ToString(), m.Content, m.CreatedAt)).ToList();
    }

    public async Task SendMessageAsync(int userId, int mentorId, string content)
    {
        var conv = await _repo.GetOrCreateConversationAsync(userId, mentorId);
        await _repo.AddMessageAsync(new MentorMessage
        {
            ConversationId = conv.Id,
            SenderType = MentorMessageSender.User,
            Content = content
        });
        await _repo.SaveChangesAsync();

        await AddSimulatedMentorReplyAsync(conv.Id, mentorId, content);
    }

    private async Task AddSimulatedMentorReplyAsync(int conversationId, int mentorId, string userContent)
    {
        var mentor = await _mentorRepo.GetByIdAsync(mentorId);
        string mentorName = mentor?.Name ?? "Mentor";

        string reply = $"Chào bạn, mình là {mentorName}. Mình đã nhận được tin nhắn: " +
                        $"\"{Truncate(userContent, 60)}\". Mình sẽ phản hồi chi tiết sớm nhất có thể!";

        await _repo.AddMessageAsync(new MentorMessage
        {
            ConversationId = conversationId,
            SenderType = MentorMessageSender.Mentor,
            Content = reply
        });
        await _repo.SaveChangesAsync();
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "...";
}
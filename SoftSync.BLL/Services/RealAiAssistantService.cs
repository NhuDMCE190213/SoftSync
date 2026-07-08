using SoftSync.BLL.Interfaces;
using System.Net.Http.Json;

namespace SoftSync.BLL.Services
{
    public class RealAiAssistantService : IAiAssistantService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public RealAiAssistantService(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public async Task<string> GetReplyAsync(string userMessage, int userId)
        {
            var client = _httpClientFactory.CreateClient("AiApi");
            var response = await client.PostAsJsonAsync("/chat", new { message = userMessage, userId });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ChatReplyDto>();
            return result?.Reply ?? "Xin lỗi, hiện tại tôi chưa thể trả lời.";
        }

        private record ChatReplyDto(string Reply);
    }
}

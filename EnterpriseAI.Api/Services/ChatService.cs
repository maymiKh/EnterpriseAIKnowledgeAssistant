using Microsoft.Extensions.AI;

namespace EnterpriseAI.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatClient _chatClient;

        public ChatService(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }
        public async Task<string> GetResponse(string message, CancellationToken cancellationToken = default)
        {
            var result = await _chatClient.GetResponseAsync(message, cancellationToken: cancellationToken);

            return result.Text;
        }
    }
}

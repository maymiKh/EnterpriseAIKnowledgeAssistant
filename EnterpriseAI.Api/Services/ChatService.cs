using Microsoft.Extensions.AI;

namespace EnterpriseAI.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatClient _chatClient;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IChatClient chatClient, ILogger<ChatService> logger)
        {
            _chatClient = chatClient;
            _logger = logger;
        }
        public async Task<string> GetResponse(string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing AI chat request");
            try {
                var result = await _chatClient.GetResponseAsync(message, cancellationToken: cancellationToken);
                _logger.LogInformation("AI chat request completed");

                return result.Text;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing AI chat request");
                throw;
            }
        }
    }
}

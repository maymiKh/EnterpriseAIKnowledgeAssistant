using EnterpriseAI.Api.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace EnterpriseAI.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatClient _chatClient;
        private readonly ILogger<ChatService> _logger;
        private readonly AiOptions _aiOptions;

        public ChatService(IChatClient chatClient, ILogger<ChatService> logger, IOptions<AiOptions> aiOptions)
        {
            _chatClient = chatClient;
            _logger = logger;
            _aiOptions = aiOptions.Value;
        }
        public async Task<string> GetResponse(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing AI chat request");

            using var timeoutCts =
            new CancellationTokenSource(
                TimeSpan.FromSeconds(
                    _aiOptions.RequestTimeoutSeconds));

            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutCts.Token);

            try {
                var response = await _chatClient.GetResponseAsync(
                 message,
                 cancellationToken: linkedCts.Token);

                _logger.LogInformation("AI chat request completed");

                return response.Text;
            }
            catch (OperationCanceledException)
            when (timeoutCts.IsCancellationRequested &&
                  !cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException(
                    "The AI model did not respond within 30 seconds.");
            }
        }
    }
}

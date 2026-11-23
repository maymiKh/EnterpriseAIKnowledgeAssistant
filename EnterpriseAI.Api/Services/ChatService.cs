using EnterpriseAI.Api.Configuration;
using EnterpriseAI.Api.Infrastructure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace EnterpriseAI.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatClient _chatClient;
        private readonly ILogger<ChatService> _logger;
        private readonly AiOptions _aiOptions;
        private readonly AiConcurrencyLimiter _concurrencyLimiter;
        public ChatService(IChatClient chatClient, ILogger<ChatService> logger, IOptions<AiOptions> aiOptions, AiConcurrencyLimiter concurrencyLimiter)
        {
            _chatClient = chatClient;
            _logger = logger;
            _aiOptions = aiOptions.Value;
            _concurrencyLimiter = concurrencyLimiter;
        }
        public async Task<string> GetResponse(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing AI chat request");

            await _concurrencyLimiter.WaitAsync(cancellationToken);
            try { 
            using var timeoutCts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(
                        _aiOptions.RequestTimeoutSeconds));

            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutCts.Token);
            try
            {

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
            finally
            {
                // Always release the slot after acquiring it.
                _concurrencyLimiter.Release();
            }
}
    }
}

namespace EnterpriseAI.Api.Services
{
    public interface IChatService
    {
        Task<string> GetResponse(string message, CancellationToken cancellationToken = default);
    }
}

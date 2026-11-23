namespace EnterpriseAI.Api.Infrastructure
{
    public class AiConcurrencyLimiter
    {
        private readonly SemaphoreSlim _semaphore;
        public AiConcurrencyLimiter()
        {
            _semaphore = new SemaphoreSlim(2, 2);
        }
        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
        }
        public void Release()
        {
            _semaphore.Release();
        }
    }
}

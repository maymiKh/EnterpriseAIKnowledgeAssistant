namespace EnterpriseAI.Api.Exercises
{
    public class AsyncExercises
    {
        private int _activeRequests;
        private int _maximumActiveRequests;

        public async Task<string> GetDataAsync()
        {
            Console.WriteLine("Starting operation...");

            await Task.Delay(2000);

            Console.WriteLine("Operation completed.");

            return "Data received";
        }

        public async Task<string> SimulateAiRequestAsync(
    int requestId,
    CancellationToken cancellationToken)
        {
            var active = Interlocked.Increment(ref _activeRequests);

            UpdateMaximum(active);

            Console.WriteLine(
                $"Request {requestId} STARTED. Active: {active}");

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);

                return $"Request {requestId} completed";
            }
            finally
            {
                var remaining = Interlocked.Decrement(ref _activeRequests);

                Console.WriteLine(
                    $"Request {requestId} FINISHED. Active: {remaining}");
            }
        }

        private void UpdateMaximum(int active)
        {
            int currentMaximum;

            do
            {
                currentMaximum = Volatile.Read(ref _maximumActiveRequests);

                if (active <= currentMaximum)
                {
                    return;
                }
            }
            while (
                Interlocked.CompareExchange(
                    ref _maximumActiveRequests,
                    active,
                    currentMaximum) != currentMaximum);
        }

        public int GetMaximumActiveRequests()
        {
            return Volatile.Read(ref _maximumActiveRequests);
        }
    }
}

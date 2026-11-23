namespace EnterpriseAI.Api.Exercises
{
    public class AsyncExercises
    {
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
            Console.WriteLine(
                $"Request {requestId} started at {DateTime.Now:HH:mm:ss}");

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                cancellationToken);

            Console.WriteLine(
                $"Request {requestId} completed at {DateTime.Now:HH:mm:ss}");

            return $"Request {requestId} completed";
        }

    }
}

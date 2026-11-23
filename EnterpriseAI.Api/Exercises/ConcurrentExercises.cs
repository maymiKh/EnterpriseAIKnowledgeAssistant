namespace EnterpriseAI.Api.Exercises
{
    public class ConcurrentExercises
    {
        public async Task<string> GetUserAsync()
        {
            await Task.Delay(2000);
            return "User information";
        }

        public async Task<string> GetHistoryAsync()
        {
            await Task.Delay(3000);
            return "Conversation history";
        }

        public async Task<string> GetConfigurationAsync()
        {
            await Task.Delay(1000);
            return "Application configuration";
        }

        public async Task<string[]> GetDataSequentiallyAsync()
        {
            var user = await GetUserAsync();

            var history = await GetHistoryAsync();

            var configuration = await GetConfigurationAsync();

            return [user, history, configuration];
        }


        public async Task<string[]> GetDataConcurrentlyAsync()
        {
            // Start all operations
            var userTask = GetUserAsync();

            var historyTask = GetHistoryAsync();

            var configurationTask = GetConfigurationAsync();

            // Wait until all operations finish
            await Task.WhenAll(
                userTask,
                historyTask,
                configurationTask);

            // Retrieve their results
            return
            [
                await userTask,
                await historyTask,
                await configurationTask
            ];
        }

        public async Task<string[]> GetDataConcurrentlyAnyAsync()
        {
            // Start all operations
            var userTask = GetUserAsync();

            var historyTask = GetHistoryAsync();

            var configurationTask = GetConfigurationAsync();

            // Wait until all operations finish
            var result = await Task.WhenAny(
                userTask,
                historyTask,
                configurationTask);
            Console.WriteLine($"First completed task result: {await result}");
            // Retrieve their results
            return
            [
                await userTask,
                await historyTask,
                await configurationTask
            ];
        }


        public async Task<string> ProcessAsync(
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Processing started...");

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                cancellationToken);

            Console.WriteLine("Processing completed.");

            return "Success";
        }
    }
}

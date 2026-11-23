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

    }
}

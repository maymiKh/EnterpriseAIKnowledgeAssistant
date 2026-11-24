namespace EnterpriseAI.Api.Services
{
    public class RequestCounter
    {
        private int _count = 0;

        public int Increment()
        {
            return Interlocked.Increment(ref _count);
        }

        public int GetCount()
        {
            return Volatile.Read(ref _count);
        }
    }
}

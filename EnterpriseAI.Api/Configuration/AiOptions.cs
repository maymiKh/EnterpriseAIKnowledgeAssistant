namespace EnterpriseAI.Api.Configuration
{
    public class AiOptions
    {
        public const string SectionName = "AI";

        public int RequestTimeoutSeconds { get; set; } = 30;
    }
}

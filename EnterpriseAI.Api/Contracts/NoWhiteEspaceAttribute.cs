using System.ComponentModel.DataAnnotations;

namespace EnterpriseAI.Api.Contracts
{
    public class NoWhiteEspaceAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is string text &&
               !string.IsNullOrWhiteSpace(text);
        }   
    }
}

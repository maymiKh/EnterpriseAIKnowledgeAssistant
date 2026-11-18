using System.ComponentModel.DataAnnotations;

namespace EnterpriseAI.Api.Contracts
{
    public record ChatRequest([Required(ErrorMessage = "Message is required")]
    [NoWhiteEspace(ErrorMessage = "Message cannot be empty or whitespace")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 2000 characters")] 
    string Message);
}

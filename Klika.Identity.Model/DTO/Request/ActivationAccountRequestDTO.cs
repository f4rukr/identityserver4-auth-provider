using Klika.Identity.Model.Constants.Regex;
using System.ComponentModel.DataAnnotations;

namespace Klika.Identity.Model.DTO.Request
{
    public class ActivationAccountRequestDTO
    {
        [Required]
        [RegularExpression(RegexValidators.EmailRegex, ErrorMessage = RegexMessages.InvalidEmail)]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}

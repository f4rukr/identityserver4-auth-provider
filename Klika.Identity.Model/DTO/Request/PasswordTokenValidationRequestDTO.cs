using Klika.Identity.Model.Constants.Regex;
using System.ComponentModel.DataAnnotations;

namespace Klika.Identity.Model.DTO.Request
{
    public class PasswordTokenValidationRequestDTO
    {
        [Required]
        [RegularExpression(RegexValidators.EmailRegex, ErrorMessage = RegexMessages.InvalidEmail)]
        public string Email { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = RegexMessages.TokenTooShort)]
        public string Token { get; set; }
    }
}

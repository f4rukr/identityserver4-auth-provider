using Klika.Identity.Model.Constants.Regex;
using System.ComponentModel.DataAnnotations;

namespace Klika.Identity.Model.DTO.Request
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        [RegularExpression(RegexValidators.EmailRegex, ErrorMessage = RegexMessages.InvalidEmail)]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [RegularExpression(RegexValidators.Password, ErrorMessage = RegexMessages.InvalidPassword)]
        [MaxLength(30, ErrorMessage = RegexMessages.TooLongPassword)]
        public string Password { get; set; }

    }
}

using Klika.Identity.Model.Constants.Regex;
using System.ComponentModel.DataAnnotations;

namespace Klika.Identity.Model.DTO.Request
{
    public class ApplicationUserRequestDTO
    {
        [Required]
        [RegularExpression(RegexValidators.FirstName, ErrorMessage = RegexMessages.InvalidFirstName)]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(RegexValidators.LastName, ErrorMessage = RegexMessages.InvalidLastName)]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(RegexValidators.EmailRegex, ErrorMessage = RegexMessages.InvalidEmail)]
        public string Email { get; set; }
        [Required]
        [RegularExpression(RegexValidators.Password, ErrorMessage = RegexMessages.InvalidPassword)]
        [MaxLength(30, ErrorMessage = RegexMessages.TooLongPassword)]
        public string Password { get; set; }
    }
}

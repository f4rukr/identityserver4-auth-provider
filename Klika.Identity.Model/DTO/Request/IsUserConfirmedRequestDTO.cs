using System.ComponentModel.DataAnnotations;
using Klika.Identity.Model.Constants.Regex;

namespace Klika.Identity.Model.DTO.Request
{
    public class IsUserConfirmedRequestDTO
    {
        [Required]
        [RegularExpression(RegexValidators.EmailRegex, ErrorMessage = RegexMessages.InvalidEmail)]
        public string Email { get; set; }
    }
}
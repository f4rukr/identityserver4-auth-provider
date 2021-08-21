using Klika.Identity.Model.Constants.Regex;
using System.ComponentModel.DataAnnotations;

namespace Klika.Identity.Model.DTO.Request
{
    public class ChangePasswordRequestDTO
    {
        [RegularExpression(RegexValidators.Password, ErrorMessage = RegexMessages.InvalidPassword)]
        [MaxLength(30, ErrorMessage = RegexMessages.TooLongPassword)]
        public string Password { get; set; }
    }
}

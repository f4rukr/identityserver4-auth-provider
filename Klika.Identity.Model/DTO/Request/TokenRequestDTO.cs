using Klika.Identity.Model.Constants.Regex;
using Klika.Identity.Model.Extensions.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Klika.Identity.Model.DTO.Request
{
    public class TokenRequestDTO
    {
        [Required] 
        public string GrantType { get; set; }

        [RequiredIf("GrantType", "password")]
        [RegularExpression(RegexValidators.EmailRegex, ErrorMessage = RegexMessages.InvalidEmail)]
        public string Email { get; set; }

        [RequiredIf("GrantType", "password")]
        [RegularExpression(RegexValidators.Password, ErrorMessage = RegexMessages.InvalidPassword)]
        [MaxLength(30, ErrorMessage = RegexMessages.TooLongPassword)]
        public string Password { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        [Required]
        public string Scope { get; set; }

        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRequestDTO
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}

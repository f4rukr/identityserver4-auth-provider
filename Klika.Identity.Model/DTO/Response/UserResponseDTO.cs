using System;

namespace Klika.Identity.Model.DTO.Response
{
    public class UserResponseDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

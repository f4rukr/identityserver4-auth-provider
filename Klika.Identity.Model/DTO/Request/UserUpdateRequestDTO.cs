using System.Text.Json.Serialization;

namespace Klika.Identity.Model.DTO.Request
{
    public class UserUpdateRequestDTO
    {
        [JsonIgnore]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

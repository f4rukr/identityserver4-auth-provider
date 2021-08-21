namespace Klika.Identity.Model.DTO.Response
{
    public class ApplicationUserResponseDTO
    {
        public string UserId {get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public ApplicationUserResponseDTO(string userId, string firstName, string lastName, string email, bool isEmailConfirmed)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            IsEmailConfirmed = isEmailConfirmed;
        }
        public ApplicationUserResponseDTO() { }
    }
}

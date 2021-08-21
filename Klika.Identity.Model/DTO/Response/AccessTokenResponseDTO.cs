namespace Klika.Identity.Model.DTO.Response
{
    public class AccessTokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public AccessTokenResponseDTO(string accesToken, string refreshToken, int expiresIn)
        {
            AccessToken = accesToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
        public AccessTokenResponseDTO() { }
    }
}

namespace Klika.Identity.Model.DTO.Response
{
    public class RefreshTokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public RefreshTokenResponseDTO(string accesToken, string refreshToken, int expiresIn)
        {
            AccessToken = accesToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
        public RefreshTokenResponseDTO() { }
    }
}

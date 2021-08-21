using IdentityModel.Client;
using Klika.Identity.Model.DTO.Request;
using Klika.Identity.Model.DTO.Response;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Klika.Identity.Model.Interfaces.User
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(ApplicationUserRequestDTO request);
        Task<IdentityResult> ValidateVerificationTokenAsync(ActivationAccountRequestDTO request);
        Task<IdentityResult> SendConfirmationEmailAsync(string email);
        Task<TokenResponse> TokenAsync(TokenRequestDTO request);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequestDTO request);
        Task<TokenRevocationResponse> RevokeTokenAsync(RefreshTokenRequestDTO request);
        Task<UserResponseDTO> GetUserAsync(string userId, string email);
        Task<IdentityResult> DeleteUserAsync(string email);
        Task<IdentityResult> RequestResetPasswordTokenAsync(RequestResetPasswordRequestDTO request);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequestDTO request);
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordRequestDTO request, string userId);
        Task<IdentityResult> ValidateResetPasswordTokenAsync(PasswordTokenValidationRequestDTO request);
        Task<IdentityResult> EditUserAsync(UserUpdateRequestDTO request);

    }
}

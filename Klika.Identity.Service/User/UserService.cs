using AutoMapper;
using IdentityModel.Client;
using Klika.Identity.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Klika.Identity.Model.Constants.Roles;
using System.Transactions;
using Klika.Identity.Model.Constants.Email;
using Klika.Identity.Service.Extensions;
using Klika.Identity.Model.Constants.TokenProviders;
using IdentityModel;
using Klika.Identity.Database.DbContexts;
using Klika.Identity.Model.Configuration.Email;
using Klika.Identity.Model.DTO.Request;
using Klika.Identity.Model.DTO.Response;
using Klika.Identity.Model.Interfaces.User;
using Klika.Identity.Model.Interfaces.Mailer;
using Klika.Identity.Model.Extensions.UserChecks;

namespace Klika.Identity.Service.User
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IMailerService _mailerService;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IdentityDbContext _identityDbContext;

        public UserService(
                IMapper mapper,
                UserManager<ApplicationUser> userManager,
                IConfiguration config,
                IHttpClientFactory clientFactory,
                ILogger<UserService> logger,
                IMailerService mailerService,
                IOptions<EmailSettings> emailSettings)
        {
            _userManager = userManager;
            _config = config;
            _clientFactory = clientFactory;
            _mapper = mapper;
            _logger = logger;
            _mailerService = mailerService;
            _emailSettings = emailSettings;
        }

        public async Task<IdentityResult> RegisterAsync(ApplicationUserRequestDTO request)
        {
            try
            {
                ApplicationUser applicationUser = _mapper.Map<ApplicationUserRequestDTO, ApplicationUser>(request);

                IdentityResult identityResult = null;

                using (TransactionScope scope = new(scopeOption: TransactionScopeOption.Required,
                    transactionOptions: new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = TimeSpan.Zero},
                    asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled))
                {
                    identityResult = await _userManager.CreateAsync(applicationUser, request.Password).ConfigureAwait(false);

                    if (identityResult.Succeeded)
                    {
                        await _userManager.AddClaimsAsync(applicationUser, new List<Claim>() {
                            new Claim("email", applicationUser.Email)
                        }).ConfigureAwait(false);
                        await _userManager.AddToRolesAsync(applicationUser, new List<string>() { Roles.UserBasic })
                            .ConfigureAwait(false); // Define user roles on registration

                        identityResult = await SendConfirmationEmailAsync(applicationUser.Email).ConfigureAwait(false);                        
                    }
                    scope.Complete();                   
                }
                return identityResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RegisterAsync));
                throw;
            }
        }

        public async Task<IdentityResult> SendConfirmationEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

                var validationResult = user.UserExistsByEmailAndNotConfirmed();

                if (validationResult.Succeeded)
                {
                    var result = await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

                        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                        var confirmationLink =
                            $"{_emailSettings.Value.FrontendAppUrl}/user/sign-up/activate-account?email={Uri.EscapeDataString(user.Email)}&token={token}";

                        await _mailerService.SendEmailAsync(user, EmailConstants.UserActivation, confirmationLink).ConfigureAwait(false);

                        return IdentityResult.Success;
                    }
                    return result;
                }
                return validationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(SendConfirmationEmailAsync));
                throw;
            }
        }

        public async Task<IdentityResult> ValidateVerificationTokenAsync(ActivationAccountRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

                var validationResult = user.UserExistsByEmailAndNotConfirmed();

                if (validationResult.Succeeded)
                {
                    string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

                    var result = await _userManager.ConfirmEmailAsync(user, decodedToken).ConfigureAwait(false);

                    return result;
                }

                return validationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ValidateVerificationTokenAsync));
                throw;
            }
        }

        public async Task<TokenResponse> TokenAsync(TokenRequestDTO request)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var cache = new DiscoveryCache(_config["AuthApiUrl"]);
                var disco = await cache.GetAsync()
                    .ConfigureAwait(false);
                if (disco.IsError)
                    throw new Exception(disco.Error);

                switch (request.GrantType)
                {
                    case OidcConstants.GrantTypes.Password:
                        var passwordFlow = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                        {
                            Address = disco.TokenEndpoint,
                            ClientId = request.ClientId,
                            ClientSecret = request.ClientSecret,
                            Scope = request.Scope,
                            UserName = request.Email,
                            Password = request.Password
                        }).ConfigureAwait(false);

                        return passwordFlow;
                    case OidcConstants.GrantTypes.ClientCredentials:
                        var clientCredentialsFlow = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                            {
                                Address = disco.TokenEndpoint,
                                ClientId = request.ClientId,
                                ClientSecret = request.ClientSecret,
                                Scope = request.Scope,
                            }).ConfigureAwait(false);

                        return clientCredentialsFlow;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(TokenAsync));
                throw;
            }
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var cache = new DiscoveryCache(_config["AuthApiUrl"]);
                var disco = await cache.GetAsync()
                                       .ConfigureAwait(false);
                if (disco.IsError)
                    throw new Exception(disco.Error);

                var refreshToken = await client.RequestRefreshTokenAsync(new RefreshTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    RefreshToken = request.RefreshToken
                }).ConfigureAwait(false);
                
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RefreshTokenAsync));
                throw;
            }
        }

        public async Task<TokenRevocationResponse> RevokeTokenAsync(RefreshTokenRequestDTO request)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var cache = new DiscoveryCache(_config["AuthApiUrl"]);
                var disco = await cache.GetAsync()
                                       .ConfigureAwait(false);
                
                if (disco.IsError)
                    throw new Exception(disco.Error);

                var revokeResult = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = disco.RevocationEndpoint,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    Token = request.RefreshToken
                }).ConfigureAwait(false);
                
                return revokeResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RevokeTokenAsync));
                throw;
            }
        }

        public async Task<UserResponseDTO> GetUserAsync(string userId, string email)
        {
            try
            {
                ApplicationUser user = null;
                if (!string.IsNullOrEmpty(userId))
                {
                    user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
                    return user is null ? null : _mapper.Map<ApplicationUser, UserResponseDTO>(user);
                }

                if (!string.IsNullOrEmpty(email))
                {
                    user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
                    return user is null ? null : new UserResponseDTO()
                    {
                        IsEmailConfirmed = user.EmailConfirmed
                    }; 
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetUserAsync));
                throw;
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(string email)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

                var validationResult = user.UserExistsWithEmail();

                if (!validationResult.Succeeded)
                    return validationResult;

                var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(DeleteUserAsync));
                throw;
            }
        }

        public async Task<IdentityResult> RequestResetPasswordTokenAsync(RequestResetPasswordRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

                var validationResult = user.UserExistsAndConfirmed();

                if (validationResult.Succeeded)
                {
                    var result = await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

                        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                        var confirmationLink =
                            $"{_emailSettings.Value.FrontendAppUrl}/user/login/reset-password?email={Uri.EscapeDataString(user.Email)}&token={token}";

                        await _mailerService.SendEmailAsync(user, EmailConstants.ResetPassword, confirmationLink).ConfigureAwait(false);

                        return IdentityResult.Success;
                    }

                    return result;
                }
                return validationResult;                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RequestResetPasswordTokenAsync));
                throw;
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

                var validationResult = user.UserExistsAndConfirmed();

                if (validationResult.Succeeded)
                {
                    var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

                    var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password).ConfigureAwait(false);

                    return result;
                }
                
                return validationResult;                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ResetPasswordAsync));
                throw;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordRequestDTO request, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

                var userValidation = user.UserExistsAndConfirmed();

                if (userValidation.Succeeded)
                {
                    return await _userManager.ChangePasswordAsync(user, request.Password).ConfigureAwait(false);
                }

                return userValidation;                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ChangePasswordAsync));
                throw;
            }
        }

        public async Task<IdentityResult> ValidateResetPasswordTokenAsync(PasswordTokenValidationRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

                var validationResult = user.UserExistsAndConfirmed();

                if (validationResult.Succeeded)
                {
                    string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

                    var result = await _userManager.VerifyUserTokenAsync(user, CustomTokenProviders.PasswordDataProtectorTokenProvider, UserManager<ApplicationUser>.ResetPasswordTokenPurpose, decodedToken).ConfigureAwait(false);

                    var identityErrorDescriber = new IdentityErrorDescriber();

                    return result ? IdentityResult.Success :
                    IdentityResult.Failed(new IdentityError[]
                    {
                        identityErrorDescriber.InvalidToken()
                    });
                }
                return validationResult;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, nameof(ValidateResetPasswordTokenAsync));
                throw;
            }
        }

        public async Task<IdentityResult> EditUserAsync(UserUpdateRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId).ConfigureAwait(false);

                var validationResult = user.UserExistsAndConfirmed();

                if (validationResult.Succeeded)
                {
                    user.FirstName = request.FirstName;
                    user.LastName = request.LastName;
                    var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
                    return result;
                }

                return validationResult;

            }catch(Exception ex)
            {
                _logger.LogError(ex, nameof(EditUserAsync));
                throw;
            }
        }
    }
}

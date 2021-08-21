using Klika.Identity.Model.Constants.Errors;
using Klika.Identity.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Klika.Identity.Model.DTO.Request;
using Klika.Identity.Model.DTO.Response;
using Klika.Identity.Model.Interfaces.User;
using Klika.Identity.Model.Extensions.UserChecks;

namespace Klika.Identity.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(IUserService userService,
                                  ILogger<IdentityController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// New user account registration
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdentityResult>> Register([FromBody] ApplicationUserRequestDTO request)
        {          
            try
            {
                var result = await _userService.RegisterAsync(request).ConfigureAwait(false);

                if (!result.Succeeded)
                    return BadRequest(result);

                return CreatedAtAction(nameof(GetUser), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/register");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Sends email for user activation
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("confirm")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SendConfirmationEmail([FromBody] ResendConfirmationEmailRequestDTO request)
        {
            try
            {
                var result = await _userService.SendConfirmationEmailAsync(request.Email).ConfigureAwait(false);

                if(result.Succeeded)
                    return Ok();

                return result.ParseUserNotFoundOrBadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/confirm");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Activates an account with email and token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("activate")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IdentityResult>> ActivateAccount([FromBody] ActivationAccountRequestDTO request)
        {
            try
            {
                var result = await _userService.ValidateVerificationTokenAsync(request).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok(result);

                return result.ParseUserNotFoundOrBadRequest();                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/activate");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Provides access and refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("token")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AccessTokenResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<AccessTokenResponseDTO>> Token([FromBody] TokenRequestDTO request)
        {
            try
            {
                var result = await _userService.TokenAsync(request).ConfigureAwait(false);

                if (result is null)
                    return BadRequest(new ApiErrorResponse(ErrorCodes.InvalidGrant, ErrorDescriptions.InvalidGrantType));

                if (result.IsError)
                    return BadRequest(new ApiErrorResponse(result.Error, result.ErrorDescription));

                return Ok(new AccessTokenResponseDTO(
                    result.AccessToken,
                    result.RefreshToken,
                    result.ExpiresIn));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/token");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Provides new access and refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("token/refresh")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RefreshTokenResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<RefreshTokenResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                var result = await _userService.RefreshTokenAsync(request).ConfigureAwait(false);

                if (result.IsError)
                    return BadRequest(new ApiErrorResponse(result.Error, result.ErrorDescription));

                return Ok(new RefreshTokenResponseDTO(
                    result.AccessToken, 
                    result.RefreshToken, 
                    result.ExpiresIn));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/token/refresh");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Revokes refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("token/revoke")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                var result = await _userService.RevokeTokenAsync(request).ConfigureAwait(false);

                if (result.IsError)
                    return BadRequest(new ApiErrorResponse(result.Error, null));

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/token/revoke");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Checks if user is confirmed
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("users/isConfirmed")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IsUserConfirmedResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<IsUserConfirmedResponseDTO>> IsConfirmed([FromQuery] IsUserConfirmedRequestDTO request)
        {
            try
            {
                var result = await _userService.GetUserAsync(null, request.Email).ConfigureAwait(false);
                
                if (result is null)
                    return NotFound(new ApiErrorResponse(ErrorCodes.UserNotFound, ErrorDescriptions.UserDoesNotExistWithEmail));

                return Ok(new IsUserConfirmedResponseDTO { IsEmailConfirmed = result.IsEmailConfirmed});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET:/users/isConfirmed");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets user information
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponseDTO>> GetUser()
        {
            try
            {
                var result = await _userService.GetUserAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, null).ConfigureAwait(false);
                
                if (result is null)
                    return NotFound(new ApiErrorResponse(ErrorCodes.UserNotFound, ErrorDescriptions.UserDoesNotExistWithId));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET:/users");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("users")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        public async Task<ActionResult> EditUser([FromBody] UserUpdateRequestDTO request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                request.UserId = userId;

                var result = await _userService.EditUserAsync(request).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PUT:/users");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Sends email with password reset link
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("password/request-reset")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<IdentityResult>> RequestResetPassword([FromBody] RequestResetPasswordRequestDTO request)
        {
            try
            {
                var result = await _userService.RequestResetPasswordTokenAsync(request).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok();

                return result.ParseUserNotFoundOrBadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/password/request-reset");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Resets password 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("password/reset")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<IdentityResult>> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            try
            {
                var result = await _userService.ResetPasswordAsync(request).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok();

                return result.ParseUserNotFoundOrBadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/password/reset");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Changes password 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("password/change")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<IdentityResult>> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _userService.ChangePasswordAsync(request, userId).ConfigureAwait(false);

                if (!result.Succeeded)
                    return BadRequest(result);
                return Ok();

            }catch(Exception ex)
            {
                _logger.LogError(ex, "POST:/password/change");
                return StatusCode(StatusCodes.Status500InternalServerError);
            } 
        }

        /// <summary>
        /// Validates reset password token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("password/token-validate")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<IdentityResult>> ValidateResetPasswordToken([FromBody] PasswordTokenValidationRequestDTO request)
        {
            try
            {
                var result = await _userService.ValidateResetPasswordTokenAsync(request).ConfigureAwait(false);
                
                if (result.Succeeded)
                    return Ok();

                return result.ParseUserNotFoundOrBadRequest();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "POST:/password/token-validate");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        /// <summary>
        /// Deletes user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpDelete("users")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser([FromQuery] string email)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(email).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok();

                return result.ParseUserNotFoundOrBadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DELETE:/users");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

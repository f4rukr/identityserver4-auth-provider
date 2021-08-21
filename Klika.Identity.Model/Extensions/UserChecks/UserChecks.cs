using Klika.Identity.Model.Constants.Errors;
using Klika.Identity.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Klika.Identity.Model.Extensions.UserChecks
{
    public static class UserChecks
    {
        public static IdentityResult UserExistsByEmailAndNotConfirmed(this ApplicationUser user)
        {
            List<IdentityError> identityErrors = new();

            if (user is null)
            {
                identityErrors.Add(new IdentityError
                {
                    Code = ErrorCodes.UserNotFound,
                    Description = ErrorDescriptions.UserDoesNotExistWithEmail
                });
            }
            else if (user.EmailConfirmed)
            {
                identityErrors.Add(new IdentityError
                {
                    Code = ErrorCodes.UserAlreadyVerified,
                    Description = ErrorDescriptions.UserAlreadyVerified
                });
            }

            if (identityErrors.Count > 0)
                return IdentityResult.Failed(identityErrors.ToArray());

            return IdentityResult.Success;
        }

        public static IdentityResult UserExistsWithEmail(this ApplicationUser user)
        {
            if (user is null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = ErrorCodes.UserNotFound,
                    Description = ErrorDescriptions.UserDoesNotExistWithEmail
                });
            }
            return IdentityResult.Success;
        }

        public static IdentityResult UserExistsAndConfirmed(this ApplicationUser user)
        {
            List<IdentityError> identityErrors = new();

            if (user is null)
            {
                identityErrors.Add(new IdentityError
                {
                    Code = ErrorCodes.UserNotFound,
                    Description = ErrorDescriptions.UserDoesNotExistWithEmail
                });
            }
            else if (!user.EmailConfirmed)
            {
                identityErrors.Add(new IdentityError
                {
                    Code = ErrorCodes.UserNotVerified,
                    Description = ErrorDescriptions.EmailNotConfirmed
                });
            }

            if (identityErrors.Count > 0)
                return IdentityResult.Failed(identityErrors.ToArray());

            return IdentityResult.Success;
        }

        public static JsonResult ParseUserNotFoundOrBadRequest(this IdentityResult result)
        {
            List<IdentityError> identityErrors = result.Errors.ToList();

            foreach (var error in identityErrors)
            {
                if (error.Code == ErrorCodes.UserNotFound)
                {
                    return new JsonResult(result)
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                else return new JsonResult(result)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            return new JsonResult(new object { }){ 
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}

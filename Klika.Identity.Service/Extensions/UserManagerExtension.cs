using Klika.Identity.Model.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Klika.Identity.Service.Extensions
{
    public static class UserManagerExtension
    {
        public static async Task<List<IdentityError>> ValidatePasswordAsync(UserManager<ApplicationUser> userManager, string password)
        {
            List<IdentityError> passwordErrors = new();

            var validators = userManager.PasswordValidators;

            foreach (var validator in validators)
            {
                var validatorResult = await validator.ValidateAsync(userManager, null, password).ConfigureAwait(false);

                if (!validatorResult.Succeeded)
                {
                    foreach (var error in validatorResult.Errors)
                    {
                        passwordErrors.Add(
                            new IdentityError
                            {
                                Code = error.Code,
                                Description = error.Description
                            });
                    }
                }
            }

            return passwordErrors;
        }

        public static async Task<IdentityResult> ChangePasswordAsync(this UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string password)
        {
            var passwordErrors = await ValidatePasswordAsync(userManager, password).ConfigureAwait(false);

            if (passwordErrors.Count > 0)
            {
                return IdentityResult.Failed(passwordErrors.ToArray());
            }

            string passwordHash = userManager.PasswordHasher.HashPassword(user, password);

            user.PasswordHash = passwordHash;

            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);

            return result;            
        }
    }
}

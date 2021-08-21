using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Klika.Identity.Model.Interfaces.User
{
    public interface IIdentityUserService<TUser> where TUser : IdentityUser
    {
        Task<TUser> FindByNameAsync(string userName);
        Task<IList<Claim>> GetClaimsAsync(TUser user);
        Task<IList<string>> GetRolesAsync(TUser user);
        PasswordVerificationResult VerifyHashedPassword(TUser user, string password);
    }
}

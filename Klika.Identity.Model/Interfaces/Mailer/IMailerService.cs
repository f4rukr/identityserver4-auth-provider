using Klika.Identity.Model.Entities;
using System.Threading.Tasks;

namespace Klika.Identity.Model.Interfaces.Mailer
{
    public interface IMailerService
    {
        Task SendEmailAsync(ApplicationUser to, string subject, string body);
    }
}

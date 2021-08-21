using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Klika.Identity.Model.Constants.Errors;
using Microsoft.Extensions.Options;
using Klika.Identity.Model.Entities;
using Klika.Identity.Model.Constants.Email;
using Klika.Identity.Model.Responses;
using Klika.Identity.Model.Configuration.Email;
using Klika.Identity.Model.Interfaces.Mailer;

namespace Klika.Identity.Service.Mailer
{
    public class MailerService : IMailerService
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly ILogger<MailerService> _logger;

        public MailerService(IOptions<EmailSettings> emailSettings, ILogger<MailerService> logger)
        {
            _emailSettings = emailSettings;
            _logger = logger;

        }

        /// <summary>
        /// Returns HTML template by subject
        /// </summary>
        /// <param name="subject"></param>
        private string GetHtmlTemplate(string subject, string FirstName, string link)
        {
            return subject switch
            {
                EmailConstants.UserActivation => MailTemplate.BuildConfirmationEmail(FirstName, link),
                EmailConstants.ResetPassword => MailTemplate.BuildForgotPasswordEmail(FirstName, link),
                _ => throw new Exception("Email subject is not supported"),
            };
        }

        /// <summary>
        /// Sends email with activation anchor so that user can activate it's account.
        /// </summary>
        /// <param name="to">Registered user</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Activation url</param>
        /// <returns></returns>
        public async Task SendEmailAsync(ApplicationUser to, string subject, string body)
        {
            using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.EUCentral1))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = _emailSettings.Value.From,
                    Destination = new Destination
                    {
                        ToAddresses =
                        new List<string> { to.Email }
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = GetHtmlTemplate(subject, to.FirstName, body)
                            }
                        }
                    },
                };

                try
                {
                    await client.SendEmailAsync(sendRequest).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, nameof(SendEmailAsync));
                    throw new ApiErrorException(ApiErrorTypes.FailedToDeliverMail);
                }
            }
        }
        
    }
}

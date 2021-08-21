namespace Klika.Identity.Model.Configuration.Email
{
    public static class MailTemplate
    {
        public static string BuildConfirmationEmail(string firstName, string activationLink)
        {
            return $@"
                <html>
                    <head></head>
                        <body>
                            <div style='font-weight: 400; font-size: 16px; line-height: 24px; font-style: normal;'>
                                <p>Hi {firstName} </p>
                        
                                <p>We are happy to welcome you to DiNero! Click the link below to confirm your email address. </p>
                                <a href='{activationLink}' style='margin-top: 20px''>
                                Verify
                                </a>
                        
                                <br>
                                <p> This link is valid only for the next 30 minutes.</p>
                        
                                <p>Thanks,</p>
                                <p>Your DiNero Team</p>
                                Link didn’t work? Try copy-pasting it into your browser’s address bar:
                                <p>{activationLink}</p>
                                </p>
                            </div>
                       </body>
                 </html>";
        }

        public static string BuildForgotPasswordEmail(string firstName, string activationLink)
        {
            return $@"
                    <html>
                    <head></head>
                        <body>
                            <div style='font-weight: 400; font-size: 16px; line-height: 24px; font-style: normal;'>
                                <p>Hi {firstName}, </p>
                                <p> You recently requested to reset your password for DiNero app. Click the link below to reset it.</p>
                                <a href='{activationLink}' style='margin-top: 20px''>Reset password </a>
                        
                                <p>This password reset is only valid for the next 30 minutes.</p>
                                <p>Thanks,</p>
                                <p>Your DiNero team</p>
                            </div>
                        </body>
                    </html>
                    ";
        }
    }
}

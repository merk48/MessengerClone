namespace MessengerClone.Domain.Utils.Constants.Email
{
    public static class EmailTemplates
    {
        // Shared header & footer for consistent branding
        private const string Header = @"
      <html>
      <body style=""font-family:Arial,sans-serif;color:#333;"">
        <div style=""max-width:600px;margin:0 auto;padding:20px;border:1px solid #eee;"">
          <h2 style=""text-align:center;color:#0056b3;"">MyApp</h2>
    ";

        private const string Footer = @"
          <p style=""font-size:12px;color:#999;"">
            If you didn’t request this, you can safely ignore this email.
          </p>
        </div>
      </body>
      </html>
    ";

        /// <summary>Verification code (email or phone confirmation).</summary>
        public static string VerificationCode(string code, string targetLabel, int? expirationTime = 15)
        {
            return Header +
                   $@"<p>Hi there,</p>
                   <p>Your <strong>{targetLabel}</strong> verification code is:</p>
                   <p style=""font-size:24px;font-weight:bold;text-align:center;"">{code}</p>
                   <p>This code will expire in {expirationTime} minutes.</p>"
                   + Footer;
        }

        /// <summary>Password reset via link flow.</summary>
        public static string PasswordReset(string resetLink)
        {
            return Header +
                   $@"<p>Hi there,</p>
                   <p>Click the button below to reset your password:</p>
                   <p style=""text-align:center;margin:30px 0;"">
                     <a href=""{resetLink}"" 
                        style=""background:#0056b3;color:#fff;
                               padding:12px 24px;text-decoration:none;
                               border-radius:4px;"">
                       Reset Password
                     </a>
                   </p>
                   <p>If the button doesn’t work, paste this URL into your browser:</p>
                   <p><small>{resetLink}</small></p>"
                   + Footer;
        }

        /// <summary>Change email confirmation (code flow).</summary>
        public static string ChangeEmailCode(string code, string newEmail)
        {
            return Header +
                   $@"<p>You requested changing your email to <strong>{newEmail}</strong>.</p>
                   <p>Please confirm by entering this code:</p>
                   <p style=""font-size:24px;font-weight:bold;text-align:center;"">{code}</p>"
                   + Footer;
        }

        /// <summary>Change email confirmation (link flow).</summary>
        public static string ChangeEmailLink(string confirmLink, string newEmail)
        {
            return Header +
                   $@"<p>You requested changing your email to <strong>{newEmail}</strong>.</p>
                   <p>Click to confirm:</p>
                   <p style=""text-align:center;margin:30px 0;"">
                     <a href=""{confirmLink}""
                        style=""background:#0056b3;color:#fff;
                               padding:12px 24px;text-decoration:none;
                               border-radius:4px;"">
                       Confirm New Email
                     </a>
                   </p>
                   <p>Or paste this URL in your browser:</p>
                   <p><small>{confirmLink}</small></p>"
                   + Footer;
        }

        /// <summary>Change phone confirmation (code flow).</summary>
        public static string ChangePhoneCode(string code, string newPhone)
        {
            return Header +
                   $@"<p>You requested changing your phone number to <strong>{newPhone}</strong>.</p>
                   <p>Please confirm by entering this code:</p>
                   <p style=""font-size:24px;font-weight:bold;text-align:center;"">{code}</p>"
                   + Footer;
        }

        /// <summary>Two-factor authentication code.</summary>
        public static string TwoFactorCode(string code)
        {
            return Header +
                   $@"<p>Hi there,</p>
                   <p>Your two-factor authentication code is:</p>
                   <p style=""font-size:24px;font-weight:bold;text-align:center;"">{code}</p>
                   <p>If you didn’t attempt to sign in, you can ignore this.</p>"
                   + Footer;
        }

        /// <summary>Generic notification (e.g., “profile updated”).</summary>
        public static string Notification(string subject, string messageHtml)
        {
            return Header +
                   $@"<h3>{subject}</h3>
                   {messageHtml}"
                   + Footer;
        }
    }

}

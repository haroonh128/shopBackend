using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Shop.Common.Constants;
using Shop.Core.Interfaces.Services;

namespace Shop.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendOtpAsync(string toEmail, string otpCode)
        {
            try
            {
                var host = _configuration["Email:SmtpHost"];
                var portText = _configuration["Email:SmtpPort"];
                var username = _configuration["Email:Username"];
                var password = _configuration["Email:Password"];
                var fromAddress = _configuration["Email:FromAddress"];
                var fromName = _configuration["Email:FromName"] ?? "ShopPOS";

                if (string.IsNullOrWhiteSpace(host) ||
                    string.IsNullOrWhiteSpace(fromAddress) ||
                    !int.TryParse(portText, out var port))
                {
                    _logger.LogError("Email SMTP settings are missing or invalid");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    username = fromAddress;
                }

                if (string.IsNullOrWhiteSpace(password) ||
                    password.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError(
                        "Email password is not configured. For Gmail, use an App Password (not your normal password).");
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromAddress));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = "Your PIN reset code";
                message.Body = new TextPart("plain")
                {
                    Text =
                        $"Your PIN reset code is: {otpCode}. " +
                        $"Valid for {AppConstants.OtpExpiryMinutes} minutes."
                };

                using var client = new SmtpClient();
                // Port 587 = STARTTLS; port 465 = SSL on connect
                var secureSocketOptions = port == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

                await client.ConnectAsync(host, port, secureSocketOptions);
                await client.AuthenticateAsync(username.Trim(), password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
                return false;
            }
        }
    }
}

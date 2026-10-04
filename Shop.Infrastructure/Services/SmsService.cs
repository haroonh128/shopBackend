using Microsoft.Extensions.Configuration;
using Shop.Core.Interfaces.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Shop.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendOtpAsync(string phoneNumber, string otpCode)
        {
            try
            {
                var accountSid = _configuration["Twilio:AccountSid"];
                var authToken = _configuration["Twilio:AuthToken"];
                var fromNumber = _configuration["Twilio:PhoneNumber"];

                TwilioClient.Init(accountSid, authToken);

                var message = await MessageResource.CreateAsync(
                    body: $"Your verification code is: {otpCode}. Valid for 5 minutes.",
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                return message.Status != MessageResource.StatusEnum.Failed;
            }
            catch
            {
                return false;
            }
        }
    }
}

using Api.Services.Email;

namespace Test.Util;

public class FakeEmailService :IEmailService
{
        public Task SendAsync(string to, string subject, string body)
            => Task.CompletedTask;

        public Task<string> SendEmail(string from, string to, string subject, string body)
        {
            return Task.FromResult("Sent");
        }
}
namespace Api.Services.Email;

public interface IEmailService
{
    public Task<string> SendEmail(string from,  string to, string subject, string body);
}
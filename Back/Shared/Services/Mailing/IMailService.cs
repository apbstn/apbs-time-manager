using Shared.Models.Mailing;

namespace Shared.Services.Mailing;

public interface IMailService
{
    public Task SendEmailAsync(MailParams mailParams);
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.Mailing;

namespace Shared.Services.Mailing;

public class SmtpMailService : IMailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<SmtpMailService> _logger;

    public SmtpMailService(IOptions<SmtpSettings> smtpSettings,
                         ILogger<SmtpMailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(MailParams mailParams)
    {
        using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
            Subject = mailParams.Subject,
            Body = mailParams.Body,
            IsBodyHtml = mailParams.IsBodyHtml
        };
        mailMessage.To.Add(mailParams.ToEmail);

        try
        {
            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent to {ToEmail}", mailParams.ToEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {ToEmail}", mailParams.ToEmail);
            throw;
        }
    }
}

using System.Net;
using System.Net.Mail;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CreatorHub.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        // No SMTP configured (local development): log the message so the reset link can be copied.
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            _logger.LogWarning("Email:Host is not configured. Email to {To} - {Subject}: {Body}",
                toEmail, subject, htmlBody);
            return;
        }

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        using var message = new MailMessage(
            new MailAddress(_settings.FromEmail, _settings.FromName),
            new MailAddress(toEmail))
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
    }
}

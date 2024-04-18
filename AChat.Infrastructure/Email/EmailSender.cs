using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using AChat.Application.Common.Configurations;
using AChat.Application.Common.Interfaces;

namespace AChat.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailConfig;
    private readonly ILogger<EmailSender> _logger;
    public EmailSender(IOptions<EmailSettings> emailConfig,
                       ILogger<EmailSender> logger)
    {
        _logger = logger;
        _emailConfig = emailConfig.Value;
    }
    
    public async Task SendEmailAsync(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        await SendAsync(emailMessage);
        _logger.LogInformation($"Email sent to {message.To}.");
    }
    
    private MimeMessage CreateEmailMessage(Message message, bool isHtml = true)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_emailConfig.UserName ,_emailConfig.From));
        emailMessage.To.AddRange(message.To.Select(x => new MailboxAddress(string.Empty, x)));
        emailMessage.Subject = message.Subject;
        // send as html 
        emailMessage.Body = new TextPart(isHtml ? "html" : "plain")
        {
            Text = message.Content
        };
        return emailMessage;
    }
    private async Task SendAsync(MimeMessage mailMessage)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                await client.SendAsync(mailMessage);
            }
            catch
            {
                _logger.LogError($"Email sending failed to {mailMessage.To}.");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}

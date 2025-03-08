using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using MailKit.Security;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(toEmail))
        {
            throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(toEmail));
        }

        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("Email subject cannot be null or empty.", nameof(subject));
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Email message cannot be null or empty.", nameof(message));
        }

        var emailSettings = _config.GetSection("EmailSettings");

        if (emailSettings == null)
        {
            throw new InvalidOperationException("EmailSettings configuration is missing.");
        }

        var senderName = emailSettings["SenderName"];
        var senderEmail = emailSettings["SenderEmail"];
        var smtpServer = emailSettings["SmtpServer"];
        var port = emailSettings["Port"];
        var password = emailSettings["Password"];

        if (string.IsNullOrEmpty(senderName) || string.IsNullOrEmpty(senderEmail) ||
            string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(port) ||
            string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("One or more required EmailSettings values are missing.");
        }

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(senderName, senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = message
        };

        try
        {
            using var smtp = new SmtpClient();

            // Connect to the SMTP server
            await smtp.ConnectAsync(smtpServer, int.Parse(port), SecureSocketOptions.StartTls);

            // Authenticate using the sender's email and app password
            await smtp.AuthenticateAsync(senderEmail, password);

            // Send the email
            await smtp.SendAsync(email);

            // Disconnect from the SMTP server
            await smtp.DisconnectAsync(true);

            Console.WriteLine("Email sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw; // Re-throw the exception to allow calling code to handle it
        }
    }
}